// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_AttributeController.cs
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
using Magicodes.Mvc.AuditFilter;using Magicodes.Mvc.RoleMenuFilter;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Product
{
    public class Product_AttributeController : TenantBaseController<Product_Attribute>
    {
        //[RoleMenuFilter(title: "商品属性管理", id: "E2508422-99E7-4E8B-8E9D-4A63BA78504F", roleNames: "Admin,TenantManager,ShopManager", url: "/Product_Attribute", parentId: "704B112A-3FF1-4985-ACED-611FF4DAC71B")]
        // GET: Product_Attribute
        [AuditFilter("商品属性查询”", "2eaa292a-1c25-4ee8-9329-8b4a015d613b")]
        public async Task<ActionResult> Index(string q, Guid? typeId, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Product_Attributes.Include(p => p.CreateUser).Include(p => p.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q));
            if (typeId.HasValue)
            {
                queryable = queryable.Where(p => p.TypeId == typeId);
                var type = db.Product_Types.FirstOrDefault(p => p.Id == typeId);
                ViewBag.TypeName = type.Name;
                ViewBag.TypeId = type.Id;
            }
            var pagedList = new PagedList<Product_Attribute>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Product_Attribute/Details/5
        [AuditFilter("商品属性详细", "6f880487-da89-4a62-af9e-f8625628ae3f")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Attribute = await db.Product_Attributes.FindAsync(id);
            if (product_Attribute == null)
                return HttpNotFound();
            return View(product_Attribute);
        }

        // GET: Product_Attribute/Create
        [AuditFilter("商品属性待创建", "631f2c1e-7538-4748-a308-393a19517bc4")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product_Attribute/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品属性创建", "fe50ea52-307b-4e37-b1e0-67053043d22f")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,TypeId,Name,Sort,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Product_Attribute
                product_Attribute)
        {
            if (ModelState.IsValid)
            {
                product_Attribute.Id = Guid.NewGuid();
                SetModelWithChangeStates(product_Attribute, default(Guid?));
                db.Product_Attributes.Add(product_Attribute);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product_Attribute);
        }

        // GET: Product_Attribute/Edit/5
        [AuditFilter("商品属性待编辑", "a23e6f74-19c6-483d-b016-aeaeaf3a4eaf")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Attribute = await db.Product_Attributes.FindAsync(id);
            if (product_Attribute == null)
                return HttpNotFound();
            return View(product_Attribute);
        }

        // POST: Product_Attribute/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品属性编辑", "b1aa19f4-7f11-490e-ae5c-4b6710da6977")]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,TypeId,Name,Sort,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Product_Attribute
                product_Attribute)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(product_Attribute, product_Attribute.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product_Attribute);
        }

        // GET: Product_Attribute/Delete/5
        [AuditFilter("商品属性待删除", "1746296c-86e0-4a08-8190-e8a5ca62b230")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Attribute = await db.Product_Attributes.FindAsync(id);
            if (product_Attribute == null)
                return HttpNotFound();
            return View(product_Attribute);
        }

        // POST: Product_Attribute/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品属性删除", "73262c25-f3df-45a4-a1fc-055d61006fe7")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var product_Attribute = await db.Product_Attributes.FindAsync(id);
            db.Product_Attributes.Remove(product_Attribute);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Product_Attribute/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Product_Attribute/BatchOperation/{operation}")]
        [AuditFilter("商品属性批量操作", "54e600b9-3266-464f-982f-5e7681b2c6fc")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Product_Attributes.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.Product_Attributes.RemoveRange(models);
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
        [AuditFilter("商品属性异步创建", "97754EF6-3B81-4AAE-B525-143ED00CCF44")]
        public async Task<ActionResult> CreateAttribute(
            [Bind(Include = "Name,TypeId")] Product_Attribute product_Attribute)
        {
            var ajaxResponse = new AjaxResponse {Success = true, Message = "添加成功！"};
            if (ModelState.IsValid)
            {
                product_Attribute.Id = Guid.NewGuid();
                SetModelWithChangeStates(product_Attribute, default(Guid));
                db.Product_Attributes.Add(product_Attribute);
                await db.SaveChangesAsync();
            }
            return Json(ajaxResponse);
        }

        [HttpPost]
        [AuditFilter("商品属性异步编辑", "AC5C66FE-051B-4914-B4BD-1459761F58C8")]
        public async Task<ActionResult> UpdateAttribute(
            [Bind(Include = "Id,Name,TypeId")] Product_Attribute product_Attribute)
        {
            var ajaxResponse = new AjaxResponse {Success = true, Message = "修改成功！"};
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(product_Attribute, product_Attribute.Id);
                await db.SaveChangesAsync();
            }
            return Json(ajaxResponse);
        }

        [HttpPost]
        [AuditFilter("商品属性异步删除", "450A39C9-2949-460F-B0C1-AF075C27A574")]
        public async Task<ActionResult> DeleteAttribute(Guid? Id)
        {
            var ajaxResponse = new AjaxResponse {Success = true, Message = "删除成功！"};
            var product_Attribute = await db.Product_Attributes.FindAsync(Id);
            db.Product_Attributes.Remove(product_Attribute);
            await db.SaveChangesAsync();
            return Json(ajaxResponse);
        }
    }
}