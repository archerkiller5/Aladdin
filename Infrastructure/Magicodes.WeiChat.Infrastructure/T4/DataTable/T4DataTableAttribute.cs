// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : T4DataTableAttribute.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;

//======================================================================
//
//        Copyright (C) 2014-2016 Magicodes.NET团队    
//        All rights reserved
//
//        filename :T4DataTableAttribute
//        description :
//
//        created by 雪雁 at  2015/1/21 14:30:16
//        http://www.magicodes.net
//
//======================================================================

namespace Magicodes.WeiChat.Infrastructure.T4.DataTable
{
    /// <summary>
    ///     数据列表
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class T4DataTableAttribute : Attribute
    {
        /// <summary>
        ///     表格标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     表格描述
        /// </summary>
        public string Description { get; set; }
    }
}