// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : UnityController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models;

namespace Magicodes.Shop.Controllers
{
    [Authorize]
    public class UnityController : TenantBaseController<WeiChat_App>
    {
        /// <summary>
        ///     获取系统管理员页面链接
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSystemAdminHomeHtml()
        {
            return
                Content(IsSystemIenant
                    ? string.Format("<a href='{0}'>系统管理员界面</a>",
                        Url.Action("Index", "AdminHome", new {area = "SystemAdmin"}))
                    : "");
        }
    }
}