// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings_OrderController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;

namespace Magicodes.Shop.Controllers.Settings
{
    public class Settings_OrderController : AdminUniqueTenantBaseController<Settings_Order>
    {
        // GET: Settings_Order
        [RoleMenuFilter("订单设置", "8FAE9D8B-DA85-4183-AE41-0CA727078C0C", "Admin,TenantManager,ShopManager",
             url: "/Settings_Order", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        [AuditFilter("订单设置", "6C25E1C7-1C3D-4555-9452-191CBAB48FCE")]
        public override ActionResult Index()
        {
            var model = db.Set<Settings_Order>().FirstOrDefault();
            if (model == null)
            {
                model = new Settings_Order
                {
                    CreateBy = UserId,
                    CreateTime = DateTime.Now,
                    TenantId = TenantId,
                    AutomaticGoodsReceiptDays = 1
                };
                db.Set<Settings_Order>().Add(model);
                db.SaveChanges();
            }
            ViewBag.Success = false;
            ViewBag.Message = "";
            return View(model);

            return base.Index();
        }

        [HttpPost]
        public override ActionResult Index(Settings_Order model)
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