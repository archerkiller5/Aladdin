// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IgnoreParts.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.WeiChat.Infrastructure.T4
{
    /// <summary>
    ///     忽略部分
    /// </summary>
    public enum IgnoreParts
    {
        /// <summary>
        ///     所有
        /// </summary>
        All = 0,

        /// <summary>
        ///     表单
        /// </summary>
        Form = 1,

        /// <summary>
        ///     表格
        /// </summary>
        Grid = 2
    }
}