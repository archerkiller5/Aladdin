// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_LogisticsController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
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
    public class Order_LogisticsController : TenantBaseController<Order_Logistics>
    {
        // GET: Order_Logistics
        [AuditFilter("请输入审计关键描述，比如“订单查询”", "135bedb6-1b80-4582-83f9-af61280697fe")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Order_Logistics.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.Consignee.Contains(q) || p.Province.Contains(q) || p.City.Contains(q) ||
                            p.Area.Contains(q) || p.Address.Contains(q) || p.Mobile.Contains(q) ||
                            p.Logistics.Contains(q) || p.ShippingCode.Contains(q) || p.OpenId.Contains(q));
            var pagedList = new PagedList<Order_Logistics>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Order_Logistics/Details/5
        [AuditFilter("请输入审计关键描述，比如“订单详细”", "de8e597e-a587-490e-8fa0-38e96337ff71")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Logistics = await db.Order_Logistics.FindAsync(id);
            if (order_Logistics == null)
                return HttpNotFound();
            return View(order_Logistics);
        }

        // GET: Order_Logistics/Create
        [AuditFilter("请输入审计关键描述，比如“订单待创建”", "762a5622-c471-4e29-a0ad-c3b0221b20af")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order_Logistics/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单创建”", "2f662bae-1288-497b-9212-3b57c153ce0f")]
        public async Task<ActionResult> Create(
            [Bind(
                 Include =
                     "Id,OrderID,Consignee,Province,City,Area,Address,Mobile,Logistics,ShippingCode,OpenId,TenantId,CreateTime"
             )] Order_Logistics order_Logistics)
        {
            if (ModelState.IsValid)
            {
                order_Logistics.Id = Guid.NewGuid();
                //SetModelWithChangeStates(order_Logistics, default(Guid?));				
                db.Order_Logistics.Add(order_Logistics);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order_Logistics);
        }

        // GET: Order_Logistics/Edit/5
        [AuditFilter("请输入审计关键描述，比如“订单待编辑”", "3e7ec5e8-5fa3-450e-9339-60689b390e5d")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Logistics = await db.Order_Logistics.FindAsync(id);
            if (order_Logistics == null)
                return HttpNotFound();
            return View(order_Logistics);
        }

        // POST: Order_Logistics/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单编辑”", "2693665c-b97b-4097-be4a-76ea891dd738")]
        public async Task<ActionResult> Edit(
            [Bind(
                 Include =
                     "Id,OrderID,Consignee,Province,City,Area,Address,Mobile,Logistics,ShippingCode,OpenId,TenantId,CreateTime"
             )] Order_Logistics order_Logistics)
        {
            if (ModelState.IsValid)
            {
                //SetModelWithChangeStates(order_Logistics, order_Logistics.Id);		
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order_Logistics);
        }

        // GET: Order_Logistics/Delete/5
        [AuditFilter("请输入审计关键描述，比如“订单待删除”", "079e1659-50fc-486c-8de0-4a443e78ad25")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Logistics = await db.Order_Logistics.FindAsync(id);
            if (order_Logistics == null)
                return HttpNotFound();
            return View(order_Logistics);
        }

        // POST: Order_Logistics/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单删除”", "30fc1ed1-0f60-4787-8c60-f3944cf497bf")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var order_Logistics = await db.Order_Logistics.FindAsync(id);
            db.Order_Logistics.Remove(order_Logistics);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Order_Logistics/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order_Logistics/BatchOperation/{operation}")]
        [AuditFilter("请输入审计关键描述，比如“订单批量操作”", "874df93b-d92f-4000-8111-22c1cdf575e8")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Order_Logistics.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Order_Logistics.RemoveRange(models);
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