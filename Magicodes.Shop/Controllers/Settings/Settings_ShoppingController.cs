// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings_ShoppingController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models.Settings;

namespace Magicodes.Shop.Controllers.Settings
{
    public class Settings_ShoppingController : AdminUniqueTenantBaseController<Settings_Shopping>
    {
        // GET: Settings_Shopping
        [RoleMenuFilter("购物设置", "4E706318-C682-4347-85C3-A3F6204A628D", "Admin,TenantManager,ShopManager",
             url: "/Settings_Shopping", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        [AuditFilter("购物设置", "401673EC-ACC9-4E9C-B681-F3C6AB49E75A")]
        public override ActionResult Index()
        {
            return base.Index();
        }

        [HttpPost]
        public override ActionResult Index(Settings_Shopping model)
        {
            return base.Index(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}