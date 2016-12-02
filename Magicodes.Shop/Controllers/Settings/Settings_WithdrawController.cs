// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings_WithdrawController.cs
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
    public class Settings_WithdrawController : AdminUniqueTenantBaseController<Settings_Withdraw>
    {
        [RoleMenuFilter("提现设置", "BF186AF6-6A33-43AA-92C1-CE855387A491", "Admin,TenantManager,ShopManager",
             url: "/Settings_Withdraw", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        // GET: Settings_Withdraw
        [AuditFilter("提现设置", "195d175f-eb23-4bc9-b57d-d85ac32bdbe4")]
        public override ActionResult Index()
        {
            return base.Index();
        }

        // POST: Settings_Withdraw/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public override ActionResult Index(Settings_Withdraw model)
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