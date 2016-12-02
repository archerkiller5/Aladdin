// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_Info.cs
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
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicodes.WeiChat.Data.Models.Product
{
    /// <summary>
    ///     商品信息管理
    /// </summary>
    public class Product_Info : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     商品所属类目
        /// </summary>
        [Display(Name = "商品所属类目")]
        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Product_Category Category { get; set; }

        /// <summary>
        ///     商品名称
        /// </summary>
        [MaxLength(200)]
        [Required]
        [Display(Name = "商品名称")]
        public string Name { get; set; }

        /// <summary>
        ///     商品简介
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "商品简介")]
        [DataType(DataType.MultilineText)]
        public string Intro { get; set; }

        /// <summary>
        ///     商品描述
        /// </summary>
        [Display(Name = "商品描述")]
        [DataType(DataType.MultilineText)]
        public string Des { get; set; }

        /// <summary>
        ///     商品库存
        /// </summary>
        [Display(Name = "商品库存")]
        public int Number { get; set; }

        /// <summary>
        ///     浏览量
        /// </summary>
        [Required]
        [Display(Name = "浏览量")]
        public int VisitCount { get; set; }

        /// <summary>
        ///     出售数量
        /// </summary>
        [Required]
        [Display(Name = "销售量")]
        public int SellCount { get; set; }

        /// <summary>
        ///     商品价格
        /// </summary>
        [Display(Name = "商品价格")]
        public decimal Price { get; set; }

        /// <summary>
        ///     商品状态
        /// </summary>
        [Display(Name = "商品状态")]
        public ProductState State { get; set; }

        /// <summary>
        ///     所属商品类别
        /// </summary>
        [Display(Name = "商品属性类别")]
        public Guid TypeId { get; set; }

        /// <summary>
        /// 商品评论数
        /// </summary>
        [Display(Name="商品评论数")]
        public int CommentCount { get; set; }
    }

    /// <summary>
    ///     商品标签关联管理
    /// </summary>
    public class Product_ProductTag : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     关联商品ID
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        ///     关联商品TagID
        /// </summary>
        public Guid TagId { get; set; }
    }

    /// <summary>
    ///     商品属性关联管理
    /// </summary>
    public class Product_ProductAttribute : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     关联商品ID
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        ///     关联商品属性ID
        /// </summary>
        public Guid AttributeId { get; set; }

        /// <summary>
        ///     关联商品属性名称
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        ///     属性价格
        /// </summary>
        [Display(Name = "价格")]
        public decimal AttributePrice { get; set; }

        /// <summary>
        ///     属性排序
        /// </summary>
        [Display(Name = "排序")]
        public int AttributeSort { get; set; }
    }

    /// <summary>
    ///     商品状态
    /// </summary>
    public enum ProductState
    {
        [Display(Name = "在售")] OnSell = 0,
        [Display(Name = "下架")] SoldOut = 1
    }
}