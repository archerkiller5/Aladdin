// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : HMTLHelperExtensions.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web.Mvc;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Unity;

namespace Magicodes.Shop.Helpers
{
    public static class HMTLHelperExtensions
    {
        /// <summary>
        ///     是否为当前菜单
        /// </summary>
        /// <param name="html"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
        {
            var cssClass = "active";
            var currentAction = (string) html.ViewContext.RouteData.Values["action"];
            var currentController = (string) html.ViewContext.RouteData.Values["controller"];

            if (string.IsNullOrEmpty(controller))
                controller = currentController;

            if (string.IsNullOrEmpty(action))
                action = currentAction;

            return (controller == currentController) && (action == currentAction)
                ? cssClass
                : string.Empty;
        }

        /// <summary>
        ///     允许选择多个控制器
        /// </summary>
        /// <param name="html"></param>
        /// <param name="controllerStr"></param>
        /// <returns></returns>
        public static string IsSelectesControllers(this HtmlHelper html, string controllerStr)
        {
            var controllers = controllerStr.Split(',');
            foreach (var item in controllers)
                if (html.IsSelected(item) == "active")
                    return "active";
            return string.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            var currentAction = (string) html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static string IsSelectesUrl(this HtmlHelper html, string url)
        {
            return
                html.ViewContext.HttpContext.Request.Url.AbsoluteUri.IndexOf(url,
                    StringComparison.CurrentCultureIgnoreCase) == -1
                    ? ""
                    : "active";
        }

        /// <summary>
        ///     显示枚举
        /// </summary>
        /// <param name="html"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DisplayForEnum(this HtmlHelper html, Enum value)
        {
            return value.GetEnumMemberDisplayName();
        }

        /// <summary>
        ///     根据图片ID获取图片路径
        /// </summary>
        /// <param name="html"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static string DisplayImgForId(this HtmlHelper html, Guid Id)
        {
            using (var db = new AppDbContext())
            {
                var photo = db.Site_Photos.FirstOrDefault(p => (p.Id == Id) && (p.IsDeleted == false));
                return photo == null ? "" : photo.Url;
            }
        }
    }
}