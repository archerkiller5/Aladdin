// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_Tag.cs
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

namespace Magicodes.WeiChat.Data.Models.Product
{
    /// <summary>
    ///     商品标签管理类
    /// </summary>
    public class Product_Tag : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     商品标签名称
        /// </summary>
        [MaxLength(200)]
        [Required]
        [Display(Name = "标签名称")]
        public string Name { get; set; }

        [Display(Name = "是否系统标签")]
        public bool IsSystem { get; set; }

        [Display(Name = "是否选中")]
        public bool IsChecked { get; set; }
    }
}