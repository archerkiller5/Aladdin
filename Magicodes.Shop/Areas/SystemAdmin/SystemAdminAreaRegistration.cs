// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SystemAdminAreaRegistration.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:04
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;

namespace Magicodes.Shop.Areas.SystemAdmin
{
    public class SystemAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "SystemAdmin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SystemAdmin_default",
                "SystemAdmin/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}