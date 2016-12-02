// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_TypeController.cs
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
using Magicodes.WeiChat.Data.Models.Product;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Product
{
    public class Product_TypeController : TenantBaseController<Product_Type>
    {
        [RoleMenuFilter("属性类别管理", "78551480-12BD-4CF6-BB58-9334600D7B45", "Admin,TenantManager,ShopManager",
             url: "/Product_Type", parentId: "704B112A-3FF1-4985-ACED-611FF4DAC71B")]
        // GET: Product_Type
        [AuditFilter("商品类别查询", "e959454f-0f86-47f2-b6ed-49c59e6d78df")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Product_Types.Include(p => p.CreateUser).Include(p => p.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q) || p.Remark.Contains(q));
            var pagedList = new PagedList<Product_Type>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Product_Type/Details/5
        [AuditFilter("商品类别详细", "0d923506-5457-46bc-a49b-9702c82c50a3")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Type = await db.Product_Types.Include(p => p.CreateUser).FirstOrDefaultAsync(p => p.Id == id);
            if (product_Type == null)
                return HttpNotFound();
            return View(product_Type);
        }

        // GET: Product_Type/Create
        [AuditFilter("商品类别待创建", "d38ca88b-e018-44d2-b4a5-fd1d252b4a02")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product_Type/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品类别创建", "82d2cc1b-eaea-49cc-a0a8-faec07d237a1")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Name,Remark,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Product_Type
                product_Type)
        {
            if (ModelState.IsValid)
            {
                product_Type.Id = Guid.NewGuid();
                SetModelWithChangeStates(product_Type, default(Guid?));
                db.Product_Types.Add(product_Type);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product_Type);
        }

        // GET: Product_Type/Edit/5
        [AuditFilter("商品类别待编辑", "b64c753c-f532-43cc-a46d-16ef32238d69")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Type = await db.Product_Types.FindAsync(id);
            if (product_Type == null)
                return HttpNotFound();
            return View(product_Type);
        }

        // POST: Product_Type/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品类别编辑", "99fdbdf1-4511-4b86-b2e1-cc1b814474ed")]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Name,Remark,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Product_Type
                product_Type)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(product_Type, product_Type.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product_Type);
        }

        // GET: Product_Type/Delete/5
        [AuditFilter("商品类别待删除", "b4d0a15d-edd7-4b60-bdfb-d283f6b8983f")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Type = await db.Product_Types.FindAsync(id);
            if (product_Type == null)
                return HttpNotFound();
            return View(product_Type);
        }

        // POST: Product_Type/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品类别删除", "f8dc3d77-4deb-4793-9be6-b88989917338")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var product_Type = await db.Product_Types.FindAsync(id);
            db.Product_Types.Remove(product_Type);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Product_Type/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Product_Type/BatchOperation/{operation}")]
        [AuditFilter("商品类别批量操作", "dec2bc6f-fae1-42d2-9314-e8913a0739bc")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Product_Types.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Product_Types.RemoveRange(models);
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

        [AuditFilter("商品属性查询”", "2eaa292a-1c25-4ee8-9329-8b4a015d613b")]
        public async Task<ActionResult> AttributeIndex(Guid? typeId, int pageIndex = 1, int pageSize = 100)
        {
            var queryable = db.Product_Attributes.Include(p => p.CreateUser).Include(p => p.UpdateUser).AsQueryable();
            if (typeId.HasValue)
            {
                queryable = queryable.Where(p => p.TypeId == typeId);
                var type = db.Product_Types.FirstOrDefault(p => p.Id == typeId);
                ViewBag.TypeName = type.Name;
                ViewBag.TypeId = type.Id;
            }
            var pagedList = new PagedList<Product_Attribute>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }
    }
}