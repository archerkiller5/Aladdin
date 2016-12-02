// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : T4SelectAttribute.cs
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
//        filename :T4SelectAttribute
//        description :
//
//        created by 雪雁 at  2015/1/15 14:34:59
//        http://www.magicodes.net
//
//======================================================================

namespace Magicodes.WeiChat.Infrastructure.T4
{
    /// <summary>
    ///     下拉列表生成特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class T4SelectAttribute : Attribute
    {
        public T4SelectAttribute()
        {
        }

        public T4SelectAttribute(string dataUrl, string displayField, string valueField)
        {
            DataUrl = dataUrl;
            DisplayField = displayField;
            ValueField = valueField;
        }

        public T4SelectAttribute(string dataUrl, string displayField, string valueField, string root)
        {
            DataUrl = dataUrl;
            DisplayField = displayField;
            ValueField = valueField;
            Root = root;
        }

        /// <summary>
        ///     JSONUrl
        /// </summary>
        public string DataUrl { get; set; }

        /// <summary>
        ///     显示字段名
        /// </summary>
        public string DisplayField { get; set; }

        /// <summary>
        ///     值字段名
        /// </summary>
        public string ValueField { get; set; }

        /// <summary>
        ///     根属性
        /// </summary>
        public string Root { get; set; }
    }
}