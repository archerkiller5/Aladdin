// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : T4OnlyVerificationAttribute.cs
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
//        filename :T4OnlyVerification
//        description :
//
//        created by 雪雁 at  2014/10/28 21:51:59
//        http://www.magicodes.net
//
//======================================================================

namespace Magicodes.WeiChat.Infrastructure.T4
{
    /// <summary>
    ///     T4生成唯一验证代码【属性】
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class T4OnlyVerificationAttribute : Attribute
    {
        /// <summary>
        ///     使用默认的构造函数表示此属性生成唯一验证代码
        /// </summary>
        public T4OnlyVerificationAttribute()
        {
        }

        /// <summary>
        ///     使用默认的构造函数表示此属性生成唯一验证代码
        /// </summary>
        public T4OnlyVerificationAttribute(bool ignoreDeletedData)
        {
            IgnoreDeletedData = ignoreDeletedData;
        }

        /// <summary>
        ///     忽略已删除数据
        /// </summary>
        public bool IgnoreDeletedData { get; set; }
    }
}