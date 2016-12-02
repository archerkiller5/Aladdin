// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : RefundApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Magicodes.Logger;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeChat.SDK.Pays;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/Refund")]
    public class RefundApiController : TenantBaseApiController<Order_Info>
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;

        #region 退换货订单

        /// <summary>
        ///     退换货订单
        /// </summary>
        /// <param name="state"></param>
        /// <param name="pageStart"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefundOrder/{state}")]
        public IHttpActionResult RefundOrder(int state, int pageStart = 0, int pageSize = 10)
        {
            try
            {
                var refundOrders = new List<Order_Refund>();
                var openId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                if (state == 0)
                    refundOrders = db.Order_Refunds
                        .Where(o => o.OpenId == openId)
                        .OrderByDescending(o => o.CreateTime)
                        .Skip(pageStart).Take(pageSize).ToList();
                else
                    refundOrders = db.Order_Refunds
                        .Where(o => (o.OpenId == openId) && (o.State == (EnumOrderRefundState) state))
                        .OrderByDescending(o => o.CreateTime)
                        .Skip(pageStart).Take(pageSize).ToList();

                var orderIds = refundOrders.Select(o => o.OrderID).ToList();
                var refundIds = refundOrders.Select(o => o.Id).ToList();

                var orderDetails = db.Order_Details.Where(o => orderIds.Contains(o.OrderID)).ToList();
                var refundDetails = db.Order_RefundDetails.Where(o => refundIds.Contains(o.OrderRefund)).ToList();
                var orders = db.Order_Infos.Where(o => orderIds.Contains(o.Id)).ToList();

                var val = new List<object>();
                foreach (var item in refundOrders)
                {
                    var refundDetail = refundDetails.Where(o => o.OrderRefund == item.Id).ToList();
                    var orderDetail = orderDetails.Where(o => o.OrderID == item.OrderID).ToList();
                    val.Add(new
                    {
                        Order = item,
                        OrderDetail = orderDetail.ConvertAll(v =>
                        {
                            var refund = refundDetail.SingleOrDefault(o => o.OrderDetail == v.Id);
                            return new
                            {
                                Detail = v,
                                StateName = null == refund ? "" : item.Type.GetEnumMemberDisplayName()
                            };
                        }),
                        Num = orderDetail.Sum(o => o.Quantity),
                        RefundNum = orderDetail.ConvertAll(v =>
                        {
                            var refund = refundDetail.SingleOrDefault(o => o.OrderDetail == v.Id);
                            return new
                            {
                                Quantity = null == refund ? 0 : v.Quantity
                            };
                        }).Sum(o => o.Quantity),
                        orders.SingleOrDefault(o => o.Id == item.OrderID).TotalPrice,
                        orders.SingleOrDefault(o => o.Id == item.OrderID).Shipping,
                        StateName = item.State.GetEnumMemberDisplayName()
                    });
                }
                return Ok(val);
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Debug, "RefundOrder：" + ex.Message);
            }
            return Ok();
        }

        /// <summary>
        ///     退换货订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefundOrder/Detail")]
        public IHttpActionResult RefundDetail(Guid id)
        {
            try
            {
                var refund =
                    db.Order_Refunds.SingleOrDefault(
                        o => (o.Id == id) && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId));
                if (null == refund)
                    return Ok(new {success = false, message = "订单信息不存在"});

                var orderDetails = db.Order_Details.Where(o => o.OrderID == refund.OrderID).ToList();
                var refundDetails = db.Order_RefundDetails.Where(o => o.OrderRefund == refund.Id).ToList();
                var order = db.Order_Infos.SingleOrDefault(o => o.Id == refund.OrderID);
                var OrderDetail = orderDetails.ConvertAll(v =>
                {
                    var temp = refundDetails.SingleOrDefault(o => o.OrderDetail == v.Id);
                    return new
                    {
                        Detail = v,
                        StateName = null == temp ? "" : refund.Type.GetEnumMemberDisplayName()
                    };
                });
                var val = new
                {
                    StateName = refund.State.GetEnumMemberDisplayName(),
                    order.Shipping,
                    order.TotalPrice,
                    CreateTime = refund.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                };

                return Ok(new {success = true, val, Refund = refund, Details = OrderDetail});
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Debug, "RefundDetail：" + ex.Message);
            }
            return Ok(new {success = false, message = "订单信息不存在"});
        }

        /// <summary>
        ///     退换货下单
        /// </summary>
        /// <param name="inparamters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RefundOrder/Add")]
        public IHttpActionResult AddRefundOrder([FromBody] string[] inparamters)
        {
            var ids = string.Empty;
            if ((null == inparamters) || (inparamters.Length < 1))
                return Ok(new {StatusCode = 0, message = "订单信息不存在"});
            try
            {
                var id = inparamters[0].ToGuid();
                var openId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                var order = db.Order_Infos.SingleOrDefault(o => (o.Id == id) && (o.OpenId == openId));
                if (null == order)
                    return Ok(new {StatusCode = 0, message = "订单信息不存在"});

                var price = inparamters[1].ToDecimal(0m);
                var type = inparamters[2].ToInt(1);
                var reason = inparamters[3].FilterHtml().FilterSqlKey();
                var remark = inparamters[4].FilterHtml().FilterSqlKey();
                var details = (JArray) JsonConvert.DeserializeObject(inparamters[5]);

                #region 退换货订单

                var refundOrder = new Order_Refund
                {
                    Id = Guid.NewGuid(),
                    OrderID = order.Id,
                    OrderCode = order.Code,
                    Code = PayUtil.GenerateOutTradeNo(),
                    Type = (EnumOrderRefundType) type,
                    Amount = price,
                    Quantity = 0,
                    Reason = reason,
                    Logistics = string.Empty,
                    ShippingCode = string.Empty,
                    Consignee = string.Empty,
                    Mobile = string.Empty,
                    Address = string.Empty,
                    State =
                        (EnumOrderRefundType) type == EnumOrderRefundType.Return
                            ? EnumOrderRefundState.Replace
                            : EnumOrderRefundState.WaitSellerAgree,
                    Remark = remark,
                    OpenId = openId,
                    CreateTime = DateTime.Now,
                    TenantId = TenantId
                };

                #endregion

                #region 订单产品

                var by = false;
                var shipping = new List<decimal>();
                foreach (var detail in details)
                {
                    var detailID = detail.ToString().ToGuid();
                    var product = db.Order_Details.FirstOrDefault(o => o.Id == detailID);
                    if (null != product)
                    {
                        var refundDetail = new Order_RefundDetail
                        {
                            Id = Guid.NewGuid(),
                            OrderRefund = refundOrder.Id,
                            OrderDetail = product.Id,
                            OpenId = openId,
                            CreateTime = DateTime.Now,
                            TenantId = TenantId
                        };
                        db.Order_RefundDetails.Add(refundDetail);
                        //增加退换货数据
                        refundOrder.Quantity += product.Quantity;
                    }
                }

                #endregion

                //订单变更状态
                order.State = EnumOrderStatus.ReturnedGoods;
                db.Entry(order).State = EntityState.Modified;
                if (db.Order_Refunds.Any(o => o.Code == refundOrder.Code))
                    refundOrder.Code = PayUtil.GenerateOutTradeNo();
                db.Order_Refunds.Add(refundOrder);

                db.SaveChanges();

                return Ok(new {StatusCode = 1, message = refundOrder.Id});
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Debug, "Order/Add：" + ex.Message);
                return Ok(new {StatusCode = 0, message = "生成失败"});
            }
        }

        /// <summary>
        ///     换货确认收货
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RefundOrder/Confirm/{id}")]
        public IHttpActionResult RefundConfirm(Guid id)
        {
            try
            {
                var order =
                    db.Order_Refunds.SingleOrDefault(
                        o => (o.Id == id) && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId));
                if (null == order)
                    return Ok(new {StatusCode = 0, message = "订单不存在"});

                order.State = EnumOrderRefundState.ReplaceSuccess;
                db.Entry(order).State = EntityState.Modified;
                //更新产品换货数
                var val = db.Order_RefundDetails.Where(o => o.OrderRefund == order.Id)
                    .Join(db.Order_Details, t => t.OrderDetail, ts => ts.Id, (t, ts) => new
                    {
                        ID = t.Id,
                        ts.ProductID,
                        ts.Quantity
                    }).ToList();
                db.SaveChanges();

                return Ok(new {StatusCode = 1, message = "换货成功"});
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Debug, "RefundConfirm：" + ex.Message);
            }
            return Ok(new {StatusCode = 0, message = "操作失败"});
        }

        /// <summary>
        ///     退款发货
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RefundOrder/Send")]
        public IHttpActionResult SendGoods([FromBody] string[] inparamters)
        {
            try
            {
                if ((null == inparamters) || (inparamters.Length < 1))
                    return Ok(new {StatusCode = 0, message = "订单信息不存在"});
                var id = inparamters[0].ToGuid();
                var order =
                    db.Order_Refunds.SingleOrDefault(
                        o => (o.Id == id) && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId));
                if (null == order)
                    return Ok(new {StatusCode = 0, message = "订单信息不存在"});

                //修改订单状态
                order.State = EnumOrderRefundState.WaitSellerConfirmGoods;
                order.Logistics = inparamters[1];
                order.ShippingCode = inparamters[2];
                order.Address = inparamters[3];
                order.Consignee = inparamters[4];
                order.Mobile = inparamters[5];
                db.Entry(order).State = EntityState.Modified;

                db.SaveChanges();
                return Ok(new {StatusCode = 1, message = "发货成功"});
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Debug, "RefundOrder/Send：" + ex.Message);
            }
            return Ok(new {StatusCode = 0, message = "发货失败"});
        }

        #endregion
    }
}