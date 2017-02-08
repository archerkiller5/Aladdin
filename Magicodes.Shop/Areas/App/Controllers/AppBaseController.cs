// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppBaseController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Magicodes.Logger;
using Magicodes.Shop.Helpers;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.Logging;

namespace Magicodes.Shop.Areas.App.Controllers
{
    [AllowAnonymous]
    public class AppBaseController : Controller
    {
        internal const string TenantIdSessionName = "Magicodes.TenantId";
        private readonly Lazy<LoggerBase> _logger = new Lazy<LoggerBase>(() => Loggers.Current.ControllerLogger);
        protected AppDbContext db = new AppDbContext();
        /// <summary>
        ///     日志记录器
        /// </summary>
        public LoggerBase Logger => _logger.Value;

        /// <summary>
        ///     租户Id
        /// </summary>
        public int TenantId
        {
            get
            {
                //请求参数中的租户Id
                var reqTennantId = default(int);

                #region 获取请求参数中的租户Id

                if (!string.IsNullOrWhiteSpace(Request.QueryString["TenantId"]))
                    reqTennantId = Convert.ToInt32(Request.QueryString["TenantId"]);
                else
                    reqTennantId = Request.RequestContext.RouteData.Values["TenantId"] != null
                        ? Convert.ToInt32(Request.RequestContext.RouteData.Values["TenantId"])
                        : default(int);

                #endregion

                if (reqTennantId != default(int))
                {
                    HttpContext.Session[TenantIdSessionName] = reqTennantId;
                    return reqTennantId;
                }
                if (HttpContext.Session[TenantIdSessionName] != null)
                    return Convert.ToInt32(HttpContext.Session[TenantIdSessionName]);
                return default(int);
            }
        }

        /// <summary>
        ///     微信用户信息
        /// </summary>
        public WeiChat_User WeiChatUser
        {
            get { return WeiChatApplicationContext.Current.WeiChatUser; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //设置配置Key
            base.OnActionExecuting(filterContext);
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            //设置配置Key
            base.OnAuthentication(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            //记录异常信息
            Logger.LogException(filterContext.Exception);
            filterContext.Result = ErrorTip("出错啦！", "服务器出现错误，请稍后再试或联系客服！", okUrl: Url.TenantAction("Index", "Store"));
            filterContext.ExceptionHandled = true;
        }

        /// <summary>
        ///     操作成功页面
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="iconCls">图标，默认“aui-icon-check”</param>
        /// <param name="okText">确定按钮文本，默认“确定”</param>
        /// <param name="okUrl">确定按钮跳转链接</param>
        /// <param name="okIconcls">确定按钮图标，默认“aui-icon-roundcheck”</param>
        /// <param name="cancelText">取消按钮文本，默认“取消”</param>
        /// <param name="cancelUrl">取消按钮链接</param>
        /// <param name="canceIconcls">取消按钮图标，默认“aui-icon-roundclose”</param>
        /// <param name="titleCls">标题样式，默认“aui-text-success”</param>
        /// <returns>成功页面</returns>
        public ActionResult SuccessTip(string title = "成功标题", string message = "成功文本", string iconCls = "aui-icon-check",
            string okText = "确定", string okUrl = "", string okIconcls = "aui-icon-roundcheck", string cancelText = "取消",
            string cancelUrl = "", string canceIconcls = "aui-icon-roundclose", string titleCls = "aui-text-success")
        {
            ViewBag.Title = title;
            ViewBag.Message = message;
            ViewBag.IconCls = iconCls;
            ViewBag.TitleCls = titleCls;

            ViewBag.OkText = okText;
            ViewBag.OkUrl = okUrl;
            ViewBag.OkIconcls = okIconcls;

            ViewBag.CancelText = cancelText;
            ViewBag.CancelUrl = cancelUrl;
            ViewBag.CanceIconcls = canceIconcls;
            return View("~/Areas/App/Views/AppBase/Tip.cshtml");
        }

        /// <summary>
        ///     操作错误页面
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="iconCls">图标，默认“aui-icon-close”</param>
        /// <param name="okText">确定按钮文本，默认“确定”</param>
        /// <param name="okUrl">确定按钮跳转链接</param>
        /// <param name="okIconcls">确定按钮图标，默认“aui-icon-roundcheck”</param>
        /// <param name="cancelText">取消按钮文本，默认“取消”</param>
        /// <param name="cancelUrl">取消按钮链接</param>
        /// <param name="canceIconcls">取消按钮图标，默认“aui-icon-roundclose”</param>
        /// <param name="titleCls">标题样式，默认“aui-text-danger”</param>
        /// <returns>错误页面</returns>
        public ActionResult ErrorTip(string title = "错误标题", string message = "错误文本", string iconCls = "aui-icon-close",
            string okText = "确定", string okUrl = "", string okIconcls = "aui-icon-roundcheck", string cancelText = "取消",
            string cancelUrl = "", string canceIconcls = "aui-icon-roundclose", string titleCls = "aui-text-danger")
        {
            ViewBag.Title = title;
            ViewBag.Message = message;
            ViewBag.IconCls = iconCls;
            ViewBag.TitleCls = titleCls;

            ViewBag.OkText = okText;
            ViewBag.OkUrl = okUrl;
            ViewBag.OkIconcls = okIconcls;

            ViewBag.CancelText = cancelText;
            ViewBag.CancelUrl = cancelUrl;
            ViewBag.CanceIconcls = canceIconcls;
            return View("~/Areas/App/Views/AppBase/Tip.cshtml");
        }

        /// <summary>
        ///     操作警告页面
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">消息内容</param>
        /// <param name="iconCls">图标，默认“aui-icon-warn”</param>
        /// <param name="okText">确定按钮文本，默认“确定”</param>
        /// <param name="okUrl">确定按钮跳转链接</param>
        /// <param name="okIconcls">确定按钮图标，默认“aui-icon-roundcheck”</param>
        /// <param name="cancelText">取消按钮文本，默认“取消”</param>
        /// <param name="cancelUrl">取消按钮链接</param>
        /// <param name="canceIconcls">取消按钮图标，默认“aui-icon-roundclose”</param>
        /// <param name="titleCls">标题样式，默认“aui-text-warning”</param>
        /// <returns>警告页面</returns>
        public ActionResult WarnTip(string title = "警告标题", string message = "警告文本", string iconCls = "aui-icon-warn",
            string okText = "确定", string okUrl = "", string okIconcls = "aui-icon-roundcheck", string cancelText = "取消",
            string cancelUrl = "", string canceIconcls = "aui-icon-roundclose", string titleCls = "aui-text-warning")
        {
            ViewBag.Title = title;
            ViewBag.Message = message;
            ViewBag.IconCls = iconCls;
            ViewBag.TitleCls = titleCls;

            ViewBag.OkText = okText;
            ViewBag.OkUrl = okUrl;
            ViewBag.OkIconcls = okIconcls;

            ViewBag.CancelText = cancelText;
            ViewBag.CancelUrl = cancelUrl;
            ViewBag.CanceIconcls = canceIconcls;
            return View("~/Areas/App/Views/AppBase/Tip.cshtml");
        }

        /// <summary>
        ///     图片提示页
        /// </summary>
        /// <param name="imageUrl">图片地址</param>
        /// <param name="okText">确定按钮文本</param>
        /// <param name="okUrl">确定按钮链接</param>
        /// <param name="okIconcls">确定按钮图标</param>
        /// <returns>图片页</returns>
        public ActionResult ImageTip(string imageUrl, string okText = "确定", string okUrl = "",
            string okIconcls = "aui-icon-roundcheck")
        {
            ViewBag.ImageUrl = imageUrl;
            ViewBag.OkText = okText;
            ViewBag.OkUrl = okUrl;
            ViewBag.OkIconcls = okIconcls;
            return View("~/Areas/App/Views/AppBase/ImageTip.cshtml");
        }
    }
}