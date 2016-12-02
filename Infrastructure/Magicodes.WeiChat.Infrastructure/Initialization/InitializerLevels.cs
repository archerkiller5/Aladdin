// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : InitializerLevels.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.WeiChat.Infrastructure.Initialization
{
    /// <summary>
    ///     初始化优先级别
    /// </summary>
    public enum InitializerLevels
    {
        /// <summary>
        ///     最低（默认值）
        /// </summary>
        Lowest = 0,

        /// <summary>
        ///     低（一般推荐此值）
        /// </summary>
        Low = 1,

        /// <summary>
        ///     中等
        /// </summary>
        Middle = 2,

        /// <summary>
        ///     高
        /// </summary>
        High = 3,

        /// <summary>
        ///     最高（除了系统先决任务，一般不推荐使用此级别
        /// </summary>
        Highest = 4
    }
}