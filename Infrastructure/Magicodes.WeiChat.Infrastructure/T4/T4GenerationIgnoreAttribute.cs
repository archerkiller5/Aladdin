// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : T4GenerationIgnoreAttribute.cs
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
//        filename :T4GenerationIgnore
//        description :
//
//        created by 雪雁 at  2014/10/27 21:22:07
//        http://www.magicodes.net
//
//======================================================================

namespace Magicodes.WeiChat.Infrastructure.T4
{
    /// <summary>
    ///     T4生成忽略【类或属性】
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class T4GenerationIgnoreAttribute : Attribute
    {
        /// <summary>
        ///     使用默认的构造函数表示此类或此属性在生成时忽略
        /// </summary>
        public T4GenerationIgnoreAttribute()
        {
            IgnorePart = IgnoreParts.All;
        }

        /// <summary>
        /// </summary>
        /// <param name="ignorePart"></param>
        public T4GenerationIgnoreAttribute(IgnoreParts ignorePart)
        {
            IgnorePart = ignorePart;
        }

        public IgnoreParts IgnorePart { get; set; }
    }
}