// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_Type.cs
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
    ///     商品类别管理
    /// </summary>
    public class Product_Type : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     类别名称
        /// </summary>
        [MaxLength(50)]
        [Required]
        [Display(Name = "类别名称")]
        public string Name { get; set; }

        /// <summary>
        ///     类别备注
        /// </summary>
        [MaxLength(200)]
        [Display(Name = "类别备注")]
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }
    }

    /// <summary>
    ///     商品属性管理
    /// </summary>
    public class Product_Attribute : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     所属商品类别
        /// </summary>
        [Display(Name = "所属商品类别")]
        public Guid TypeId { get; set; }

        /// <summary>
        ///     属性名称
        /// </summary>
        [Display(Name = "属性名称")]
        public string Name { get; set; }

        /// <summary>
        ///     属性排序
        /// </summary>
        [Display(Name = "属性排序")]
        public int Sort { get; set; }

        [Display(Name = "是否选中")]
        public bool IsChecked { get; set; }

        [Display(Name = "价格")]
        public decimal Price { get; set; }
    }
}