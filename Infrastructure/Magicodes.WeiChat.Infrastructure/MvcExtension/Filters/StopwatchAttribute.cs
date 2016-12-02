// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StopwatchAttribute.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Diagnostics;
using System.Web.Mvc;
using Magicodes.Logger;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.Filters
{
    public class StopwatchAttribute : ActionFilterAttribute
    {
        private readonly LoggerBase _logger = Loggers.Current.DefaultLogger;
            //LogManager.GetLogger("StopwatchAttribute");

        private Stopwatch currentStopwatch;

        public StopwatchAttribute(int warnThreshold)
        {
            WarnThreshold = warnThreshold;
        }

        public int WarnThreshold { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            currentStopwatch = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            currentStopwatch.Stop();

            var log = string.Format("controller:{0},action:{1},execution time:{2}ms",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName, currentStopwatch.ElapsedMilliseconds);
            _logger.Log(LoggerLevels.Info, log);
            if (currentStopwatch.ElapsedMilliseconds > WarnThreshold)
                _logger.Log(LoggerLevels.Warn, log);
        }
    }
}