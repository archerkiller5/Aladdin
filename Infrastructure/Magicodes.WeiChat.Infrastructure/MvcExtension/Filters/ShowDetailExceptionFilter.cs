// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ShowDetailExceptionFilter.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.Filters
{
    public class ShowDetailExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var ex = filterContext.Exception;
            var sb = new StringBuilder();
            var context = filterContext.HttpContext;
            sb.AppendFormat("<h1>{0}</h1><br/>", ex.Message);
            sb.AppendFormat("<p>{0}</p><br/>", ex);
            if (ex.InnerException != null)
            {
                sb.Append("<p><h1>内部异常：</h1></p>");
                sb.AppendFormat("<p>{0}</p>", ex.InnerException);
            }

            if (ex is DbEntityValidationException)
            {
                sb.AppendFormat("<h1>{0}</h1><br/>", "实体验证错误");
                var validationException = ex as DbEntityValidationException;
                sb.AppendLine("<p>错误数：" + validationException.EntityValidationErrors.Count() + "</p>");
                foreach (var validationResult in validationException.EntityValidationErrors)
                    foreach (var item in validationResult.ValidationErrors)
                        sb.AppendFormat("<p>（{0}:{1}）</p>", item.PropertyName, item.ErrorMessage);
            }

            sb.Append("<p><h1>Url参数：</h1></p><table>");
            foreach (var key in context.Request.QueryString.AllKeys)
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", key, context.Request.QueryString[key]);
            sb.Append("</table>");
            sb.Append("<p><h1>Form参数：</h1></p><table>");
            foreach (var key in context.Request.Form.AllKeys)
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", key, context.Request.Form[key]);
            sb.Append("</table>");
            sb.Append("<p><h1>Route参数：</h1></p><table>");
            foreach (var item in context.Request.RequestContext.RouteData.Values)
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", item.Key, item.Value);
            sb.Append("</table>");

            sb.Append("<p><h1>Cookies：</h1></p><table>");
            foreach (var item in context.Request.Cookies.AllKeys)
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", item, context.Request.Cookies[item].Value);
            sb.Append("</table>");


            filterContext.ExceptionHandled = true;
            filterContext.Result = new ContentResult {Content = sb.ToString()};
        }
    }
}