// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : TenantSettingsController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Linq;
using System.Web.Mvc;
using Magicodes.WeiChat.ComponentModel.Setting;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.WeiChat.Infrastructure.Config;

namespace Magicodes.Shop.Controllers
{
    public class TenantSettingsController : TenantBaseController<WeiChat_App>
    {
        private readonly SettingManager settingManager = SettingManager.Current;
        // GET: TenantSettings
        public ActionResult Index()
        {
            var groups = settingManager.GetTenantSettingGroup(TenantId);
            if (!groups.Any(p => p.Name == "站点设置"))
            {
                settingManager.AddOrUpdateTenantGroupSetting(new App_SettingGroup
                {
                    Description = "设置网站基础信息以及相关配置",
                    IsVisibleToClients = false,
                    Name = "站点设置",
                    Scopes = SettingScopes.Application,
                    DisplayName = "站点设置"
                }, TenantId);
                settingManager.AddOrUpdateTenantGroupSetting(new App_SettingGroup
                {
                    Description = "设置服务器事件的开关",
                    IsVisibleToClients = false,
                    Name = "服务器事件设置",
                    Scopes = SettingScopes.Application,
                    DisplayName = "服务器事件设置"
                }, TenantId);
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetGroups()
        {
            return Json(settingManager.GetTenantSettingGroup(TenantId), JsonRequestBehavior.AllowGet);
        }
    }
}