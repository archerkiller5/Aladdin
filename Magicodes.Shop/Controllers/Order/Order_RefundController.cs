// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_RefundController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.Mvc.AuditFilter;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Order
{
    public class Order_RefundController : TenantBaseController<Order_Refund>
    {
        // GET: Order_Refund
        [AuditFilter("请输入审计关键描述，比如“订单查询”", "894a09c3-4a6c-4c70-bef4-eb53ee0df905")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Order_Refunds.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.OrderCode.Contains(q) || p.Code.Contains(q) || p.Reason.Contains(q) ||
                            p.Logistics.Contains(q) || p.ShippingCode.Contains(q) || p.Consignee.Contains(q) ||
                            p.Mobile.Contains(q) || p.Address.Contains(q) || p.Remark.Contains(q) ||
                            p.OpenId.Contains(q));
            var pagedList = new PagedList<Order_Refund>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Order_Refund/Details/5
        [AuditFilter("请输入审计关键描述，比如“订单详细”", "eb26097d-7052-4a55-ae56-af26d0384c58")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Refund = await db.Order_Refunds.FindAsync(id);
            if (order_Refund == null)
                return HttpNotFound();
            return View(order_Refund);
        }

        // GET: Order_Refund/Create
        [AuditFilter("请输入审计关键描述，比如“订单待创建”", "991fec97-a13a-46be-ad49-c8532d302535")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order_Refund/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单创建”", "81d76a1a-89c8-4cbd-95df-ba833f4f47ac")]
        public async Task<ActionResult> Create(
            [Bind(
                 Include =
                     "Id,OrderID,OrderCode,Code,Type,Amount,Quantity,Reason,Logistics,ShippingCode,Consignee,Mobile,Address,State,Remark,OpenId,TenantId,CreateTime"
             )] Order_Refund order_Refund)
        {
            if (ModelState.IsValid)
            {
                order_Refund.Id = Guid.NewGuid();
                //SetModelWithChangeStates(order_Refund, default(Guid?));				
                db.Order_Refunds.Add(order_Refund);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order_Refund);
        }

        // GET: Order_Refund/Edit/5
        [AuditFilter("请输入审计关键描述，比如“订单待编辑”", "761ff1b3-657f-4f69-ad18-d26b7d5cefc6")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Refund = await db.Order_Refunds.FindAsync(id);
            if (order_Refund == null)
                return HttpNotFound();
            return View(order_Refund);
        }

        // POST: Order_Refund/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单编辑”", "855553b8-ea5b-445a-a9db-279526090cd0")]
        public async Task<ActionResult> Edit(
            [Bind(
                 Include =
                     "Id,OrderID,OrderCode,Code,Type,Amount,Quantity,Reason,Logistics,ShippingCode,Consignee,Mobile,Address,State,Remark,OpenId,TenantId,CreateTime"
             )] Order_Refund order_Refund)
        {
            if (ModelState.IsValid)
            {
                //SetModelWithChangeStates(order_Refund, order_Refund.Id);		
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order_Refund);
        }

        // GET: Order_Refund/Delete/5
        [AuditFilter("请输入审计关键描述，比如“订单待删除”", "bb354048-8d1a-4092-acf0-f54ce18ae48b")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Refund = await db.Order_Refunds.FindAsync(id);
            if (order_Refund == null)
                return HttpNotFound();
            return View(order_Refund);
        }

        // POST: Order_Refund/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单删除”", "49c2a41f-fef8-4581-a6e2-f35e465d0452")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var order_Refund = await db.Order_Refunds.FindAsync(id);
            db.Order_Refunds.Remove(order_Refund);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Order_Refund/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order_Refund/BatchOperation/{operation}")]
        [AuditFilter("请输入审计关键描述，比如“订单批量操作”", "a064daeb-d143-4078-ae58-294fa38bc5f3")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Order_Refunds.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Order_Refunds.RemoveRange(models);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}