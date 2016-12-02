// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SettingManager.cs
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
using System.Collections.Generic;
using System.Linq;
using Magicodes.WeiChat.ComponentModel.Setting;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.Infrastructure.Config
{
    public class SettingManager : ThreadSafeLazyBaseSingleton<SettingManager>
    {
        internal ConcurrentDictionary<string, ISettingGroup> ApplicationGroupSettings =
            new ConcurrentDictionary<string, ISettingGroup>();

        internal ConcurrentDictionary<string, ISettingValue> ApplicationSettings =
            new ConcurrentDictionary<string, ISettingValue>();

        internal ConcurrentDictionary<int, ConcurrentDictionary<string, ISettingGroup>> TenantGroupSettings =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, ISettingGroup>>();

        internal ConcurrentDictionary<int, ConcurrentDictionary<string, ISettingValue>> TenantSettings =
            new ConcurrentDictionary<int, ConcurrentDictionary<string, ISettingValue>>();

        public SettingManager()
        {
            using (var db = new AppDbContext())
            {
                var applicationValues = db.App_SettingValues.Where(p => p.Scopes == SettingScopes.Application);
                foreach (var item in applicationValues)
                    ApplicationSettings.TryAdd(item.Name, item);
                var applicationGroups = db.App_SettingGroups.Where(p => p.Scopes == SettingScopes.Application);
                foreach (var item in applicationGroups)
                    ApplicationGroupSettings.TryAdd(item.Name, item);
            }
        }

        /// <summary>
        ///     获取全局设置信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISettingValue GetApplicationSettingValue(string name)
        {
            if (ApplicationSettings.ContainsKey(name))
                return ApplicationSettings[name];
            return null;
        }

        public ICollection<ISettingValue> GetApplicationSettingValue()
        {
            return ApplicationSettings.Values;
        }

        public ISettingGroup GetApplicationSettingGroup(string name)
        {
            if (ApplicationGroupSettings.ContainsKey(name))
                return ApplicationGroupSettings[name];
            return null;
        }

        public ICollection<ISettingGroup> GetApplicationSettingGroup()
        {
            return ApplicationGroupSettings.Values;
        }

        public ICollection<ISettingValue> GetTenantSettingValue(int? tenantId = null)
        {
            return GetTenantSettings(tenantId).Values;
        }

        private ConcurrentDictionary<string, ISettingValue> GetTenantSettings(int? tenantId)
        {
            var theTenantId = tenantId == null ? WeiChatApplicationContext.Current.TenantId : tenantId.Value;
            var scopes = SettingScopes.Tenant;
            ConcurrentDictionary<string, ISettingValue> settings = null;
            if (TenantSettings.ContainsKey(theTenantId))
            {
                settings = TenantSettings[theTenantId];
            }
            else
            {
                settings = new ConcurrentDictionary<string, ISettingValue>();
                using (var db = new AppDbContext())
                {
                    var tenantSettings =
                        db.App_SettingValues.Where(p => (p.Scopes == scopes) && (p.TenantId == theTenantId));
                    foreach (var item in tenantSettings)
                        settings.TryAdd(item.Name, item);
                    TenantSettings.TryAdd(theTenantId, settings);
                }
            }
            return settings;
        }

        /// <summary>
        ///     获取租户设置信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public ISettingValue GetTenantSettingValue(string name, int? tenantId = null)
        {
            var settings = GetTenantSettings(tenantId);
            return settings.ContainsKey(name) ? settings[name] : null;
        }

        public ISettingGroup GetTenantSettingGroup(string name, int? tenantId = null)
        {
            var settings = GetTenantSettingGroupDictionary(tenantId);
            return settings.ContainsKey(name) ? settings[name] : null;
        }

        public ICollection<ISettingGroup> GetTenantSettingGroup(int? tenantId = null)
        {
            return GetTenantSettingGroupDictionary(tenantId).Values;
        }

        private ConcurrentDictionary<string, ISettingGroup> GetTenantSettingGroupDictionary(int? tenantId)
        {
            var theTenantId = tenantId == null ? WeiChatApplicationContext.Current.TenantId : tenantId.Value;
            var scopes = SettingScopes.Tenant;
            ConcurrentDictionary<string, ISettingGroup> settings = null;
            if (TenantGroupSettings.ContainsKey(theTenantId))
            {
                settings = TenantGroupSettings[theTenantId];
            }
            else
            {
                settings = new ConcurrentDictionary<string, ISettingGroup>();
                using (var db = new AppDbContext())
                {
                    var tenantSettings =
                        db.App_SettingGroups.Where(p => (p.Scopes == scopes) && (p.TenantId == theTenantId));
                    foreach (var item in tenantSettings)
                        settings.TryAdd(item.Name, item);
                    TenantGroupSettings.TryAdd(theTenantId, settings);
                }
            }

            return settings;
        }

        public ISettingValue GetUserSettingValue(string name, string userId = null)
        {
            using (var db = new AppDbContext())
            {
                var theUserId = userId == null ? WeiChatApplicationContext.Current.UserId : userId;
                return
                    db.App_SettingValues.FirstOrDefault(
                        p => (p.Scopes == SettingScopes.User) && (p.UserId == theUserId) && (p.Name == name));
            }
        }

        public ISettingGroup GetUserSettingGroup(string name, string userId = null)
        {
            using (var db = new AppDbContext())
            {
                var theUserId = userId == null ? WeiChatApplicationContext.Current.UserId : userId;
                return
                    db.App_SettingGroups.FirstOrDefault(
                        p => (p.Scopes == SettingScopes.User) && (p.UserId == theUserId) && (p.Name == name));
            }
        }

        public ISettingGroup AddOrUpdateApplicationGroupSetting(ISettingGroup group)
        {
            using (var db = new AppDbContext())
            {
                var scopes = SettingScopes.Application;
                var groupInfo = db.App_SettingGroups.FirstOrDefault(p => (p.Name == group.Name) && (p.Scopes == scopes));
                var isAdd = groupInfo == null;
                if (groupInfo == null) groupInfo = new App_SettingGroup();
                groupInfo.IsVisibleToClients = group.IsVisibleToClients;
                groupInfo.Name = group.Name;
                groupInfo.Scopes = scopes;
                groupInfo.UpdateTime = DateTime.Now;
                groupInfo.DisplayName = group.DisplayName;
                groupInfo.Description = group.Description;
                groupInfo.CreateBy = WeiChatApplicationContext.Current.UserId;
                if (group.ParentGroup != null)
                {
                    var parentGroup =
                        db.App_SettingGroups.FirstOrDefault(
                            p => (p.Name == group.ParentGroup.Name) && (p.Scopes == scopes));
                    if (parentGroup != null)
                        groupInfo.ParentId = parentGroup.Id;
                    else
                        throw new ArgumentException("ParentGroup不存在", "ParentGroup");
                }
                if (isAdd) db.App_SettingGroups.Add(groupInfo);
                db.SaveChanges();
                ApplicationGroupSettings.AddOrUpdate(groupInfo.Name, groupInfo,
                    (tKey, existingVal) => { return groupInfo; });
                return groupInfo;
            }
        }

        public ISettingGroup AddOrUpdateTenantGroupSetting(ISettingGroup group, int? tenantId)
        {
            using (var db = new AppDbContext())
            {
                var scopes = SettingScopes.Tenant;
                var theTenantId = tenantId == null ? WeiChatApplicationContext.Current.TenantId : tenantId.Value;
                var groupInfo = db.App_SettingGroups.FirstOrDefault(p => (p.Name == group.Name) && (p.Scopes == scopes));
                var isAdd = groupInfo == null;
                if (groupInfo == null) groupInfo = new App_SettingGroup();
                groupInfo.IsVisibleToClients = group.IsVisibleToClients;
                groupInfo.Name = group.Name;
                groupInfo.Scopes = scopes;
                groupInfo.UpdateTime = DateTime.Now;
                groupInfo.DisplayName = group.DisplayName;
                groupInfo.Description = group.Description;
                groupInfo.TenantId = theTenantId;
                groupInfo.CreateBy = WeiChatApplicationContext.Current.UserId;
                if (group.ParentGroup != null)
                {
                    var parentGroup =
                        db.App_SettingGroups.FirstOrDefault(
                            p => (p.Name == group.ParentGroup.Name) && (p.Scopes == scopes));
                    if (parentGroup != null)
                        groupInfo.ParentId = parentGroup.Id;
                    else
                        throw new ArgumentException("ParentGroup不存在", "ParentGroup");
                }
                if (isAdd) db.App_SettingGroups.Add(groupInfo);
                db.SaveChanges();

                var settings = GetTenantSettingGroupDictionary(theTenantId);
                settings.AddOrUpdate(groupInfo.Name, groupInfo, (tKey, existingVal) => { return groupInfo; });
                return groupInfo;
            }
        }
    }
}