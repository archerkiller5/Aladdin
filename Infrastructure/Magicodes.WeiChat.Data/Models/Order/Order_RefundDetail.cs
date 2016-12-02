// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_RefundDetail.cs
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
    ///     订单退换货详情
    /// </summary>
    public class Order_RefundDetail : WeiChat_WeChatBase<Guid>
    {
        /// <summary>
        ///     订单退换货ID
        /// </summary>
        [Display(Name = "订单退换货ID")]
        public Guid OrderRefund { get; set; }

        /// <summary>
        ///     订单产品ID
        /// </summary>
        [Display(Name = "订单产品ID")]
        public Guid OrderDetail { get; set; }
    }
}