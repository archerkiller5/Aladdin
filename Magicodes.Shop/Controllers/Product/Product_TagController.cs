// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_TagController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:11
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
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models.Product;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Product
{
    public class Product_TagController : TenantBaseController<Product_Tag>
    {
        [RoleMenuFilter("标签管理", "98F50AE3-F2FB-4855-BD64-685431330A1D", "Admin,TenantManager,ShopManager",
             url: "/Product_Tag", parentId: "704B112A-3FF1-4985-ACED-611FF4DAC71B")]
        // GET: Product_Tag
        [AuditFilter("商品标签查询", "d40ed781-d119-4783-89d6-69ebd790520f")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Product_Tags.Include(p => p.CreateUser).Include(p => p.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q));
            var pagedList = new PagedList<Product_Tag>(
                await queryable.OrderByDescending(p => p.CreateTime)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Product_Tag/Details/5
        [AuditFilter("商品标签详细", "24afc28b-ab87-4bab-9e64-6989a5a5dfe9")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Tag = await db.Product_Tags.FindAsync(id);
            if (product_Tag == null)
                return HttpNotFound();
            return View(product_Tag);
        }

        // GET: Product_Tag/Create
        [AuditFilter("商品标签待创建", "e90a3385-459c-4689-a2c8-d913fd547daa")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product_Tag/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品标签创建", "c45e1ad6-763a-4ef5-8ceb-b33d3e2b80b1")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Name,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Product_Tag product_Tag)
        {
            if (ModelState.IsValid)
            {
                product_Tag.Id = Guid.NewGuid();
                SetModelWithChangeStates(product_Tag, default(Guid?));
                db.Product_Tags.Add(product_Tag);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product_Tag);
        }

        // GET: Product_Tag/Edit/5
        [AuditFilter("商品标签待编辑", "ae83800c-dec3-42e2-938d-1733378a6162")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Tag = await db.Product_Tags.FindAsync(id);
            if (product_Tag == null)
                return HttpNotFound();
            return View(product_Tag);
        }

        // POST: Product_Tag/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品标签编辑", "1b90b752-8da8-4da8-b37a-36c7e56b6b25")]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Name,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Product_Tag product_Tag)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(product_Tag, product_Tag.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product_Tag);
        }

        // GET: Product_Tag/Delete/5
        [AuditFilter("商品标签待删除", "a3407cf3-8080-44ec-b5c8-eff41172a858")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Tag = await db.Product_Tags.FindAsync(id);
            if (product_Tag == null)
                return HttpNotFound();
            return View(product_Tag);
        }

        // POST: Product_Tag/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品标签删除", "abfb9d40-f743-4117-b487-51c429e5377b")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var product_Tag = await db.Product_Tags.FindAsync(id);
            db.Product_Tags.Remove(product_Tag);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Product_Tag/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Product_Tag/BatchOperation/{operation}")]
        [AuditFilter("商品标签批量操作", "0f8173dc-3ce1-407c-af9f-3a3be8cdc3b1")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Product_Tags.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Product_Tags.RemoveRange(models);
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

        [HttpPost]
        [AuditFilter("商品标签异步创建", "702A0F60-AEAC-4CD7-BF4E-29FE9B02E413")]
        public async Task<ActionResult> CreateTag([Bind(Include = "Name")] Product_Tag product_Tag)
        {
            var ajaxResponse = new AjaxResponse { Success = true, Message = "添加成功！" };
            if (ModelState.IsValid)
            {
                product_Tag.Id = Guid.NewGuid();
                SetModelWithChangeStates(product_Tag, default(Guid));
                db.Product_Tags.Add(product_Tag);
                await db.SaveChangesAsync();
            }
            return Json(ajaxResponse);
        }

        [HttpPost]
        [AuditFilter("商品标签异步编辑", "09DE11EE-D4B4-4945-B860-E5427268E0CC")]
        public async Task<ActionResult> UpdateTag([Bind(Include = "Id,Name")] Product_Tag product_Tag)
        {
            var ajaxResponse = new AjaxResponse { Success = true, Message = "修改成功！" };
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(product_Tag, product_Tag.Id);
                await db.SaveChangesAsync();
            }
            return Json(ajaxResponse);
        }

        [HttpPost]
        [AuditFilter("商品标签异步删除", "8F50CFEE-4297-4E16-AD29-2679399B3B89")]
        public async Task<ActionResult> DeleteTag(Guid? Id)
        {
            var ajaxResponse = new AjaxResponse { Success = true, Message = "删除成功！" };
            var product_Tag = await db.Product_Tags.FindAsync(Id);
            db.Product_Tags.Remove(product_Tag);
            await db.SaveChangesAsync();
            return Json(ajaxResponse);
        }
    }
}