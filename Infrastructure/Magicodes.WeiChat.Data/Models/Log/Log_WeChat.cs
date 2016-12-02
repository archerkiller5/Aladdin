// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Log_WeChat.cs
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
using Magicodes.WeiChat.Data.Models.Order;

namespace Magicodes.WeiChat.Data.Models.Log
{
    /// <summary>
    ///     积分日志
    /// </summary>
    public class Log_Point : WeiChat_WeChatBase<long>
    {
        [Display(Name = "积分")]
        /// <summary>
        /// 积分
        /// </summary>
        public int Point { get; set; }

        [Display(Name = "备注")]
        [MaxLength(50)]
        public string Remark { get; set; }
    }

    /// <summary>
    ///     会员访问日志
    /// </summary>
    public class Log_MemberAccess : WeiChat_WeChatBase<long>
    {
        [Display(Name = "请求路径")]
        [MaxLength(500)]
        public string RequestUrl { get; set; }

        [Display(Name = "客户端IP")]
        [MaxLength(20)]
        public string ClientIpAddress { get; set; }

        [MaxLength(500)]
        [Display(Name = "浏览器信息")]
        public string BrowserInfo { get; set; }

        [Display(Name = "执行时间")]
        public long ExecutionDuration { get; set; }
    }

    /// <summary>
    ///     订单支付日志
    /// </summary>
    public class Log_Order : WeiChat_WeChatBase<long>
    {
        /// <summary>
        ///     订单Id
        /// </summary>
        [Display(Name = "订单Id")]
        public Guid OrderId { get; set; }

        /// <summary>
        ///     订单金额
        /// </summary>
        [Display(Name = "订单金额")]
        [Range(0, 1000000)]
        public decimal Amount { get; set; }

        /// <summary>
        ///     支付方式（1:微信）
        /// </summary>
        [Display(Name = "支付类型")]
        public EnumThirdPayType PaymentType { get; set; }
    }

    /// <summary>
    ///     充值日志
    /// </summary>
    public class Log_Recharge : WeiChat_WeChatBase<long>
    {
        /// <summary>
        ///     金额
        /// </summary>
        [Display(Name = "金额")]
        [Range(0, 1000000)]
        public decimal Amount { get; set; }

        [Display(Name = "备注")]
        [MaxLength(50)]
        public string Remark { get; set; }
    }

    /// <summary>
    ///     提现日志
    /// </summary>
    public class Log_Withdraw : WeiChat_WeChatBase<long>
    {
        /// <summary>
        ///     提现金额
        /// </summary>
        [Display(Name = "提现金额")]
        [Range(0, 1000000)]
        public decimal WithdrawAmount { get; set; }

        /// <summary>
        ///     实付金额
        /// </summary>
        [Display(Name = "实付金额")]
        [Range(0, 1000000)]
        public decimal ActualAmount { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(50)]
        public string Remark { get; set; }

        /// <summary>
        ///     实际提现时间
        /// </summary>
        [Display(Name = "实际提现时间")]
        public DateTime? WithdrawTime { get; set; }

        /// <summary>
        ///     提现审批状态
        /// </summary>
        [Display(Name = "提现审批状态")]
        public WithdrawApprovalStatus Status { get; set; }

        /// <summary>
        ///     审批意见
        /// </summary>
        [Display(Name = "审批意见")]
        [MaxLength(500)]
        public string ApprovalComments { get; set; }

        /// <summary>
        ///     手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        [MaxLength(20)]
        public string Phone { get; set; }

        /// <summary>
        ///     真是姓名
        /// </summary>
        [Display(Name = "真实姓名")]
        [MaxLength(20)]
        public string TrueName { get; set; }
    }

    /// <summary>
    ///     提现审批状态
    /// </summary>
    public enum WithdrawApprovalStatus
    {
        [Display(Name = "待审批")] Pending = 0,
        [Display(Name = "审批通过")] Approved = 1,
        [Display(Name = "拒绝")] Reject = 2
    }

    /// <summary>
    ///     资金监控日志
    /// </summary>
    public class Log_FinancialMonitoring : WeiChat_WeChatBase<long>
    {
        /// <summary>
        ///     金额
        /// </summary>
        [Display(Name = "金额")]
        [Range(0, 1000000)]
        public decimal Amount { get; set; }

        [Display(Name = "备注")]
        [MaxLength(50)]
        public string Remark { get; set; }

        [Display(Name = "客户端IP")]
        [MaxLength(20)]
        public string ClientIpAddress { get; set; }

        /// <summary>
        ///     资金监控日志类型
        /// </summary>
        [Display(Name = "资金监控日志类型")]
        public FinancialMonitoringTypes Type { get; set; }

        /// <summary>
        ///     是处理成功
        /// </summary>
        [Display(Name = "是处理成功")]
        public bool IsSuccess { get; set; }

        /// <summary>
        ///     错误日志
        /// </summary>
        [Display(Name = "错误日志")]
        public string ErrorLog { get; set; }

        /// <summary>
        ///     支付接口日志
        /// </summary>
        [Display(Name = "支付接口日志")]
        public string PaymentInterfaceLog { get; set; }
    }

    /// <summary>
    ///     资金监控日志类型
    /// </summary>
    public enum FinancialMonitoringTypes
    {
        /// <summary>
        ///     订单支付
        /// </summary>
        [Display(Name = "订单支付")] OrderPay = 0,

        /// <summary>
        ///     充值
        /// </summary>
        [Display(Name = "充值")] Recharge = 1,

        /// <summary>
        ///     提现
        /// </summary>
        [Display(Name = "提现")] Withdraw = 2
    }

    public class Log_RedPacketSending : WeiChat_WeChatBase<long>
    {
        /// <summary>
        ///     活动名称
        /// </summary>
        [Display(Name = "活动名称")]
        public string ActName { get; set; }

        /// <summary>
        ///     总金额
        /// </summary>
        [Display(Name = "总金额")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        ///     红包个数
        /// </summary>
        [Display(Name = "红包个数")]
        public int TotalNum { get; set; }
    }
}