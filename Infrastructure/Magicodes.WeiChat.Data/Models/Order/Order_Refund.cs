// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_Refund.cs
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
    ///     订单退换货
    /// </summary>
    public class Order_Refund : WeiChat_WeChatBase<Guid>
    {
        /// <summary>
        ///     订单ID
        /// </summary>
        [Display(Name = "订单ID")]
        public Guid OrderID { get; set; }

        /// <summary>
        ///     订单编号
        /// </summary>
        [Display(Name = "订单编号")]
        [MaxLength(32)]
        public string OrderCode { get; set; }

        /// <summary>
        ///     退换货编号
        /// </summary>
        [Display(Name = "退换货编号")]
        [MaxLength(32)]
        public string Code { get; set; }

        /// <summary>
        ///     类型（1：退货退款；2：换货；）
        /// </summary>
        [Display(Name = "类型")]
        public EnumOrderRefundType Type { get; set; }

        /// <summary>
        ///     退款金额
        /// </summary>
        [Display(Name = "退款金额")]
        public decimal Amount { get; set; }

        /// <summary>
        ///     产品数量
        /// </summary>
        [Display(Name = "产品数量")]
        public int Quantity { get; set; }

        /// <summary>
        ///     退换货原因
        /// </summary>
        [Display(Name = "退换货原因")]
        [MaxLength(512)]
        public string Reason { get; set; }

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
        [MaxLength(16)]
        public string ShippingCode { get; set; }

        /// <summary>
        ///     收货人姓名
        /// </summary>
        [Display(Name = "收货人姓名")]
        [MaxLength(16)]
        public string Consignee { get; set; }

        /// <summary>
        ///     手机
        /// </summary>
        [Display(Name = "手机")]
        [MaxLength(11)]
        [DataType(DataType.PhoneNumber)]
        public string Mobile { get; set; }

        /// <summary>
        ///     买家收货地址
        /// </summary>
        [Display(Name = "收货地址")]
        [MaxLength(128)]
        public string Address { get; set; }

        /// <summary>
        ///     退换货状态(1:换货中;2:换货成功;3:买家已经申请退款，等待卖家同意;4:卖家已经同意退款，等待买家退货;5:退款中;6:买家已经退货，等待卖家确认收货;7:退款成功;8:卖家拒绝退款;9:退款关闭;)
        /// </summary>
        [Display(Name = "退换货状态")]
        public EnumOrderRefundState State { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(512)]
        public string Remark { get; set; }
    }

    #region 退换货类型

    /// <summary>
    ///     退换货类型 : byte
    ///     <remarks>(1：退货退款；2：换货；)</remarks>
    /// </summary>
    public enum EnumOrderRefundType : byte
    {
        /// <summary>
        ///     退货退款 (1)
        /// </summary>
        [Display(Name = "退货退款")] ReturnGoods = 1,

        /// <summary>
        ///     换货 (2)
        /// </summary>
        [Display(Name = "换货")] Return = 2
    }

    #endregion

    #region 退换货状态

    /// <summary>
    ///     退换货状态 : byte
    ///     <remarks>(1:换货中;2:换货成功;3:买家已经申请退款，等待卖家同意;4:卖家已经同意退款，等待买家退货;5:退款中;6:买家已经退货，等待卖家确认收货;7:退款成功;8:卖家拒绝退款;9:退款关闭;)</remarks>
    /// </summary>
    public enum EnumOrderRefundState : byte
    {
        /// <summary>
        ///     换货中 (1)
        /// </summary>
        [Display(Name = "换货中")] Replace = 1,

        /// <summary>
        ///     换货成功 (2)
        /// </summary>
        [Display(Name = "换货成功")] ReplaceSuccess = 2,

        /// <summary>
        ///     买家已经申请退款，等待卖家同意(3)
        /// </summary>
        [Display(Name = "买家已经申请退款，等待卖家同意")] WaitSellerAgree = 3,

        /// <summary>
        ///     卖家已经同意退款，等待买家退货(4)
        /// </summary>
        [Display(Name = "卖家已经同意退款，等待买家退货")] WaitBuyerReturnGoods = 4,

        /// <summary>
        ///     退款中 (5)
        /// </summary>
        [Display(Name = "退款中")] Refund = 5,

        /// <summary>
        ///     买家已经退货，等待卖家确认收货 (6)
        /// </summary>
        [Display(Name = "买家已经退货，等待卖家确认收货")] WaitSellerConfirmGoods = 6,

        /// <summary>
        ///     退款成功 (7)
        /// </summary>
        [Display(Name = "退款成功")] RefundSuccess = 7,

        /// <summary>
        ///     卖家拒绝退款 (8)
        /// </summary>
        [Display(Name = "卖家拒绝退款")] SellerRefuseBuyer = 8,

        /// <summary>
        ///     退款关闭 (9)
        /// </summary>
        [Display(Name = "退款关闭")] Closed = 9
    }

    #endregion
}