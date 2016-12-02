// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AdminHomeController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:02
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models;

namespace Magicodes.Shop.Areas.SystemAdmin.Controllers
{
    [RoleMenuFilter("主页", "F49C8230-9706-4CDB-9465-8F77ED505A9D", "Admin", iconCls: "fa fa-th-large",
         tag: "System", orderNo: 1)]
    public class AdminHomeController : SystemAdminBase<WeiChat_App, int>
    {
        // GET: SystemAdmin/AdminHome
        [AuditFilter("系统主页", "82EA0F37-5C61-4B52-9ED5-B97D2ADE112F")]
        [RoleMenuFilter("系统主页", "82EA0F37-5C61-4B52-9ED5-B97D2ADE112F", "Admin", url: "/SystemAdmin/AdminHome",
             parentId: "F49C8230-9706-4CDB-9465-8F77ED505A9D", tag: "System")]
        public ActionResult Index()
        {
            return View();
        }
    }
}