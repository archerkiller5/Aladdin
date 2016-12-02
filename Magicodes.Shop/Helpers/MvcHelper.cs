// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MvcHelper.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;
using System.Web.Routing;
using Magicodes.WeiChat.Infrastructure;

namespace Magicodes.Shop.Helpers
{
    /// <summary>
    ///     MVC辅助类
    /// </summary>
    public static class MvcHelper
    {
        /// <summary>
        ///     生成租户Url
        /// </summary>
        /// <param name="Url">ASP.NET MVC 生成 URL的辅助对象</param>
        /// <param name="actionName">操作方法的名称</param>
        /// <param name="controllerName">控制器的名称</param>
        /// <returns>租户Url</returns>
        public static string TenantAction(this UrlHelper Url, string actionName, string controllerName = null)
        {
            if ((actionName != null) && (controllerName == null))
                return Url.Action(actionName, new {WeiChatApplicationContext.Current.TenantId});
            if ((actionName != null) && (controllerName != null))
                return Url.Action(actionName, controllerName, new {WeiChatApplicationContext.Current.TenantId});
            return null;
        }

        /// <summary>
        ///     生成租户Url
        /// </summary>
        /// <param name="Url">ASP.NET MVC 生成 URL的辅助对象</param>
        /// <param name="actionName">操作方法的名称</param>
        /// <param name="controllerName">控制器的名称</param>
        /// <param name="values">一个包含路由参数的对象。通过检查对象的属性，利用反射检索参数。该对象通常是使用对象初始值设定项语法创建的</param>
        /// <returns>租户Url</returns>
        public static string TenantAction(this UrlHelper Url, string actionName, string controllerName = null,
            object values = null)
        {
            return Url.TenantAction(actionName, controllerName, new RouteValueDictionary(values));
        }

        /// <summary>
        ///     生成租户Url
        /// </summary>
        /// <param name="Url">ASP.NET MVC 生成 URL的辅助对象</param>
        /// <param name="actionName">操作方法的名称。</param>
        /// <param name="controllerName">控制器的名称</param>
        /// <param name="dic">一个包含路由参数的对象</param>
        /// <returns>租户Url</returns>
        public static string TenantAction(this UrlHelper Url, string actionName, string controllerName = null,
            RouteValueDictionary dic = null)
        {
            if (dic == null)
            {
                dic = new RouteValueDictionary
                {
                    {"TenantId", WeiChatApplicationContext.Current.TenantId}
                };
            }
            else
            {
                if (!dic.ContainsKey("TenantId"))
                    dic["TenantId"] = WeiChatApplicationContext.Current.TenantId;
            }
            if ((actionName != null) && (controllerName == null))
                return Url.Action(actionName, dic);
            if ((actionName != null) && (controllerName != null))
                return Url.Action(actionName, controllerName, dic);
            return null;
        }
    }
}