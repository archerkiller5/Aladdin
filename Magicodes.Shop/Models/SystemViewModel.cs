// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SystemViewModel.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:16
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.Shop.Models
{
    public class CreateAdminUserViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "确认密码不匹配！")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "手机号码")]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
    }
}