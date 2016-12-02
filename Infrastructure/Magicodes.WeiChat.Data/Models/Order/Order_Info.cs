// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_Info.cs
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
    ///     订单信息
    /// </summary>
    public class Order_Info : WeiChat_WeChatBase<Guid>
    {
        /// <summary>
        ///     订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        [MaxLength(32)]
        public string Code { get; set; }

        /// <summary>
        ///     订单价格
        /// </summary>
        [Display(Name = "订单价格")]
        public decimal TotalPrice { get; set; }

        /// <summary>
        ///     支付方式（1:微信）
        /// </summary>
        [Display(Name = "支付方式")]
        public EnumThirdPayType ThirdPayType { get; set; }

        /// <summary>
        ///     成交时间
        /// </summary>
        [Display(Name = "成交时间")]
        public DateTime? DealOn { get; set; }

        /// <summary>
        ///     付款时间
        /// </summary>
        [Display(Name = "付款时间")]
        public DateTime? PaymentOn { get; set; }

        /// <summary>
        ///     发货时间
        /// </summary>
        [Display(Name = "发货时间")]
        public DateTime? ShippingOn { get; set; }

        /// <summary>
        ///     收货时间
        /// </summary>
        [Display(Name = "收货时间")]
        public DateTime? ReceiptOn { get; set; }

        /// <summary>
        ///     更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        ///     运费
        /// </summary>
        [Display(Name = "运费")]
        public decimal Shipping { get; set; }

        /// <summary>
        ///     买家留言
        /// </summary>
        [Display(Name = "买家留言")]
        [MaxLength(128)]
        public string Leave { get; set; }

        /// <summary>
        ///     拒绝原因
        /// </summary>
        [Display(Name = "拒绝原因")]
        [MaxLength(128)]
        public string RejectReason { get; set; }

        /// <summary>
        ///     订单状态(1:待付款、2:待发货、3:待收货、4:交易完成、5:已关闭、6:未付款删除、7:已付款删除、8:退货/退款)
        /// </summary>
        public EnumOrderStatus State { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(512)]
        public string Remark { get; set; }

        /// <summary>
        ///     退换货是否显示
        /// </summary>
        public bool IsRefund { get; set; }

        /// <summary>
        ///     买家用户信息
        /// </summary>
        public virtual WeiChat_User User { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIpAddress { get; set; }
    }

    #region 支付方式

    /// <summary>
    ///     支付方式 : byte
    ///     <remarks>(1:微信)</remarks>
    /// </summary>
    public enum EnumThirdPayType : byte
    {
        /// <summary>
        ///     微信 (1)
        /// </summary>
        [Display(Name = "微信")] WX = 1
    }

    #endregion

    #region 订单状态

    /// <summary>
    ///     订单状态 : byte
    ///     <remarks>(1:待付款、2:待发货、3:待收货、4:交易完成、5:已关闭、6:未付款删除、7:已付款删除、8:退货/退款)</remarks>
    /// </summary>
    public enum EnumOrderStatus : byte
    {
        /// <summary>
        ///     查询时使用
        /// </summary>
        [Display(Name = "所有订单")] AllOrder = 0,

        /// <summary>
        ///     待付款 (1)
        /// </summary>
        [Display(Name = "待付款")] Obligation = 1,

        /// <summary>
        ///     待发货 (2)
        /// </summary>
        [Display(Name = "待发货")] Overhang = 2,

        /// <summary>
        ///     待收货 (3)
        /// </summary>
        [Display(Name = "待收货")] WaitReceiving = 3,

        /// <summary>
        ///     交易完成 (4)
        /// </summary>
        [Display(Name = "交易完成")] Success = 4,

        /// <summary>
        ///     已关闭 (5)
        /// </summary>
        [Display(Name = "已关闭")] Closed = 5,

        /// <summary>
        ///     未付款删除 (6)
        /// </summary>
        [Display(Name = "未付款删除")] UnpaidDelete = 6,

        /// <summary>
        ///     已付款删除 (7)
        /// </summary>
        [Display(Name = "已付款删除")] PaidDelete = 7,

        /// <summary>
        ///     退货/退款 (8)
        /// </summary>
        [Display(Name = "退货/退款")] ReturnedGoods = 8
    }

    #endregion
}