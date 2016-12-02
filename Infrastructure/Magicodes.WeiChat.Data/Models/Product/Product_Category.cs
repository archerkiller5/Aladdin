// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_Category.cs
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
    ///     商品类目
    /// </summary>
    public class Product_Category : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     父级类目
        /// </summary>
        [Display(Name = "父级类目")]
        public Guid? ParentId { get; set; }

        /// <summary>
        ///     商品类目名称
        /// </summary>
        [MaxLength(200)]
        [Required]
        [Display(Name = "类目名称")]
        public string Name { get; set; }

        /// <summary>
        ///     商品类目Code
        /// </summary>
        [Display(Name = "类目编码")]
        public string Code { get; set; }

        /// <summary>
        ///     商品类目LOGO图片路径
        /// </summary>
        [Display(Name = "类目LOGO")]
        public string Logo { get; set; }

        /// <summary>
        ///     商品类目排序
        /// </summary>
        [Display(Name = "类目排序")]
        public int Sort { get; set; }

        /// <summary>
        ///     商品类目是否显示
        /// </summary>
        [Display(Name = "是否显示")]
        public bool IsDisplay { get; set; }
    }
}