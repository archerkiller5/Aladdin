// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings_ProductController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;

namespace Magicodes.Shop.Controllers.Settings
{
    public class Settings_ProductController : AdminUniqueTenantBaseController<Settings_Product>
    {
        // GET: Settings_Product
        [RoleMenuFilter("产品(商品)设置", "8409B8CB-12D4-4792-9DE8-46EEC9321E1B", "Admin,TenantManager,ShopManager",
             url: "/Settings_Product", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        [AuditFilter("产品(商品)设置", "63e316a1-9358-4b23-af07-c9c508f35370")]
        public override ActionResult Index()
        {
            return base.Index();
        }


        [HttpPost]
        public override ActionResult Index(Settings_Product model)
        {
            //Settings_Product
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