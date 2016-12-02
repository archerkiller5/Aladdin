// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ViewModel.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:11
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using Magicodes.WeiChat.Data.Models.Order;

namespace Magicodes.Shop.Controllers.Product
{
    public class ViewModel
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public decimal TotalPrice { get; set; }
        public EnumThirdPayType ThirdPayType { get; set; }
        public DateTime? DealOn { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? PaymentOn { get; set; }
        public DateTime? ShippingOn { get; set; }
        public DateTime? ReceiptOn { get; set; }
        public decimal Shipping { get; set; }
        public string Leave { get; set; }
        public EnumOrderStatus State { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        public OrderLogistics OrderLogistics { get; set; }
    }

    public class OrderDetail
    {
        public Guid? ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Price { get; set; }
        public int? Quantity { get; set; }
    }

    public class OrderLogistics
    {
        public Guid? Id { get; set; }
        public string Consignee { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Logistics { get; set; }
        public string ShippingCode { get; set; }
    }
}