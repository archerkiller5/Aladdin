// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : CustomViewModel.cs
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
    public class CustomViewModel
    {
        /// <summary>
        ///     客服账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        ///     客服昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///     客服工号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     客服头像
        /// </summary>
        public string HeadImgUrl { get; set; }
    }

    public class AddCustomViewModel
    {
        [Required]
        [MaxLength(12)]
        [Display(Name = "客服账号")]
        public string Account { get; set; }

        [Required]
        [MaxLength(12)]
        [Display(Name = "客服昵称")]
        public string NickName { get; set; }


        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码不匹配，请重新输入！")]
        public string ConfirmPassword { get; set; }
    }

    public class RemoveCustomViewModel
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "客服账号")]
        public string Account { get; set; }
    }
}