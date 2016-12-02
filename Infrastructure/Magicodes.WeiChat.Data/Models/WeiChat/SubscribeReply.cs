// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SubscribeReply.cs
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

namespace Magicodes.WeiChat.Data.Models.WeiChat
{
    /// <summary>
    ///     关注时回复
    /// </summary>
    public class WeiChat_SubscribeReply : WeiChat_AdminUniqueTenantBase<Guid>
    {
        public WeiChat_SubscribeReply()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        ///     关键字类型
        /// </summary>
        [Display(Name = "类型")]
        public KeyWordContentTypes KeyWordContentType { get; set; }

        /// <summary>
        ///     内容Id
        /// </summary>
        [Display(Name = "内容")]
        public Guid ContentId { get; set; }
    }
}