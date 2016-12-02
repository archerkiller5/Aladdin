// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ApiStopwatchAttribute.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Magicodes.Logger;

namespace Magicodes.WeiChat.Infrastructure.WebApiExtension.Filters
{
    public class ApiStopwatchAttribute : ActionFilterAttribute
    {
        private readonly LoggerBase _logger = Loggers.Current.DefaultLogger;
        private Stopwatch currentStopwatch;

        public ApiStopwatchAttribute(int warnThreshold)
        {
            WarnThreshold = warnThreshold;
        }

        public int WarnThreshold { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            currentStopwatch = Stopwatch.StartNew();
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            currentStopwatch.Stop();
            var url = actionExecutedContext.Request.RequestUri.PathAndQuery;
            var log = string.Format("apiController:{0},action:{1},execution time:{2}ms{3}Url:{4}",
                actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
                currentStopwatch.ElapsedMilliseconds,
                Environment.NewLine,
                url
            );
            _logger.Log(LoggerLevels.Info, log);
            if (currentStopwatch.ElapsedMilliseconds > WarnThreshold)
                _logger.Log(LoggerLevels.Warn, log);
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}