// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Loggers.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Magicodes.Logger;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.Infrastructure
{
    /// <summary>
    ///     日志记录器
    /// </summary>
    public class Loggers : ThreadSafeLazyBaseSingleton<Loggers>
    {
        /// <summary>
        ///     默认的日志记录器
        /// </summary>
        public LoggerBase DefaultLogger { get; set; }

        /// <summary>
        ///     控制器日志记录器
        /// </summary>
        public LoggerBase ControllerLogger { get; set; }
    }
}