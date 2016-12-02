// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : T4ODataGridAttribute.cs
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
//        filename :T4ODataGridAttribute
//        description :
//
//        created by 雪雁 at  2015/1/9 14:36:56
//        http://www.magicodes.net
//
//======================================================================

namespace Magicodes.WeiChat.Infrastructure.T4
{
    /// <summary>
    ///     ODataGrid生成特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class T4ODataGridAttribute : Attribute
    {
        public T4ODataGridAttribute()
        {
        }

        public T4ODataGridAttribute(string actionName)
        {
            ActionName = actionName;
        }

        public string ActionName { get; set; }
    }
}