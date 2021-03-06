﻿// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : T4ReadOnlyFieldAttribute.cs
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
//        filename :T4ReadOnlyFieldAttribute
//        description :
//
//        created by 雪雁 at  2015/1/12 16:11:07
//        http://www.magicodes.net
//
//======================================================================

namespace Magicodes.WeiChat.Infrastructure.T4
{
    /// <summary>
    ///     T4字段生成只读特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class T4ReadOnlyFieldAttribute : Attribute
    {
        public T4ReadOnlyFieldAttribute()
        {
            ReadOnlyType = ReadOnlyTypes.Add;
        }

        public T4ReadOnlyFieldAttribute(ReadOnlyTypes readOnlyType)
        {
            ReadOnlyType = readOnlyType;
        }

        public ReadOnlyTypes ReadOnlyType { get; set; }
    }
}