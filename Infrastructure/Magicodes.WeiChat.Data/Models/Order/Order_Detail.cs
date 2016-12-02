// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_Detail.cs
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

namespace Magicodes.WeiChat.Data.Models.Order
{
    /// <summary>
    ///     订单详情
    /// </summary>
    public class Order_Detail : WeiChat_WeChatBase<Guid>
    {
        /// <summary>
        ///     订单ID
        /// </summary>
        [Display(Name = "订单ID")]
        public Guid OrderID { get; set; }

        /// <summary>
        ///     产品名称
        /// </summary>
        [Display(Name = "产品名称")]
        [MaxLength(64)]
        public string ProductName { get; set; }

        /// <summary>
        ///     产品别名
        /// </summary>
        [Display(Name = "产品别名")]
        [MaxLength(64)]
        public string ProductAlias { get; set; }

        /// <summary>
        ///     产品主图片
        /// </summary>
        [Display(Name = "产品主图片")]
        [MaxLength(128)]
        public string ProductImage { get; set; }

        /// <summary>
        ///     产品规则名称1
        /// </summary>
        [Display(Name = "产品规则名称1")]
        [MaxLength(16)]
        public string Rule1 { get; set; }

        /// <summary>
        ///     产品规则1-值
        /// </summary>
        [Display(Name = "产品规则1-值")]
        [MaxLength(16)]
        public string Rule1Value { get; set; }

        /// <summary>
        ///     产品规则名称2
        /// </summary>
        [Display(Name = "产品规则名称2")]
        [MaxLength(16)]
        public string Rule2 { get; set; }

        /// <summary>
        ///     产品规则2-值
        /// </summary>
        [Display(Name = "产品规则2-值")]
        [MaxLength(16)]
        public string Rule2Value { get; set; }

        /// <summary>
        ///     产品ID
        /// </summary>
        [Display(Name = "产品ID")]
        public Guid ProductID { get; set; }

        /// <summary>
        ///     产品套餐ID
        /// </summary>
        [Display(Name = "产品套餐ID")]
        public Guid PackageID { get; set; }

        /// <summary>
        ///     单价
        /// </summary>
        [Display(Name = "单价")]
        public decimal Price { get; set; }

        /// <summary>
        ///     原价(市场价)
        /// </summary>
        [Display(Name = "原价(市场价)")]
        public decimal OriginalPrice { get; set; }

        /// <summary>
        ///     数量
        /// </summary>
        [Display(Name = "数量")]
        public int Quantity { get; set; }

        /// <summary>
        /// 评价id
        /// </summary>
        [Display(Name ="评价id")]
        public Guid? CommentId { get; set; }
    }
}