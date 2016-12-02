// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : TenantBaseController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Interface;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Results;
using Magicodes.WeiChat.Infrastructure.Tenant;
using Magicodes.Logger;
using Magicodes.WeiChat.Infrastructure;

namespace Magicodes.Shop.Controllers
{
    /// <summary>
    ///     后台租户控制器基类
    ///     后台控制器请继承此类，继承之后会自动设置微信配置Key以及启用租户筛选器
    /// </summary>
    /// <typeparam name="TEntry"></typeparam>
    [Authorize]
    public class TenantBaseController<TEntry> : BaseController
        where TEntry : class, ITenantId, new()
    {
        public readonly LoggerBase log = Loggers.Current.DefaultLogger;
        /// <summary>
        ///     重写Action待执行事件，注册微信key以及启用租户筛选器
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TenantManager.Current.EnableTenantFilter(db, TenantId);
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        ///     执行完成事件
        ///     禁用租户筛选器
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            TenantManager.Current.DisableTenantFilter(db);
        }

        /// <summary>
        ///     序列化JSON（替换为JSON.NET序列化）
        /// </summary>
        /// <param name="data">对象</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="contentEncoding">内容编码</param>
        /// <param name="behavior">是否允许HttpGet请求</param>
        /// <returns>返回JsonResult</returns>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding,
            JsonRequestBehavior behavior)
        {
            return base.Json(data, contentType, contentEncoding, behavior);
        }

        /// <summary>
        ///     导出CSV
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult Csv(IEnumerable<TEntry> data)
        {
            return new CsvFileResult<TEntry>(data);
        }

        /// <summary>
        ///     导出Excel
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult Excel(IEnumerable<TEntry> data)
        {
            return new ExcelFileResult<TEntry>(data);
        }
    }
}