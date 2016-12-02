// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : User_BindPhone.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.User
{
    /// <summary>
    ///     用户绑定手机号
    /// </summary>
    public class User_BindPhone : WeiChat_WeChatWithNoKeyBase
    {
        /// <summary>
        ///     OPENID
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     验证码
        /// </summary>
        public string CheckNumber { get; set; }
    }
}