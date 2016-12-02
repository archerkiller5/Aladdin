// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings_TemplateMessageController.cs
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
    public class Settings_TemplateMessageController : AdminUniqueTenantBaseController<Settings_TemplateMessage>
    {
        // GET: Settings_TemplateMessage
        [RoleMenuFilter("模板消息设置", "5EA2D0C7-6CE6-453C-AB4C-B5ACC0BE52B5", "Admin,TenantManager,ShopManager",
             url: "/Settings_TemplateMessage", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        [AuditFilter("模板消息设置”", "2da1d542-9b2c-44fb-ab18-bdbcbd099fd5")]
        public override ActionResult Index()
        {
            return base.Index();
        }

        [HttpPost]
        public override ActionResult Index(Settings_TemplateMessage model)
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