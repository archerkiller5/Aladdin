// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings_TemplateMessage.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.Settings
{
    /// <summary>
    ///     模板消息设置
    /// </summary>
    public class Settings_TemplateMessage : WeiChat_AdminUniqueTenantBase<int>
    {
        /// <summary>
        ///     是否启用积分提醒
        /// </summary>
        [Display(Name = "是否启用积分提醒")]
        public bool IsEnablePointsNotice { get; set; }

        /// <summary>
        ///     是否启用提现审核结果通知
        /// </summary>
        [Display(Name = "是否启用提现审核结果通知")]
        public bool IsEnableWithdrawAuditNotice { get; set; }

        /// <summary>
        ///     是否启用订单支付成功提醒
        /// </summary>
        [Display(Name = "是否启用订单支付成功提醒")]
        public bool IsEnableSuccessfulPaymentOrdersNotice { get; set; }

        /// <summary>
        ///     是否启用订单发货提醒
        /// </summary>
        [Display(Name = "是否启用订单发货提醒")]
        public bool IsEnableOrdersShippingNotice { get; set; }

        /// <summary>
        ///     是否启用新订单通知
        /// </summary>
        [Display(Name = "是否启用新订单通知")]
        public bool IsEnableTheNewOrderNotice { get; set; }
    }

    /// <summary>
    ///     积分设置
    /// </summary>
    public class Settings_Point : WeiChat_AdminUniqueTenantBase<int>
    {
        /// <summary>
        ///     是否启用签到送积分
        /// </summary>
        [Display(Name = "是否启用签到送积分")]
        public bool IsEnableSignPoints { get; set; }

        /// <summary>
        ///     签到送积分数
        /// </summary>
        [Display(Name = "签到送积分数")]
        [Range(0, 10000)]
        public int SignPoints { get; set; }

        /// <summary>
        ///     是否启用购物送积分
        /// </summary>
        [Display(Name = "是否启用购物送积分")]
        public bool IsEnableShoppingPoints { get; set; }

        /// <summary>
        ///     购物送积分比率=消费金额*购物送积分比率
        /// </summary>
        [Display(Name = "购物送积分比率", Description = "购物实际所得积分=消费金额*购物送积分比率")]
        [Range(0, 1)]
        public decimal ShoppingPoints { get; set; }
    }

    /// <summary>
    ///     提现设置
    /// </summary>
    public class Settings_Withdraw : WeiChat_AdminUniqueTenantBase<int>
    {
        /// <summary>
        ///     是否允许提现
        /// </summary>
        [Display(Name = "是否允许提现")]
        public bool IsEnableWithdraw { get; set; }

        /// <summary>
        ///     最低提现金额
        /// </summary>
        [Range(0, 1000)]
        [Display(Name = "最低提现金额")]
        public int MinimumWithdrawalAmount { get; set; }

        /// <summary>
        ///     手续费比率
        /// </summary>
        [Range(0, 1)]
        [Display(Name = "手续费比率")]
        public decimal FeePercentage { get; set; }

        /// <summary>
        ///     提现提示
        /// </summary>
        [Display(Name = "提现提示")]
        public string Tip { get; set; }
    }

    /// <summary>
    ///     产品（商品）设置
    /// </summary>
    public class Settings_Product : WeiChat_AdminUniqueTenantBase<int>
    {
        /// <summary>
        ///     是否显示销量
        /// </summary>
        [Display(Name = "是否显示销量")]
        public bool IsEnableShowSalesCount { get; set; }

        /// <summary>
        ///     是否允许评价
        /// </summary>
        [Display(Name = "是否允许评价")]
        public bool IsEnableComment { get; set; }
    }

    /// <summary>
    ///     购物设置
    /// </summary>
    public class Settings_Shopping : WeiChat_AdminUniqueTenantBase<int>
    {
        /// <summary>
        ///     是否需要手机认证
        /// </summary>
        [Display(Name = "是否需要手机认证")]
        public bool IsEnableMobilePhoneAuthentication { get; set; }
    }

    /// <summary>
    ///     订单设置
    /// </summary>
    public class Settings_Order : WeiChat_AdminUniqueTenantBase<int>
    {
        /// <summary>
        ///     自动收货天数设置
        /// </summary>
        [Display(Name = "自动收货天数设置")]
        [Range(1, 90)]
        public int AutomaticGoodsReceiptDays { get; set; }
    }
}