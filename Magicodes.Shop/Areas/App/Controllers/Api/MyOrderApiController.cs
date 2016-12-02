// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MyOrderApiController.cs
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
using System.Data.SqlTypes;
using System.Linq;
using System.Web.Http;
using Magicodes.Logger;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Pays;
using Magicodes.WeChat.SDK.Pays.TenPayV3;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using Magicodes.Shop.Helpers;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/MyOrder")]
    public class MyOrderApiController : TenantBaseApiController<Order_Info>
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;

        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get(int orderstate = 0)
        {
            var listObject = new List<object>();
            var listOrder = new List<Order_Info>();
            try
            {
                var q =
                    db.Order_Infos.OrderByDescending(p => p.CreateTime)
                        .Where(p => p.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId);

                if (orderstate > 0)
                {
                    var status = (EnumOrderStatus)orderstate;
                    q = q.Where(p => p.State == status);
                }
                if (orderstate == 0)
                {
                    q = q.Where(p => p.State != EnumOrderStatus.PaidDelete &&
                    p.State != EnumOrderStatus.UnpaidDelete);
                }
                listOrder = q.ToList();
                //通过订单id获取订单明细
                foreach (var o in listOrder)
                {
                    var listProductItems = db.Order_Details.Where(p => p.OrderID == o.Id).ToList();
                    var op = new
                    {
                        OrderId = o.Id,
                        Status = o.State.GetEnumMemberDisplayName(),
                        CreateTime = o.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        o.TotalPrice,
                        TotalSum = o.TotalPrice,
                        Image = listProductItems[0].ProductImage,
                        ProductList = listProductItems
                    };
                    listObject.Add(op);
                }
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Debug, ex.ToString());
            }
            return Ok(listObject);
        }

        #region 退换货

        [HttpPost]
        [Route("ApplyRefund")]
        public IHttpActionResult ApplyRefund([FromBody] string[] inparamters)
        {
            var ids = string.Empty;
            if ((null == inparamters) || (inparamters.Length < 1))
                return Ok(new { StatusCode = 0, message = "订单信息不存在" });
            try
            {
                var OrderId = inparamters[0].ToGuid();
                var openId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                var order = db.Order_Infos.SingleOrDefault(o => o.Id == OrderId);
                if (null == order)
                    return Ok(new { StatusCode = 0, message = "订单信息不存在" });
                var reason = inparamters[1];

                #region 退换货订单

                var refundOrder = new Order_Refund
                {
                    Id = Guid.NewGuid(),
                    OrderID = order.Id,
                    OrderCode = order.Code,
                    Code = PayUtil.GenerateOutTradeNo(),
                    Type = EnumOrderRefundType.ReturnGoods,
                    Amount = order.TotalPrice,
                    Quantity = 0,
                    Reason = "",
                    Logistics = string.Empty,
                    ShippingCode = string.Empty,
                    Consignee = string.Empty,
                    Mobile = string.Empty,
                    Address = string.Empty,
                    State = EnumOrderRefundState.WaitSellerAgree,
                    Remark = "",
                    OpenId = openId,
                    CreateTime = DateTime.Now,
                    TenantId = TenantId
                };

                #endregion

                #region 订单产品

                var details = db.Order_Details.Where(p => p.OrderID == order.Id).ToList();
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
                if (db.Order_Refunds.Any(o => o.Code == refundOrder.Code))
                    refundOrder.Code = PayUtil.GenerateOutTradeNo();
                db.Order_Refunds.Add(refundOrder);

                db.SaveChanges();

                return Ok(new { StatusCode = 1, message = refundOrder.Id });
            }
            catch (Exception ex)
            {
                return Ok(new { StatusCode = 0, message = "生成失败" });
            }

            #endregion
        }

        #region 订单

        /// <summary>
        ///     订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Detail")]
        public IHttpActionResult Detail(Guid id)
        {
            try
            {
                log.Log(LoggerLevels.Debug, "ID:" + id);
                var order =
                    db.Order_Infos.SingleOrDefault(
                        o => (o.Id == id) && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId));
                if (null == order)
                    return Ok(new { success = false, message = "订单信息不存在" });
                var orderProducts = db.Order_Details.Where(o => o.OrderID == id).ToList();
                var orderLogistics = db.Order_Logistics.SingleOrDefault(o => o.OrderID == id);
               
                var val = new
                {
                    StateName = order.State.GetEnumMemberDisplayName(),
                    DealOn = order.DealOn.ToString(),
                    PaymentOn = order.PaymentOn == (DateTime)SqlDateTime.MinValue ? "" : order.PaymentOn.ToString(),
                    ShippingOn = order.ShippingOn == (DateTime)SqlDateTime.MinValue ? "" : order.ShippingOn.ToString(),
                    ReceiptOn = order.ReceiptOn == (DateTime)SqlDateTime.MinValue ? "" : order.ReceiptOn.ToString()
                };

                return Ok(new { val, Order = order, Details = orderProducts, Logistics = orderLogistics, PrePrice = 0 });
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Debug, "OrderDetail：" + ex.Message);
            }
            return BadRequest("订单信息不存在!");
        }

        [HttpGet]
        [Route("getFreight")]
        public IHttpActionResult GetFreight()
        {
            var freight = db.Logistics_FreightTemplates.ToList();
            return Ok(freight);
        }

        /// <summary>
        ///     下单
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public IHttpActionResult Add([FromBody] dynamic Json)
        {
            var ids = string.Empty;
            if (null == Json)
                return Ok(new { success = false, message = "产品信息不存在" });

            try
            {
                Guid AddressId = Guid.Parse(Convert.ToString(Json.AddressId));
                string leave = Convert.ToString(Json.Remark);
                Guid FreiId = Guid.Parse(Convert.ToString(Json.YunFei));
                var carts = (JArray)JsonConvert.DeserializeObject(Convert.ToString(Json.Products));
                var FreiPrice = db.Logistics_FreightTemplates.Find(FreiId).Price;
                HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
                #region 订单信息

                var order = new Order_Info
                {
                    Id = Guid.NewGuid(),
                    Code = PayUtil.GenerateOutTradeNo(),
                    OpenId = WeiChatApplicationContext.Current.WeiChatUser.OpenId,
                    TotalPrice = 0,
                    Remark = "",
                    //User = WeiChatApplicationContext.Current.WeiChatUser,
                    ThirdPayType = EnumThirdPayType.WX,
                    DealOn = DateTime.Now,
                    PaymentOn = (DateTime)SqlDateTime.MinValue,
                    ShippingOn = (DateTime)SqlDateTime.MinValue,
                    ReceiptOn = (DateTime)SqlDateTime.MinValue,
                    Shipping = FreiPrice,
                    Leave = leave,
                    RejectReason = string.Empty,
                    State = EnumOrderStatus.Obligation,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    TenantId = TenantId,
                    ClientIpAddress = context != null ? context.GetClientIpAddress() : null
                };

                #endregion

                #region 订单产品

                var by = false;
                var shipping = new List<decimal>();
                foreach (var cart in carts)
                {
                    var cartID = Guid.Parse(cart.ToString());
                    var cartTemp = db.Cart_Infos.FirstOrDefault(o => o.Id == cartID);

                    var product = (from pro in db.Product_Infos
                                   join photo in db.Site_Photos on pro.Id equals photo.GalleryId
                                   where pro.Id == cartTemp.ProductID
                                   select new { pro.Name, photo.Url, pro.Id }).FirstOrDefault();

                    //db.Product_Infos.FirstOrDefault(o => o.Id == cartTemp.ProductID);
                    log.Log(LoggerLevels.Debug, "product：" + JsonConvert.SerializeObject(product));
                    var package = db.Product_ProductAttributes.FirstOrDefault(o => o.AttributeId == cartTemp.PackageID);
                    log.Log(LoggerLevels.Debug, "package：" + JsonConvert.SerializeObject(package));
                    log.Log(LoggerLevels.Debug, "cartTemp：" + JsonConvert.SerializeObject(cartTemp));
                    if (null != cartTemp)
                    {
                        var orderDetail = new Order_Detail
                        {
                            Id = Guid.NewGuid(),
                            OrderID = order.Id,
                            ProductName = product.Name,
                            ProductImage = product.Url,
                            ProductID = product.Id,
                            PackageID = package.Id,
                            Price = package.AttributePrice,
                            Quantity = cartTemp.Quantity,
                            OpenId = order.OpenId,
                            CreateTime = DateTime.Now,
                            TenantId = TenantId
                        };
                        db.Order_Details.Add(orderDetail);
                        //清除购物车
                        db.Cart_Infos.Remove(cartTemp);
                        //计算费用
                        order.TotalPrice += orderDetail.Price * orderDetail.Quantity;
                    }
                }

                #endregion

                #region 收货

                var address = db.User_Addresses.FirstOrDefault(o => o.Id == AddressId);
                log.Log(LoggerLevels.Debug, "address：" + JsonConvert.SerializeObject(address));
                var orderLogistics = new Order_Logistics
                {
                    Id = Guid.NewGuid(),
                    OrderID = order.Id,
                    Consignee = address.Name,
                    Province = address.Province,
                    City = address.City,
                    Area = address.District,
                    Address = address.Street,
                    Mobile = address.PhoneNumber,
                    Logistics = string.Empty,
                    ShippingCode = string.Empty,
                    OpenId = order.OpenId,
                    CreateTime = DateTime.Now,
                    TenantId = TenantId
                };
                db.Order_Logistics.Add(orderLogistics);

                #endregion

                if (db.Order_Infos.Any(o => o.Code == order.Code)) order.Code = PayUtil.GenerateOutTradeNo();

                db.Order_Infos.Add(order);
                db.SaveChanges();
                return Ok(order.Id);
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Error, "Order/Add：" + ex.Message);
                return BadRequest("系统异常，请联系管理员！");
            }
        }

        /// <summary>
        ///     微信支付（统一下单）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Pay/{id}")]
        public IHttpActionResult WechatPay(Guid id)
        {
            try
            {
                //查询订单
                var order =
                    db.Order_Infos.SingleOrDefault(
                        o => (o.Id == id) && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId));
                if (null == order)
                    return BadRequest("订单信息不存在");
                
                #region 统一下单
                var model = new UnifiedorderRequest
                {
                    OpenId = WeiChatApplicationContext.Current.WeiChatUser.OpenId,
                    SpbillCreateIp = "8.8.8.8",
                    OutTradeNo = order.Code,
                    TotalFee = Convert.ToInt32((order.TotalPrice + order.Shipping) * 100).ToString(),
                    NonceStr = PayUtil.GetNoncestr(),
                    TradeType = "JSAPI",
                    Body = "购买商品",
                    DeviceInfo = "WEB"
                };
                var result = WeChatApisContext.Current.TenPayV3Api.Unifiedorder(model);

                var _dict = new Dictionary<string, string>
                {
                    {"appId", result.AppId},
                    {"timeStamp", PayUtil.GetTimestamp()},
                    {"nonceStr", result.NonceStr},
                    {"package", "prepay_id=" + result.PrepayId},
                    {"signType", "MD5"}
                };
                _dict.Add("paySign", PayUtil.CreateMd5Sign(_dict, WeChatConfigManager.Current.GetPayConfig().TenPayKey));

                #endregion

                return Ok(_dict);
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Error, "WechatPay：" + ex.Message);
            }
            return BadRequest("操作失败，请联系管理员!");
        }

        /// <summary>
        ///     关闭或删除订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order/Cancel/{id}/{state}")]
        public IHttpActionResult CancelOrder(Guid id, int state)
        {
            try
            {
                var order =
                    db.Order_Infos.SingleOrDefault(
                        o => (o.Id == id) && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId));
                if (null == order)
                    return Ok(new { success = false, message = "订单不存在" });

                order.State = (EnumOrderStatus)state;
                order.UpdateTime = DateTime.Now;
                //变更收货时间
                if (order.State == EnumOrderStatus.Success) order.ReceiptOn = DateTime.Now;

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { success = true, message = "操作成功" });
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Debug, "CancelOrder：" + ex.Message);
            }
            return Ok(new { success = false, message = "操作失败" });
        }

        /// <summary>
        /// 供前台删除订单用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DelOrder/{id}")]
        public IHttpActionResult DelOrder(Guid id)
        {
            try
            {
                var order =
                    db.Order_Infos.SingleOrDefault(
                        o => (o.Id == id) && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId));
                if (null == order)
                    return Ok(new { success = false, message = "订单不存在" });
                if (order.State == EnumOrderStatus.Obligation)
                    order.State = EnumOrderStatus.UnpaidDelete;
                else
                    order.State = EnumOrderStatus.PaidDelete;

                order.UpdateTime = DateTime.Now;

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { success = true, message = "操作成功" });
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Error, "DelOrder：" + ex.Message);
            }
            return Ok(new { success = false, message = "操作失败" });
        }
        /// <summary>
        /// 确认收货 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ConfirmReceiptGoods/{id}")]
        public IHttpActionResult ConfirmReceiptGoods(Guid id)
        {
            try
            {
                var order =
                db.Order_Infos.SingleOrDefault(
                    o => (o.Id == id) && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId));
                if (null == order)
                    return Ok(new { success = false, message = "订单不存在" });

                order.State = EnumOrderStatus.Success;
                order.UpdateTime = DateTime.Now;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                return Ok(new { success = true, message = "操作成功" });
            }
            catch (Exception ea)
            {
                log.Log(LoggerLevels.Error, "ConfirmReceiptGoods：" + ea.Message);
            }
            return Ok(new { success = false, message = "操作失败" });
        }

        #endregion
    }
}