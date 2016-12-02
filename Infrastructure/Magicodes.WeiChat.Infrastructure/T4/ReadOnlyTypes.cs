// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ReadOnlyTypes.cs
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
    ///     只读类型
    /// </summary>
    public enum ReadOnlyTypes
    {
        /// <summary>
        ///     所有字段均只读
        /// </summary>
        All = 0,

        /// <summary>
        ///     添加时只读
        /// </summary>
        Add = 1,

        /// <summary>
        ///     编辑时只读
        /// </summary>
        Edit = 2
    }
}