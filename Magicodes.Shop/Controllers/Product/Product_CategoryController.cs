// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_CategoryController.cs
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
    [RoleMenuFilter("商品管理", "704B112A-3FF1-4985-ACED-611FF4DAC71B", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-shopping-cart")]
    public class Product_CategoryController : TenantBaseController<Product_Category>
    {
        [RoleMenuFilter("类目管理", "9BF2F838-ACF6-405D-BAFC-1944A412AAE3", "Admin,TenantManager,ShopManager",
             url: "/Product_Category", parentId: "704B112A-3FF1-4985-ACED-611FF4DAC71B")]
        // GET: Product_Category
        [AuditFilter("商品类目查询", "1fa3afec-c682-4e60-9c8b-bf22f9de640e")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Product_Categorys.Include(p => p.CreateUser).Include(p => p.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q) || p.Code.Contains(q));
            var pagedList = new PagedList<Product_Category>(
                await queryable.OrderBy(p => p.Sort)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Product_Category/Details/5
        [AuditFilter("商品类目详细信息", "19c45161-bfd8-49fb-ad2e-7ccf9b40efee")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Category =
                await db.Product_Categorys.Include(p => p.CreateUser).FirstOrDefaultAsync(p => p.Id == id);
            if (product_Category == null)
                return HttpNotFound();
            return View(product_Category);
        }

        // GET: Product_Category/Create
        [AuditFilter("商品类目待创建", "7e2d3115-7339-4d12-8855-a89c3d18801e")]
        public ActionResult Create(Guid? parentId)
        {
            ViewBag.ResourcesParentId =
                new SelectList(db.Product_Categorys.Where(p => p.IsDisplay).OrderBy(p => p.Sort).ToList(),
                    dataTextField: "Name", dataValueField: "Id", selectedValue: parentId);
            return View();
        }

        // POST: Product_Category/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品类目创建", "e47c6a54-05ca-40c9-a4ad-aaab3cd98f43")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,ParentId,Name,Code,Logo,Sort,IsDisplay,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId"
             )] Product_Category product_Category)
        {
            if (ModelState.IsValid)
            {
                product_Category.Id = Guid.NewGuid();
                SetModelWithChangeStates(product_Category, default(Guid?));
                db.Product_Categorys.Add(product_Category);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ResourcesParentId =
                new SelectList(db.Product_Categorys.Where(p => p.IsDisplay).OrderBy(p => p.Sort).ToList(),
                    dataTextField: "Name", dataValueField: "Id", selectedValue: product_Category.ParentId);
            return View(product_Category);
        }

        // GET: Product_Category/Edit/5
        [AuditFilter("商品类目待编辑", "cd3562ce-5015-47c4-8c4f-1a043d8834a3")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Category = await db.Product_Categorys.FindAsync(id);
            if (product_Category == null)            
                  return HttpNotFound();
            ViewBag.logo = product_Category.Logo;
            ViewBag.ResourcesParentId =
                new SelectList(db.Product_Categorys.Where(p => p.IsDisplay & (p.Id != product_Category.Id)).ToList(),
                    dataTextField: "Name", dataValueField: "Id", selectedValue: product_Category.ParentId);
            return View(product_Category);
        }

        // POST: Product_Category/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品类目编辑", "ab64734c-4ba3-4e86-a996-d7ddded3423c")]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,ParentId,Name,Code,Logo,Sort,IsDisplay,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId"
             )] Product_Category product_Category)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(product_Category, product_Category.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ResourcesParentId =
                new SelectList(db.Product_Categorys.Where(p => p.IsDisplay & (p.Id != product_Category.Id)).ToList(),
                    dataTextField: "Name", dataValueField: "Id", selectedValue: product_Category.ParentId);
            return View(product_Category);
        }

        // GET: Product_Category/Delete/5
        [AuditFilter("商品类目待删除", "b86fc4c6-0bf7-4064-8f7f-cc23b57a79d8")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Category = await db.Product_Categorys.FindAsync(id);
            if (product_Category == null)
                return HttpNotFound();
            return View(product_Category);
        }

        // POST: Product_Category/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品类目删除", "5a55b7b3-aa15-4f0c-a799-d130b3a3447e")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var product_Category = await db.Product_Categorys.FindAsync(id);
            db.Product_Categorys.Remove(product_Category);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Product_Category/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Product_Category/BatchOperation/{operation}")]
        [AuditFilter("商品类目批量操作", "f1925fd3-2925-40bc-b0ce-4c34207c64c5")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Product_Categorys.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Product_Categorys.RemoveRange(models);
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