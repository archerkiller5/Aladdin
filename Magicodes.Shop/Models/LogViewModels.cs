// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : LogViewModels.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:16
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.ComponentModel.DataAnnotations;
using Magicodes.WeiChat.Data.Models.Order;

namespace Magicodes.Shop.Models
{
    public class LogOrderViewModel
    {
        public string OpenId { get; set; }
        [Display(Name = "手机号")]
        public string Mobile { get; set; }
        [Display(Name = "订单ID")]
        public Guid OrderId { get; set; }
        [Display(Name = "订单金额")]
        public decimal Amount { get; set; }
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }
        [Display(Name = "支付方式")]
        public EnumThirdPayType PaymentType { get; set; }
    }
}