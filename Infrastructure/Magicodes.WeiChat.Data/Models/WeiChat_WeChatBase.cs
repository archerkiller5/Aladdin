// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_WeChatBase.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.ComponentModel.DataAnnotations;
using Magicodes.WeiChat.Data.Models.Interface;

namespace Magicodes.WeiChat.Data.Models
{
    /// <summary>
    ///     微信基础模型基类（无主键）
    /// </summary>
    public abstract class WeiChat_WeChatWithNoKeyBase : ITenantId
    {
        public virtual string OpenId { get; set; }

        public int TenantId { get; set; }

        [Display(Name ="创建时间")]
        public virtual DateTime CreateTime { get; set; }


    }
    /// <summary>
    ///     微信基础模型基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class WeiChat_WeChatBase<TKey> : WeiChat_WeChatWithNoKeyBase
    {
        [Key]
        public virtual TKey Id { get; set; }
    }
}