// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_InfoController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Pays;
using Magicodes.WeChat.SDK.Pays.EnterprisePay;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.WeiChat.Unity;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Order
{
    [RoleMenuFilter("订单管理", "4ACE9D95-4D3A-4B5C-A684-DC0CF3E115CF", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-cogs")]
    public class Order_InfoController : TenantBaseController<Order_Info>
    {
        // GET: Order_Info
        [RoleMenuFilter("订单信息管理", "B9832552-75A4-445C-A578-BF90FD9B7678", "Admin,TenantManager,ShopManager",
             url: "/Order_Info", parentId: "4ACE9D95-4D3A-4B5C-A684-DC0CF3E115CF")]
        // GET: Product_Info
        [AuditFilter("订单信息查询", "1EA07E0E-742B-4FC3-A32E-56BEE0F2A904")]
        public ActionResult Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Order_Infos.AsQueryable();

            //所有订单数
            ViewBag.OrderCount = queryable.Where(p => p.State != EnumOrderStatus.ReturnedGoods).Count();
            //待付款
            ViewBag.ObligationCount = queryable.Where(p => p.State == EnumOrderStatus.Obligation).Count();
            //待发货
            ViewBag.OverhangCount = queryable.Where(p => p.State == EnumOrderStatus.Overhang).Count();
            //待收货
            ViewBag.WaitReceivingCount = queryable.Where(p => p.State == EnumOrderStatus.WaitReceiving).Count();
            //交易完成
            ViewBag.SuccessCount = queryable.Where(p => p.State == EnumOrderStatus.Success).Count();
            //已关闭
            ViewBag.ClosedCount = queryable.Where(p => p.State == EnumOrderStatus.Closed).Count();
            //未付款删除
            ViewBag.UnpaidDeleteCount = queryable.Where(p => p.State == EnumOrderStatus.UnpaidDelete).Count();
            //已付款删除
            ViewBag.PaidDeleteCount = queryable.Where(p => p.State == EnumOrderStatus.PaidDelete).Count();

            return View();

            //int orderState = StringHelper.ToInt(Request.Params["OrderState"].ToString(), 0);//订单状态编号
            //string orderNo = Request.Params["OrderNo"];//订单编号
            ////string userId = Request.Params["UserId"];//会员id
            //int freighType = StringHelper.ToInt(Request.Params["FreighType"].ToString(), 0);//配送方式
            //int payType = StringHelper.ToInt(Request.Params["PayType"].ToString(), 0);//支付方式
            //string startDate = Request.Params["StartDate"];//下单时间（开始时间）
            //string endDate = Request.Params["EndDate"];//下单时间（结束时间）

            //string shippingCode = Request.Params["ShippingCode"];//快递单号
            //string mobile = Request.Params["Mobile"];//收货人手机
            //string consignee = Request.Params["Consignee"];//收货人姓名

            //var queryable = from a in db.Order_Infos
            //                join b in db.Order_Logistics on a.Id equals b.OrderID
            //                select new
            //                {
            //                    Id = a.Id,
            //                    Code = a.Code,
            //                    TotalPrice = a.TotalPrice,
            //                    ThirdPayType = a.ThirdPayType,
            //                    DealOn = a.DealOn,
            //                    CreateTime = a.CreateTime,
            //                    PaymentOn = a.PaymentOn,
            //                    ShippingOn = a.ShippingOn,
            //                    ReceiptOn = a.ReceiptOn,
            //                    Logistics = a.Logistics,
            //                    Leave = a.Leave,
            //                    State =a.State,
            //                    OrderLogistics = new OrderLogistics
            //                    {
            //                        Id = b.Id,
            //                        Consignee = b.Consignee,
            //                        Address = b.Province + b.City + b.Area,
            //                        Mobile = b.Mobile,
            //                        Logistics = b.Logistics,
            //                        ShippingCode = b.ShippingCode
            //                    },
            //                    OrderDetail = (from c in db.Order_Details
            //                                   where c.OrderID == a.Id
            //                                   select new OrderDetail
            //                                   {
            //                                       ProductID = c.ProductID,
            //                                       ProductName = c.ProductName,
            //                                       ProductImage = c.ProductImage,
            //                                       Price = c.Price.ToString(),
            //                                       Quantity = c.Quantity
            //                                   }).ToList()
            //                };

            //#region 查询符合查询条件的订单表数据
            ////查询订单状态
            //if (orderState > 0)
            //{
            //    queryable = queryable.Where(p => p.State == (EnumOrderStatus)orderState);
            //}
            ////订单号
            //if (!string.IsNullOrEmpty(orderNo))
            //{
            //    queryable = queryable.Where(p => p.Code.Contains(orderNo));
            //}
            ////支付方式
            //if (payType > 0)
            //{
            //    queryable = queryable.Where(p => p.ThirdPayType == (EnumThirdPayType)payType);
            //}
            ////配送方式
            //if (freighType > 0)
            //{
            //    queryable = freighType == 1 ? queryable.Where(p => p.Logistics > 0) : queryable.Where(p => p.Logistics <= 0);
            //}
            ////下单时间
            //if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            //{
            //    var sDate = Convert.ToDateTime(startDate);// StringHelper.ToDateTime(startDate + " 00:00:00", DateTime.Now);
            //    var eDate = Convert.ToDateTime(endDate).AddDays(1);//StringHelper.ToDateTime(endDate + " 23:59:59", DateTime.Now.AddDays(1).AddSeconds(-1));
            //    queryable = queryable.Where(p =>
            //    EntityFunctions.TruncateTime(p.CreateTime) >= EntityFunctions.TruncateTime(sDate)
            //    && EntityFunctions.TruncateTime(p.CreateTime) <= EntityFunctions.TruncateTime(eDate));
            //}
            //#endregion

            //#region 查出有关物流信息查询条件的订单数据
            ////快递单号
            //if (!string.IsNullOrEmpty(shippingCode))
            //{
            //    queryable = queryable.Where(p => p.OrderLogistics.ShippingCode.Contains(shippingCode));
            //}
            ////收货手机
            //if (!string.IsNullOrEmpty(mobile))
            //{
            //    queryable = queryable.Where(p => p.OrderLogistics.Mobile.Contains(mobile));
            //}
            ////收件人
            //if (!string.IsNullOrEmpty(consignee))
            //{
            //    queryable = queryable.Where(p => p.OrderLogistics.Consignee.Contains(consignee));
            //}
            //#endregion
            //var pagedList = new PagedList<ViewModel>(
            //                             queryable.OrderByDescending(p => p.CreateTime)
            //                             .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList().Select(p => new ViewModel()
            //                             {
            //                                 Id = p.Id,
            //                                 Code = p.Code,
            //                                 TotalPrice = p.TotalPrice,
            //                                 ThirdPayType = p.ThirdPayType,
            //                                 DealOn = p.DealOn,
            //                                 CreateTime = p.CreateTime,
            //                                 PaymentOn = p.PaymentOn,
            //                                 ShippingOn = p.ShippingOn,
            //                                 ReceiptOn = p.ReceiptOn,
            //                                 Logistics = p.Logistics,
            //                                 Leave = p.Leave,
            //                                 State = p.State,
            //                                 OrderLogistics =p.OrderLogistics,
            //                                 OrderDetail = p.OrderDetail
            //                             }),
            //                             pageIndex, pageSize, queryable.Count());
            //ViewBag.OrderType = getStateList();

            //return View(pagedList);
        }

        private Dictionary<EnumOrderStatus, int> getStateList()
        {
            var wglist = db.Order_Infos.ToList();
            var result = new Dictionary<EnumOrderStatus, int>();
            foreach (var state in Enum.GetNames(typeof(EnumOrderStatus)))
            {
                var pStatus = (EnumOrderStatus)Enum.Parse(typeof(EnumOrderStatus), state);
                if (pStatus == EnumOrderStatus.AllOrder)
                    result.Add(pStatus, wglist.Count());
                else
                    result.Add(pStatus, wglist.Where(w => w.State == pStatus).Count());
            }
            return result;
        }

        // GET: Order_Info/Create
        [AuditFilter("订单待创建", "517d8963-1280-46e8-a0e8-d9284093113f")]
        public ActionResult Create()
        {
            ViewBag.OpenId = new SelectList(db.WeiChat_Users, "OpenId", "NickName");
            return View();
        }


        /// <summary>
        ///     根据搜索条件查询订单列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order_Info/SearchOrderList")]
        public async Task<ActionResult> SearchOrderList()
        {
            var orderState = StringHelper.ToInt(Request.Params["OrderState"], 0); //订单状态编号
            var orderNo = Request.Params["OrderNo"]; //订单编号
            var freighType = StringHelper.ToInt(Request.Params["FreighType"], 0); //配送方式
            var payType = StringHelper.ToInt(Request.Params["PayType"], 0); //支付方式
            var startDate = Request.Params["StartDate"]; //下单时间（开始时间）
            var endDate = Request.Params["EndDate"]; //下单时间（结束时间）

            var shippingCode = Request.Params["ShippingCode"]; //快递单号
            var mobile = Request.Params["Mobile"]; //收货人手机
            var consignee = Request.Params["Consignee"]; //收货人姓名

            var pageIndex = Convert.ToInt32(Request.Params["PageIndex"]); //当前页
            var pageSize = Convert.ToInt32(Request.Params["PageSize"]); //页大小

            var ajaxResponse = new AjaxResponse<object>();

            var queryable = db.Order_Infos.Where(o => o.State != EnumOrderStatus.ReturnedGoods).AsQueryable();

            var templist = queryable;

            #region 查询符合查询条件的订单表数据

            if ((orderState > 0) && (queryable.ToList().Count > 0))
            {
                templist = queryable.Where(p => p.State == (EnumOrderStatus)orderState);
                if (templist.ToList().Count >= 0)
                    queryable = templist;
            }
            if (!string.IsNullOrEmpty(orderNo) && (queryable.ToList().Count > 0))
            {
                templist = queryable.Where(p => p.Code.Contains(orderNo));
                if (templist.ToList().Count >= 0)
                    queryable = templist;
            }
            if ((payType > 0) && (queryable.ToList().Count > 0))
            {
                templist = queryable.Where(p => p.ThirdPayType == (EnumThirdPayType)payType);
                if (templist.ToList().Count >= 0)
                    queryable = templist;
            }
            if ((freighType > 0) && (queryable.ToList().Count > 0))
            {
                templist = freighType == 1
                    ? queryable.Where(p => p.Shipping > 0)
                    : queryable.Where(p => p.Shipping <= 0);
                if (templist.ToList().Count >= 0)
                    queryable = templist;
            }
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && (queryable.ToList().Count > 0))
            {
                var sDate = StringHelper.ToDateTime((startDate + " 00:00:00"), DateTime.Now);
                var eDate = StringHelper.ToDateTime((endDate + " 23:59:59"), DateTime.Now.AddDays(1).AddSeconds(-1));
                templist = queryable.Where(p =>
                    (EntityFunctions.TruncateTime(p.CreateTime) >= EntityFunctions.TruncateTime(sDate))
                    && (EntityFunctions.TruncateTime(p.CreateTime) <= EntityFunctions.TruncateTime(eDate)));

                if (templist.ToList().Count >= 0)
                    queryable = templist;
            }

            #endregion

            //没有可以匹配的订单数据
            if (queryable.ToList().Count <= 0)
            {
                ajaxResponse.Success = false;
                ajaxResponse.Result = null;
                return Json(ajaxResponse);
            }

            var totalCount = queryable.ToList().Count();

            //查出符合查询条件的订单数据并分页
            var pagedList = new PagedList<Order_Info>(
                await queryable.OrderByDescending(p => p.CreateTime)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());

            #region 查出关联物流数据表的数据

            //查出关联物流数据表的数据
            var orderList = from a in pagedList
                            join b in db.Order_Logistics on a.Id equals b.OrderID
                            select new
                            {
                                a.Id,
                                a.Code,
                                TotalPrice = a.TotalPrice.ToString(),
                                ThirdPayType = EnumHelper.GetEnumMemberDisplayName(a.ThirdPayType),
                                a.DealOn,
                                CreateTime = a.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                a.PaymentOn,
                                a.ShippingOn,
                                a.ReceiptOn,
                                Logistics = a.Shipping.ToString(),
                                Leave = a.Leave == null ? "" : a.Leave,
                                State = EnumHelper.GetEnumMemberDisplayName(a.State),
                                a.IsRefund,
                                OrderShipping = new
                                {
                                    b.Id,
                                    b.Consignee,
                                    Address = b.Province + b.City + b.Area,
                                    Mobile =
                                    b.Mobile.Length >= 11 ? b.Mobile.Substring(0, 3) + "****" + b.Mobile.Substring(7) : b.Mobile,
                                    b.Logistics,
                                    b.ShippingCode
                                },
                                OrderDetail = (from c in db.Order_Details
                                               where c.OrderID == a.Id
                                               select new
                                               {
                                                   c.ProductID,
                                                   c.ProductName,
                                                   c.ProductImage,
                                                   c.ProductAlias,
                                                   Rule1 = c.Rule1 + ":" + c.Rule1Value,
                                                   Rule2 = c.Rule2 + ":" + c.Rule2Value,
                                                   OriginalPrice = c.OriginalPrice.ToString(),
                                                   Price = c.Price.ToString(),
                                                   c.Quantity
                                               }).ToList(),
                                User = from d in db.WeiChat_Users
                                       where d.OpenId == a.OpenId
                                       select new
                                       {
                                           d.NickName
                                       }
                            };

            #endregion

            #region 查出有关物流信息查询条件的订单数据

            if (!string.IsNullOrEmpty(shippingCode) && (orderList.ToList().Count > 0))
            {
                var temp = orderList.Where(p => p.OrderShipping.ShippingCode.Contains(shippingCode));
                if (temp.ToList().Count >= 0)
                    orderList = temp;
            }
            if (!string.IsNullOrEmpty(mobile) && (orderList.ToList().Count > 0))
            {
                var temp = orderList.Where(p => p.OrderShipping.Mobile.Contains(mobile));
                if (temp.ToList().Count >= 0)
                    orderList = temp;
            }
            if (!string.IsNullOrEmpty(consignee) && (orderList.ToList().Count > 0))
            {
                var temp = orderList.Where(p => p.OrderShipping.Consignee.Contains(consignee));
                if (temp.ToList().Count >= 0)
                    orderList = temp;
            }

            #endregion

            if (orderList.ToList().Count() <= 0)
            {
                ajaxResponse.Success = false;
                ajaxResponse.Result = null;
                return Json(ajaxResponse);
            }

            var dic = new Dictionary<object, object>();
            dic.Add("TotalCount", totalCount);
            dic.Add("OrderList", orderList.ToList());

            ajaxResponse.Success = true;
            ajaxResponse.Result = dic;

            return Json(ajaxResponse);
        }

        [AuditFilter("订单详细", "2dcb2555-33e7-4f3e-b1f5-7e3c2c3025b8")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var Order_Info = await db.Order_Infos.FindAsync(id);
            if (Order_Info == null)
                return HttpNotFound();
            ViewBag.Details = await db.Order_Details.Where(o => o.OrderID == Order_Info.Id).ToListAsync();
            ViewBag.Logistics = await db.Order_Logistics.SingleOrDefaultAsync(o => o.OrderID == Order_Info.Id);
            var refund = await db.Order_Refunds.SingleOrDefaultAsync(o => o.OrderID == Order_Info.Id);
            if (null != refund)
            {
                ViewBag.RefundDetail = await db.Order_RefundDetails.Where(o => o.OrderRefund == refund.Id).ToListAsync();
                ViewBag.Refund = refund;
            }

            return View(Order_Info);
        }

        /// <summary>
        ///     发货页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> SendGoods(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var Logistics = await db.Order_Logistics.FindAsync(id);
            if (Logistics == null)
                return HttpNotFound();
            return View(Logistics);
        }

        /// <summary>
        ///     发货
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SendGood()
        {
            var id = StringHelper.ToGuid(Request["ID"]);
            var logistics = Request["Logistics"] ?? string.Empty;
            var code = Request["ShippingCode"] ?? string.Empty;
            var ajaxMessage = new AjaxResponse { Success = true, Message = "发货成功！" };
            try
            {
                var Logistics = await db.Order_Logistics.FindAsync(id);
                if (Logistics == null)
                {
                    ajaxMessage.Success = false;
                    ajaxMessage.Message = "订单不存在或已删除！";
                    return Json(ajaxMessage);
                }
                var order = await db.Order_Infos.FindAsync(Logistics.OrderID);
                if (order == null)
                {
                    ajaxMessage.Success = false;
                    ajaxMessage.Message = "订单不存在或已删除！";
                    return Json(ajaxMessage);
                }

                Logistics.Logistics = logistics;
                Logistics.ShippingCode = code;

                order.State = EnumOrderStatus.WaitReceiving;
                order.ShippingOn = DateTime.Now;
                order.UpdateTime = DateTime.Now;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = ex.Message;
            }
            return Json(ajaxMessage);
        }

        // POST: Order_Info/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<ActionResult> Create([Bind(Include = "Id,Code,UserID,TotalPrice,SecondPrice,ThirdPayType,DealOn,PaymentOn,ShippingOn,ReceiptOn,Logistics,Leave,RejectReason,State,IsRefund,Remark,OpenId,CreateTime,TenantId")] Order_Info Order_Info)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            Order_Info.Id = Guid.NewGuid();
        //SetModelWithChangeStates(Order_Info, default(Guid?));				
        //            db.Order_Info.Add(Order_Info);
        //            await db.SaveChangesAsync();
        //            return RedirectToAction("Index");
        //        }

        //        return View(Order_Info);
        //    }

        // GET: Order_Info/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var Order_Info = await db.Order_Infos.FindAsync(id);
            if (Order_Info == null)
                return HttpNotFound();
            return View(Order_Info);
        }

        // POST: Order_Info/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,Code,UserID,TotalPrice,SecondPrice,ThirdPayType,DealOn,PaymentOn,ShippingOn,ReceiptOn,Logistics,Leave,RejectReason,State,IsRefund,Remark,OpenId,CreateTime,TenantId")] Order_Info Order_Info)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        SetModelWithChangeStates(Order_Info, Order_Info.Id);		
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(Order_Info);
        //}

        // GET: Order_Info/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var Order_Info = await db.Order_Infos.FindAsync(id);
            if (Order_Info == null)
                return HttpNotFound();
            return View(Order_Info);
        }

        // POST: Order_Info/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var Order_Info = await db.Order_Infos.FindAsync(id);
            db.Order_Infos.Remove(Order_Info);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #region Ajax请求

        // POST: Order_Info/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order_Info/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Order_Infos.Where(p => ids.Contains(p.Id)).ToListAsync();
                    if (models.Count == 0)
                    {
                        ajaxResponse.Success = false;
                        ajaxResponse.Message = "没有找到匹配的项，项已被删除或不存在！";
                        return Json(ajaxResponse);
                    }
                    switch (operation.ToUpper())
                    {
                        case "DELETE":

                            #region 删除

                            {
                                db.Order_Infos.RemoveRange(models);
                                await db.SaveChangesAsync();
                                ajaxResponse.Success = true;
                                ajaxResponse.Message = string.Format("已成功操作{0}项！", models.Count);
                                break;
                            }

                        #endregion

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ajaxResponse.Success = false;
                    ajaxResponse.Message = ex.Message;
                }
            }
            else
            {
                ajaxResponse.Success = false;
                ajaxResponse.Message = "请至少选择一项！";
            }
            return Json(ajaxResponse);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        #region 订单操作

        /// <summary>
        ///     订单关闭或者删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> OpOrder()
        {
            var id = StringHelper.ToGuid(Request["ID"]);
            var type = Request["Type"] ?? string.Empty;
            var order = await db.Order_Infos.FindAsync(id);
            var ajaxMessage = new AjaxResponse { Success = true, Message = "操作成功！" };
            if (order == null)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = "订单不存在或已删除！";
                return Json(ajaxMessage);
            }

            try
            {
                switch (type.ToUpper())
                {
                    case "D":

                        #region 删除

                        {
                            if (order.State == EnumOrderStatus.Obligation)
                                order.State = EnumOrderStatus.UnpaidDelete;
                            else if ((int)order.State > (int)EnumOrderStatus.Obligation)
                                order.State = EnumOrderStatus.PaidDelete;
                            order.UpdateTime = DateTime.Now;
                            await db.SaveChangesAsync();
                            ajaxMessage.Success = true;
                            ajaxMessage.Message = "删除成功";
                            break;
                        }

                    #endregion

                    case "C":

                        #region 关闭

                        {
                            order.State = EnumOrderStatus.Closed;
                            order.UpdateTime = DateTime.Now;
                            await db.SaveChangesAsync();
                            ajaxMessage.Success = true;
                            ajaxMessage.Message = "关闭成功";
                        }

                        #endregion

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = ex.Message;
            }

            return Json(ajaxMessage);
        }

        /// <summary>
        ///     开启退换货
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> IsRefund()
        {
            var id = StringHelper.ToGuid(Request["ID"]);
            var order = await db.Order_Infos.FindAsync(id);
            var ajaxMessage = new AjaxResponse { Success = true, Message = "操作成功！" };
            if (order == null)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = "订单不存在或已删除！";
                return Json(ajaxMessage);
            }

            try
            {
                order.IsRefund = true;
                order.UpdateTime = DateTime.Now;
                await db.SaveChangesAsync();
                ajaxMessage.Success = true;
                ajaxMessage.Message = "开启成功";
            }
            catch (Exception ex)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = ex.Message;
            }

            return Json(ajaxMessage);
        }

        /// <summary>
        ///     保存备注
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> RemakSave()
        {
            var id = StringHelper.ToGuid(Request["ID"]);
            var remark = Request["remark"] ?? string.Empty;
            var order = await db.Order_Infos.FindAsync(id);
            var ajaxMessage = new AjaxResponse { Success = true, Message = "操作成功！" };
            if (order == null)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = "订单不存在或已删除！";
                return Json(ajaxMessage);
            }

            try
            {
                order.Remark = string.IsNullOrEmpty(order.Remark)
                    ? string.Concat(UserName, DateTime.Now, " ", remark.Replace("@", ""))
                    : string.Concat(order.Remark, "@", UserName, DateTime.Now, " ", remark.Replace("@", ""));
                order.UpdateTime = DateTime.Now;
                await db.SaveChangesAsync();
                ajaxMessage.Success = true;
                ajaxMessage.Message = "备注保存成功";
            }
            catch (Exception ex)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = ex.Message;
            }

            return Json(ajaxMessage);
        }

        #endregion

        #region 退换货

        [RoleMenuFilter("退换货管理", "339F5BB3-9A67-41CD-9311-C972EC3FD8A2", "Admin,TenantManager,ShopManager",
             url: "/Order_Info/Refund", parentId: "4ACE9D95-4D3A-4B5C-A684-DC0CF3E115CF")]
        // GET: Product_Info
        [AuditFilter("退换货管理", "E7A2F753-BE29-497D-9422-DEB5E39B9D72")]
        public async Task<ActionResult> Refund(string q, EnumOrderRefundState state = EnumOrderRefundState.Refund,
            int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Order_Refunds.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(p => p.Code.Contains(q) || p.OrderCode.Contains(q) || p.ShippingCode.Contains(q));
            queryable = queryable.Where(p => p.State == state);

            var pagedList = new PagedList<Order_Refund>
            (
                await queryable.OrderByDescending(p => p.CreateTime)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());

            ViewBag.RefundState = GetRefundStateList();
            ViewBag.State = state;
            var orderIds = pagedList.Select(o => o.OrderID).ToList();
            var refundIds = pagedList.Select(o => o.Id).ToList();
            var OpenIds = pagedList.Select(o => o.OpenId).ToList();

            ViewBag.OrderDetail = db.Order_Details.Where(o => orderIds.Contains(o.OrderID)).ToList();
            ViewBag.RefundDetail = db.Order_RefundDetails.Where(o => refundIds.Contains(o.OrderRefund)).ToList();
            ViewBag.User = db.WeiChat_Users.Where(o => OpenIds.Contains(o.OpenId)).ToList();

            return View(pagedList);
        }

        /// <summary>
        ///     退换货操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> OpRefund()
        {
            var id = StringHelper.ToGuid(Request["ID"]);
            var type = Request["Type"] ?? string.Empty;
            var order = await db.Order_Refunds.FindAsync(id);
            var ajaxMessage = new AjaxResponse { Success = true, Message = "操作成功！" };
            if (order == null)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = "退换货订单不存在或已删除！";
                return Json(ajaxMessage);
            }

            try
            {
                switch (type.ToUpper())
                {
                    case "C":

                        #region 关闭

                        {
                            order.State = EnumOrderRefundState.Closed;
                            await db.SaveChangesAsync();
                            ajaxMessage.Success = true;
                            ajaxMessage.Message = "关闭成功";
                            break;
                        }

                    #endregion

                    case "T":

                        #region 同意退款

                        {
                            order.State = EnumOrderRefundState.WaitBuyerReturnGoods;
                            await db.SaveChangesAsync();
                            ajaxMessage.Success = true;
                            ajaxMessage.Message = "同意退款成功";
                        }

                        #endregion

                        break;
                    case "R":

                        #region 拒绝退款

                        {
                            order.State = EnumOrderRefundState.SellerRefuseBuyer;
                            await db.SaveChangesAsync();
                            ajaxMessage.Success = true;
                            ajaxMessage.Message = "拒绝退款成功";
                        }

                        #endregion

                        break;
                    case "S":

                        #region 收货

                        {
                            order.State = EnumOrderRefundState.RefundSuccess;
                            await db.SaveChangesAsync();
                            //Todo:微信接口支付
                            Transfers(order.OpenId, (int)(order.Amount * 100));
                            ajaxMessage.Success = true;
                            ajaxMessage.Message = "收货成功";
                        }

                        #endregion

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = ex.Message;
            }

            return Json(ajaxMessage);
        }

        private void Transfers(string OpenId, int Amount)
        {
            var model = new EnterpriseRequest
            {
                Amount = Convert.ToInt32(Amount * 100).ToString(),
                CheckName = "NO_CHECK",
                Desc = "退货退款",
                DeviceInfo = "",
                NonceStr = PayUtil.GetNoncestr(),
                OpenId = OpenId,
                PartnerTradeNo = PayUtil.GenerateOutTradeNo()
            };

            var relust = WeChatApisContext.Current.EnterprisePayApi.EnterprisePayment(model);
        }

        private Dictionary<EnumOrderRefundState, int> GetRefundStateList()
        {
            var
                refundOrder = db.Order_Refunds.ToList();
            var
                result = new Dictionary<EnumOrderRefundState, int>
                    ();
            foreach (var state in Enum.GetNames(typeof(EnumOrderRefundState)))
            {
                var pStatus = (EnumOrderRefundState)Enum.Parse(typeof(EnumOrderRefundState), state);
                result.Add(pStatus, refundOrder.Where(w => w.State == pStatus).Count());
            }
            return result;
        }

        #endregion
    }
}