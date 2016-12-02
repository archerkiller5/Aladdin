// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Global.asax.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:46
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Runtime.ExceptionServices;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Magicodes.Logger;
using Magicodes.Shop.App_Start;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.Initialization;

namespace Magicodes.Shop
{
    public class MvcApplication : HttpApplication
    {
        //Logger log = LogManager.GetCurrentClassLogger();
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;

        protected void Application_Start()
        {
            //记录托管异常
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            //注册默认的存储提供程序
            StorageProviderConfig.ConfigureStorageProvider();
            //注册WebAPI相关配置
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //注册路由
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //注册全局筛选器
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //注册脚本合并压缩配置
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //注册Magicodes筛选器配置，比如审计日志、角色菜单等
            FilterConfig.RegisterMagicodesFilter();
            //注册日志记录器
            LoggerConfig.Register();
            //设置站内通知
            Magicodes_Notify_Config.Builder();
            //注册SDK函数
            WeChatSDKConfig.RegisterSdkFuncs();
            //注册服务程序
            ServiceConfigs.Register();
            //执行所有的初始化方法
            InitializerManager.Current.StartAllInitializer();
            //配置任务管理器
            TaskManagerConfig.ConfigTaskManager();
            //初始化部分数据
            DataInitializator.Init();
            
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

        private bool IsWebApiRequest()
        {
            return
                HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(
                    WebApiConfig.UrlPrefixRelative);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //获取异常
            var lastError = Server.GetLastError().GetBaseException();
            string requestStr = null;
            if (HttpContext.Current != null)
            {
                var context = HttpContext.Current;
                requestStr = string.Format("URL:{1}{0}", Environment.NewLine, context.Request.Url);
            }
            if (string.IsNullOrEmpty(requestStr))
                log.Log(LoggerLevels.Error, lastError);
            else
                log.Log(LoggerLevels.Error, requestStr); //, lastError, requestStr);
        }

        private void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            //LogManager.GetCurrentClassLogger().Error(e.Exception, "托管代码错误");
            //if (e.Exception.InnerException != null)
            //{
            //    LogManager.GetCurrentClassLogger().Error(e.Exception.InnerException, "托管代码错误");
            //}
        }
    }
}