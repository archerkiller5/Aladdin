// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Shop_Info.cs
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

namespace Magicodes.WeiChat.Data.Models.Shop
{
    /// <summary>
    ///     店铺基本信息
    /// </summary>
    public class Shop_Info : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     店铺名称
        /// </summary>
        [MaxLength(200)]
        [Required]
        [Display(Name = "店铺名称")]
        public string Name { get; set; }

        /// <summary>
        ///     店铺LOGO图片路径
        /// </summary>
        [Display(Name = "店铺LOGO")]
        public Guid Logo { get; set; }

        /// <summary>
        ///     店铺联系方式
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "联系电话")]
        public string Contact { get; set; }

        /// <summary>
        ///     店铺介绍
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "店铺介绍")]
        [DataType(DataType.MultilineText)]
        public string Describe { get; set; }

        /// <summary>
        ///     店铺主题
        /// </summary>
        [Display(Name = "店铺主题")]
        public ShopTheme Theme { get; set; }
    }


    /// <summary>
    ///     店铺主题
    /// </summary>
    public enum ShopTheme
    {
        [Display(Name = "默认")] DEFAULT = 0,
        [Display(Name = "红色")] RED = 1,
        [Display(Name = "蓝色")] BLUE = 2,
        [Display(Name = "黄认")] YELLOW = 3
    }
}