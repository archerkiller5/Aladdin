// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_RefundDetailController.cs
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
    public class Order_RefundDetailController : TenantBaseController<Order_RefundDetail>
    {
        // GET: Order_RefundDetail
        [AuditFilter("请输入审计关键描述，比如“订单查询”", "3dfd5074-451b-4b8b-9954-12623533afc9")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Order_RefundDetails.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.OpenId.Contains(q));
            var pagedList = new PagedList<Order_RefundDetail>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Order_RefundDetail/Details/5
        [AuditFilter("请输入审计关键描述，比如“订单详细”", "67ff4fc2-cb81-4992-bfc7-82825e1e0765")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_RefundDetail = await db.Order_RefundDetails.FindAsync(id);
            if (order_RefundDetail == null)
                return HttpNotFound();
            return View(order_RefundDetail);
        }

        // GET: Order_RefundDetail/Create
        [AuditFilter("请输入审计关键描述，比如“订单待创建”", "6cd0e736-980b-4357-8c9d-d87b878c1365")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order_RefundDetail/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单创建”", "0c064170-30ad-4086-819e-670e2e9fa74f")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,OrderRefund,OrderDetail,OpenId,TenantId,CreateTime")] Order_RefundDetail
                order_RefundDetail)
        {
            if (ModelState.IsValid)
            {
                order_RefundDetail.Id = Guid.NewGuid();
                //SetModelWithChangeStates(order_RefundDetail, default(Guid?));				
                db.Order_RefundDetails.Add(order_RefundDetail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order_RefundDetail);
        }

        // GET: Order_RefundDetail/Edit/5
        [AuditFilter("请输入审计关键描述，比如“订单待编辑”", "4c7e04d2-0484-4e67-b27d-ac51ba5fcb80")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_RefundDetail = await db.Order_RefundDetails.FindAsync(id);
            if (order_RefundDetail == null)
                return HttpNotFound();
            return View(order_RefundDetail);
        }

        // POST: Order_RefundDetail/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单编辑”", "b954b481-ec68-4f98-a678-d3b737af8844")]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,OrderRefund,OrderDetail,OpenId,TenantId,CreateTime")] Order_RefundDetail
                order_RefundDetail)
        {
            if (ModelState.IsValid)
            {
                //SetModelWithChangeStates(order_RefundDetail, order_RefundDetail.Id);		
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order_RefundDetail);
        }

        // GET: Order_RefundDetail/Delete/5
        [AuditFilter("请输入审计关键描述，比如“订单待删除”", "ef863d53-3654-4dce-ad2a-9705796d2dd5")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_RefundDetail = await db.Order_RefundDetails.FindAsync(id);
            if (order_RefundDetail == null)
                return HttpNotFound();
            return View(order_RefundDetail);
        }

        // POST: Order_RefundDetail/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单删除”", "d0b962b9-cc21-4edf-8775-661331817b36")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var order_RefundDetail = await db.Order_RefundDetails.FindAsync(id);
            db.Order_RefundDetails.Remove(order_RefundDetail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Order_RefundDetail/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order_RefundDetail/BatchOperation/{operation}")]
        [AuditFilter("请输入审计关键描述，比如“订单批量操作”", "1f3d81c7-5ef7-4c49-a968-4dd8c4f7a1ea")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Order_RefundDetails.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Order_RefundDetails.RemoveRange(models);
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