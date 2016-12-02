// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WebApiConfig.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Http;
using Magicodes.WeiChat.Infrastructure.WebApiExtension.Filters;
using Magicodes.WebApi.ExceptionFilter;
using Magicodes.Logger.NLog;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Security;
using System.Data.Entity.Validation;
using System.Text;
using System.Linq;
using Magicodes.Logger;
using System;

namespace Magicodes.Shop
{
    public static class WebApiConfig
    {
        internal static readonly string UrlPrefixRelative = "~/api/";

        public static void Register(HttpConfiguration config)
        {
            RegisterMagicodesFilterConfig();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new WebApiExceptionFilter());
        }

        public static void RegisterMagicodesFilterConfig()
        {
            //配置WebApi筛选器
            WebApiExceptionFilterBuilder.Create()
                //添加日志记录器
                .WithLogger(new NLogLogger("WebApiExceptionFilter"))
                .Register<KeyNotFoundException>(HttpStatusCode.NotFound)
                .Register<SecurityException>(HttpStatusCode.Forbidden)
                .WithDefaultHandler((exception, request, logger) =>
                {
                    var response = request.CreateResponse<string>(HttpStatusCode.InternalServerError, "出现意外错误，请刷新重试或者稍后联系管理员！");
                    response.ReasonPhrase = "出现意外错误，请刷新重试或者稍后联系管理员！";
                    return response;
                })
                .Register<DbEntityValidationException>(
                  (exception, request, logger) =>
                  {
                      var sb = new StringBuilder();
                      var validationException = exception as DbEntityValidationException;
                      sb.AppendLine("实体验证错误。错误数：" + validationException.EntityValidationErrors.Count() + " ");
                      foreach (var validationResult in validationException.EntityValidationErrors)
                          foreach (var item in validationResult.ValidationErrors)
                              sb.AppendFormat("（{0}:{1}）", item.PropertyName, item.ErrorMessage);
                      logger.Log(LoggerLevels.Error, sb.ToString());
                      return request.CreateResponse(HttpStatusCode.BadRequest, "验证错误，请检查输入项！");
                  }
                )
                .Build();
        }
    }
}