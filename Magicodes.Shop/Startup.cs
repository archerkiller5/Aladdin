// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Startup.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:46
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Configuration;
using Magicodes.Shop;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Magicodes.Logger.NLog;
using Microsoft.AspNet.SignalR.Hubs;
using Hangfire;

[assembly: OwinStartup(typeof(Startup))]

namespace Magicodes.Shop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            var logger = new NLogLogger("Startup");
            ConfigureAuth(app);
            //判断是否启用SignalR
            if ((ConfigurationManager.AppSettings["EnableSignalR"] != null) &&
                (ConfigurationManager.AppSettings["EnableSignalR"].ToLower() == "true"))
            {
                GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
                app.MapSignalR();
            }
            
            logger.Log(Logger.LoggerLevels.Debug, "OwinStartup 启动完成");
        }

    }
    public class ErrorHandlingPipelineModule : HubPipelineModule
    {
        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            var logger = new NLogLogger("SignalR");
            logger.Log(Logger.LoggerLevels.Error, "=> Exception " + exceptionContext.Error.Message);
            logger.Log(Logger.LoggerLevels.Error, exceptionContext.Error.ToString());
            if (exceptionContext.Error.InnerException != null)
            {
                logger.Log(Logger.LoggerLevels.Error, "=> Inner Exception " + exceptionContext.Error.InnerException.Message);
            }
            base.OnIncomingError(exceptionContext, invokerContext);

        }
    }
}