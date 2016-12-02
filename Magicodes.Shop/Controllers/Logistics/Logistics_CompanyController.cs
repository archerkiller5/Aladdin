// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Logistics_CompanyController.cs
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
using Magicodes.WeiChat.Data.Models.Logistics;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Logistics
{
    [RoleMenuFilter("物流管理", "FAE7C4E9-D710-4462-A6D5-539A66DD258B", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-send")]
    public class Logistics_CompanyController : TenantBaseController<Logistics_Company>
    {
        // GET: Logistics_Company
        [RoleMenuFilter("物流公司设置", "B77F1E72-ED8C-4D97-8A8E-AF9BBDF446BD", "Admin,TenantManager,ShopManager",
             url: "/Logistics_Company", parentId: "FAE7C4E9-D710-4462-A6D5-539A66DD258B")]
        [AuditFilter("物流公司管理页面”", "15fae918-d0fb-4754-8a89-73f19bb656bb")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable =
                db.Logistics_Companys.Where(p => p.IsDelete == false)
                    .Include(l => l.CreateUser)
                    .Include(l => l.UpdateUser)
                    .AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q) || p.Remark.Contains(q) || p.ApiCom.Contains(q));
            var pagedList = new PagedList<Logistics_Company>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Logistics_Company/Details/5
        [AuditFilter("物流公司详情页面", "276db66a-5db2-447e-b27c-f3ce03296814")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_Company =
                await
                    db.Logistics_Companys.Include(p => p.UpdateUser)
                        .Include(p => p.CreateUser)
                        .FirstOrDefaultAsync(p => p.Id == id);
            if (logistics_Company == null)
                return HttpNotFound();
            return View(logistics_Company);
        }

        // GET: Logistics_Company/Create
        [AuditFilter("物流公司创建页面", "78a5c871-5c15-499c-a346-1505c4e6f0cc")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Logistics_Company/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流公司创建", "ff8c5aca-c483-41a5-89cb-dfa69130c11e")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Name,Remark,ApiCom,IsDelete,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Logistics_Company logistics_Company)
        {
            if (ModelState.IsValid)
            {
                logistics_Company.Id = Guid.NewGuid();
                SetModelWithChangeStates(logistics_Company, default(Guid?));
                db.Logistics_Companys.Add(logistics_Company);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(logistics_Company);
        }

        // GET: Logistics_Company/Edit/5
        [AuditFilter("物流公司编辑页面", "00f394c3-1045-4699-a528-22f2e1629eee")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_Company = await db.Logistics_Companys.FindAsync(id);
            if (logistics_Company == null)
                return HttpNotFound();
            return View(logistics_Company);
        }

        // POST: Logistics_Company/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流公司编辑", "5c82d339-65b8-4d11-ba18-de806094ba95")]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Name,Remark,ApiCom,IsDelete,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Logistics_Company logistics_Company)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(logistics_Company, logistics_Company.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(logistics_Company);
        }

        // GET: Logistics_Company/Delete/5
        [AuditFilter("物流公司删除页面", "0c15b3ad-35ad-4287-8a42-ed2548c55a66")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_Company = await db.Logistics_Companys.FindAsync(id);
            if (logistics_Company == null)
                return HttpNotFound();
            return View(logistics_Company);
        }

        // POST: Logistics_Company/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流公司删除", "29f4bec4-5b23-4a86-ade3-098946a8b9d8")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var logistics_Company = await db.Logistics_Companys.FindAsync(id);
            db.Logistics_Companys.Remove(logistics_Company);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Logistics_Company/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Logistics_Company/BatchOperation/{operation}")]
        [AuditFilter("物流公司批量操作", "45733f3b-847f-46e9-857e-75298a379182")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Logistics_Companys.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Logistics_Companys.RemoveRange(models);
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