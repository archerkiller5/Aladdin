// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SettingExtension.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Concurrent;
using System.Linq;
using Magicodes.WeiChat.ComponentModel.Setting;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Settings;

namespace Magicodes.WeiChat.Infrastructure.Config
{
    public static class SettingExtension
    {
        public static ISettingValue AddOrUpdateApplicationSettingValue(this ISettingGroup group, ISettingValue value)
        {
            using (var db = new AppDbContext())
            {
                var scopes = SettingScopes.Application;
                var groupInfo = db.App_SettingGroups.FirstOrDefault(p => (p.Name == group.Name) && (p.Scopes == scopes));

                if (groupInfo == null)
                    throw new Exception("名称为" + group.Name + "的配置组不存在。");
                var settingValue =
                    db.App_SettingValues.FirstOrDefault(p => (p.Name == value.Name) && (p.Scopes == scopes));
                var isAdd = settingValue == null;
                if (settingValue == null) settingValue = new App_SettingValue();
                settingValue.IsVisibleToClients = value.IsVisibleToClients;
                settingValue.Scopes = scopes;
                settingValue.UpdateTime = DateTime.Now;
                settingValue.DisplayName = value.DisplayName;
                settingValue.Name = value.Name;
                settingValue.Description = value.Description;
                settingValue.CreateBy = WeiChatApplicationContext.Current.UserId;
                settingValue.Value = value.Value;
                settingValue.GroupId = groupInfo.Id;
                if (isAdd) db.App_SettingValues.Add(settingValue);
                db.SaveChanges();
                SettingManager.Current.ApplicationSettings.AddOrUpdate(value.Name, settingValue,
                    (tKey, existingVal) => { return settingValue; });
                return settingValue;
            }
        }

        public static ISettingValue AddOrUpdateTenantSettingValue(this ISettingGroup group, ISettingValue value,
            int? tenantId)
        {
            using (var db = new AppDbContext())
            {
                var scopes = SettingScopes.Tenant;
                var theTenantId = tenantId == null ? WeiChatApplicationContext.Current.TenantId : tenantId.Value;
                var groupInfo = db.App_SettingGroups.FirstOrDefault(p => (p.Name == group.Name) && (p.Scopes == scopes));

                if (groupInfo == null)
                    throw new Exception("名称为" + group.Name + "的配置组不存在。");
                var settingValue =
                    db.App_SettingValues.FirstOrDefault(p => (p.Name == value.Name) && (p.Scopes == scopes));
                var isAdd = settingValue == null;
                if (settingValue == null) settingValue = new App_SettingValue();
                settingValue.IsVisibleToClients = value.IsVisibleToClients;
                settingValue.Scopes = scopes;
                settingValue.UpdateTime = DateTime.Now;
                settingValue.DisplayName = value.DisplayName;
                settingValue.Name = value.Name;
                settingValue.Description = value.Description;
                settingValue.CreateBy = WeiChatApplicationContext.Current.UserId;
                settingValue.Value = value.Value;
                settingValue.GroupId = groupInfo.Id;
                settingValue.CustomData = value.CustomData;
                settingValue.TenantId = theTenantId;
                if (isAdd) db.App_SettingValues.Add(settingValue);
                db.SaveChanges();

                ConcurrentDictionary<string, ISettingValue> settings = null;
                if (SettingManager.Current.TenantSettings.ContainsKey(theTenantId))
                {
                    settings = SettingManager.Current.TenantSettings[theTenantId];
                }
                else
                {
                    settings = new ConcurrentDictionary<string, ISettingValue>();
                    {
                        var tenantSettings =
                            db.App_SettingValues.Where(p => (p.Scopes == scopes) && (p.TenantId == theTenantId));
                        foreach (var item in tenantSettings)
                            settings.TryAdd(item.Name, item);
                        SettingManager.Current.TenantSettings.TryAdd(theTenantId, settings);
                    }
                }
                settings.AddOrUpdate(settingValue.Name, settingValue, (tKey, existingVal) => { return settingValue; });
                return settingValue;
            }
        }
    }
}