// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : User_Address.cs
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

namespace Magicodes.WeiChat.Data.Models.User
{
    /// <summary>
    ///     地址库
    /// </summary>
    public class User_Address : WeiChat_WeChatBase<Guid>
    {
        [Display(Name = "省")]
        public string Province { get; set; }

        [Display(Name = "市")]
        public string City { get; set; }

        [Display(Name = "区")]
        public string District { get; set; }

        [StringLength(100, MinimumLength = 4, ErrorMessage = "地址必须明确范围,请重新输入!")]
        [Display(Name = "详细地址")]
        public string Street { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "输入收货人姓名为空或长度为空!")]
        [Display(Name = "收货人姓名")]
        public string Name { get; set; }

        [RegularExpression(
             @"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)",
             ErrorMessage = "手机号格式不正确")]
        [Display(Name = "手机号码")]
        public string PhoneNumber { get; set; }

        [Display(Name = "邮编")]
        public string PostCode { get; set; }

        [Display(Name = "是否为默认地址")]
        public bool IsDefault { get; set; }
    }
}