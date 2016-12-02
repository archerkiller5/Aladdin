// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IInitializer.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Magicodes.Logger;
using Magicodes.WeiChat.Data;

namespace Magicodes.WeiChat.Infrastructure.Initialization
{
    /// <summary>
    ///     初始化接口
    /// </summary>
    public interface IInitializer
    {
        InitializerLevels Level { get; }

        /// <summary>
        ///     日志记录
        /// </summary>
        LoggerBase Logger { get; set; }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        AppDbContext Context { get; set; }

        /// <summary>
        ///     启动时执行初始化
        /// </summary>
        void Initialize();
    }
}