// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_Logistics.cs
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
    ///     订单物流信息
    /// </summary>
    public class Order_Logistics : WeiChat_WeChatBase<Guid>
    {
        /// <summary>
        ///     订单ID
        /// </summary>
        [Display(Name = "订单ID")]
        public Guid OrderID { get; set; }

        /// <summary>
        ///     收货人姓名
        /// </summary>
        [Display(Name = "收货人姓名")]
        public string Consignee { get; set; }

        /// <summary>
        ///     省
        /// </summary>
        [Display(Name = "省")]
        public string Province { get; set; }

        /// <summary>
        ///     市
        /// </summary>
        [Display(Name = "市")]
        public string City { get; set; }

        /// <summary>
        ///     区
        /// </summary>
        [Display(Name = "区")]
        public string Area { get; set; }

        /// <summary>
        ///     街道
        /// </summary>
        [Display(Name = "街道")]
        [MaxLength(128)]
        public string Address { get; set; }

        /// <summary>
        ///     手机
        /// </summary>
        [Display(Name = "手机")]
        public string Mobile { get; set; }

        /// <summary>
        ///     物流公司
        /// </summary>
        [Display(Name = "物流公司")]
        [MaxLength(16)]
        public string Logistics { get; set; }

        /// <summary>
        ///     物流单号
        /// </summary>
        [Display(Name = "物流单号")]
        public string ShippingCode { get; set; }
    }
}