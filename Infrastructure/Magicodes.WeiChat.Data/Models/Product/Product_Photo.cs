// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_Photo.cs
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
    ///     商品图片管理
    /// </summary>
    public class Product_Photo
    {
        /// <summary>
        ///     关联商品ID
        /// </summary>
        [Display(Name = "关联商品ID")]
        public Guid ProductId { get; set; }

        /// <summary>
        ///     产品图片名称
        /// </summary>
        [MaxLength(100)]
        [Required]
        [Display(Name = "产品图片名称")]
        public string PhotoName { get; set; }

        /// <summary>
        ///     产品图片路径
        /// </summary>
        [Display(Name = "产品图片路径")]
        public string PhotoUrl { get; set; }

        /// <summary>
        ///     是否默认的显示的图片
        /// </summary>
        [Display(Name = "是否默认")]
        public bool IsDefault { get; set; }
    }
}