using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.Settings
{
    /// <summary>
    /// 短信类型
    /// </summary>
    public enum SmsTypes
    {
        /// <summary>
        /// 【用户】绑定手机验证
        /// </summary>
        [Display(Name = "【用户】绑定手机验证")]
        BoundPhone = 0,
        /// <summary>
        /// 【用户】忘记密码验证
        /// </summary>
        [Display(Name = "【用户】忘记密码验证")]
        ForgetPassword = 1,
        /// <summary>
        /// 【用户】订单支付成功提醒
        /// </summary>
        [Display(Name = "【用户】订单支付成功提醒")]
        OrderPaymentSuccess = 2,
        /// <summary>
        /// 【用户】订单支付待支付提醒
        /// </summary>
        [Display(Name = "【用户】订单支付待支付提醒")]
        OrderToBePaid = 3,
        /// <summary>
        /// 【用户】订单发货通知
        /// </summary>
        [Display(Name = "【用户】订单发货通知")]
        OrderDelivery = 4,
        /// <summary>
        /// 【用户】订单签收通知
        /// </summary>
        [Display(Name = "【用户】订单签收通知")]
        OrderReceipt = 5,
        /// <summary>
        /// 【用户】提现审核结果通知
        /// </summary>
        [Display(Name = "【用户】提现审核结果通知")]
        PresentAuditResults = 6,
        /// <summary>
        /// 【商家】新订单通知
        /// </summary>
        [Display(Name = "【商家】新订单通知")]
        NewOrders = 7,
        /// <summary>
        /// 测试短信发送
        /// </summary>
        [Display(Name = "测试短信发送")]
        TestSendMsg
        
    }
}