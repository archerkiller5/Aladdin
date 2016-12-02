// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : LoggerConfig.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Magicodes.Logger.NLog;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.Initialization;

namespace Magicodes.Shop
{
    /// <summary>
    ///     日志管理器配置
    /// </summary>
    public class LoggerConfig
    {
        public static void Register()
        {
            //初始化管理器日志记录器
            InitializerManager.Current.Logger = new NLogLogger("InitializerManagerLogger");
            //默认的记录器
            Loggers.Current.DefaultLogger = new NLogLogger("DefaultLogger");
            //控制器日志记录
            Loggers.Current.ControllerLogger = new NLogLogger("ControllerLogger");
        }
    }
}