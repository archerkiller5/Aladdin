// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Order_DetailController.cs
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
    public class Order_DetailController : TenantBaseController<Order_Detail>
    {
        // GET: Order_Detail
        [AuditFilter("订单查询", "91ad0293-4cae-402a-b948-160032afba2d")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Order_Details.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(p => p.ProductName.Contains(q) || p.ProductImage.Contains(q) || p.OpenId.Contains(q));
            var pagedList = new PagedList<Order_Detail>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());


            return View(pagedList);
        }

        // GET: Order_Detail/Details/5
        [AuditFilter("请输入审计关键描述，比如“订单详细”", "0ea68750-112e-42d7-b502-1da4e732dd87")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Detail = await db.Order_Details.FindAsync(id);
            if (order_Detail == null)
                return HttpNotFound();
            return View(order_Detail);
        }

        // GET: Order_Detail/Create
        [AuditFilter("请输入审计关键描述，比如“订单待创建”", "b749f427-67b4-4381-96d0-e264887a14b9")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Order_Detail/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单创建”", "e11fed77-785d-4fa4-a523-c4a0c1192ed5")]
        public async Task<ActionResult> Create(
            [Bind(
                 Include =
                     "Id,OrderID,ProductName,ProductImage,ProductID,PackageID,Price,Quantity,OpenId,TenantId,CreateTime"
             )] Order_Detail order_Detail)
        {
            if (ModelState.IsValid)
            {
                order_Detail.Id = Guid.NewGuid();
                //SetModelWithChangeStates(order_Detail, default(Guid?));				
                db.Order_Details.Add(order_Detail);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order_Detail);
        }

        // GET: Order_Detail/Edit/5
        [AuditFilter("请输入审计关键描述，比如“订单待编辑”", "e74b883f-9f9d-48b6-9d11-ac6cf7897f26")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Detail = await db.Order_Details.FindAsync(id);
            if (order_Detail == null)
                return HttpNotFound();
            return View(order_Detail);
        }

        // POST: Order_Detail/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单编辑”", "cfc451aa-068b-4e23-a4bc-142c0cd2ea29")]
        public async Task<ActionResult> Edit(
            [Bind(
                 Include =
                     "Id,OrderID,ProductName,ProductImage,ProductID,PackageID,Price,Quantity,OpenId,TenantId,CreateTime"
             )] Order_Detail order_Detail)
        {
            if (ModelState.IsValid)
            {
                //SetModelWithChangeStates(order_Detail, order_Detail.Id);		
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order_Detail);
        }

        // GET: Order_Detail/Delete/5
        [AuditFilter("请输入审计关键描述，比如“订单待删除”", "628f67c4-8013-4e86-8980-51811c0f46fa")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order_Detail = await db.Order_Details.FindAsync(id);
            if (order_Detail == null)
                return HttpNotFound();
            return View(order_Detail);
        }

        // POST: Order_Detail/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单删除”", "a3ff4b64-4625-4009-9a02-545c77f697a6")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var order_Detail = await db.Order_Details.FindAsync(id);
            db.Order_Details.Remove(order_Detail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Order_Detail/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order_Detail/BatchOperation/{operation}")]
        [AuditFilter("请输入审计关键描述，比如“订单批量操作”", "413ac691-a7de-4c87-82eb-c452941a8a1d")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Order_Details.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Order_Details.RemoveRange(models);
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