// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings_PointController.cs
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
    public class Settings_PointController : AdminUniqueTenantBaseController<Settings_Point>
    {
        [RoleMenuFilter("积分设置", "CDF75E43-A366-4EBE-B8A3-C696F0F7AACC", "Admin,TenantManager,ShopManager",
             url: "/Settings_Point", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        // GET: Settings_Point
        [AuditFilter("积分设置", "20C89E06-0B86-4B40-97C8-DBC9E02A8F4F")]
        public override ActionResult Index()
        {
            return base.Index();
        }

        [HttpPost]
        public override ActionResult Index(Settings_Point model)
        {
            return base.Index(model);
        }

        //[HttpPost]
        //public ActionResult Index(Settings_TemplateMessage model)
        //{
        //    SetModelWithChangeStates(model, model.Id);
        //    db.SaveChanges();
        //    return View(model);
        //}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}