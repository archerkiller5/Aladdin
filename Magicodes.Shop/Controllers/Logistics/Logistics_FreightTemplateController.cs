// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Logistics_FreightTemplateController.cs
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
    public class Logistics_FreightTemplateController : TenantBaseController<Logistics_FreightTemplate>
    {
        // GET: Logistics_FreightTemplate
        [RoleMenuFilter("运费模板设置", "FA585048-648B-4C51-B5DB-AEADB481707B", "Admin,TenantManager,ShopManager",
             url: "/Logistics_FreightTemplate", parentId: "FAE7C4E9-D710-4462-A6D5-539A66DD258B")]
        [AuditFilter("物流运费模板管理页面", "e71e3ad5-3022-4fa6-a409-239657269e77")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable =
                db.Logistics_FreightTemplates.Include(l => l.CreateUser).Include(l => l.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q) || p.Remark.Contains(q));
            var pagedList = new PagedList<Logistics_FreightTemplate>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Logistics_FreightTemplate/Details/5
        [AuditFilter("物流运费模板详情页", "a27d6627-db2f-4c0b-85de-4501c246b214")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_FreightTemplate =
                await
                    db.Logistics_FreightTemplates.Include(p => p.UpdateUser)
                        .Include(p => p.CreateUser)
                        .FirstOrDefaultAsync(p => p.Id == id);
            if (logistics_FreightTemplate == null)
                return HttpNotFound();
            return View(logistics_FreightTemplate);
        }

        // GET: Logistics_FreightTemplate/Create
        [AuditFilter("物流运费模板创建页", "f1eb8605-096a-4fa5-8679-cf218f153beb")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Logistics_FreightTemplate/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流运费模板创建", "47562d2c-3628-4f3f-b3ff-c1e260c3b13d")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Name,Price,Remark,IsDefault,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Logistics_FreightTemplate logistics_FreightTemplate)
        {
            if (ModelState.IsValid)
            {
                /*需要验证下只能有一个默认运费模板*/
                if (logistics_FreightTemplate.IsDefault)
                {
                    var Lst_Default = db.Logistics_FreightTemplates.Where(p => p.IsDefault).ToList();
                    if (Lst_Default.Count > 0)
                        Lst_Default.ForEach(p => { p.IsDefault = false; });
                }
                logistics_FreightTemplate.Id = Guid.NewGuid();
                SetModelWithChangeStates(logistics_FreightTemplate, default(Guid?));
                db.Logistics_FreightTemplates.Add(logistics_FreightTemplate);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(logistics_FreightTemplate);
        }

        // GET: Logistics_FreightTemplate/Edit/5
        [AuditFilter("物流运费模板编辑页面", "0756999f-3d53-4739-bad5-afa315e9857d")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_FreightTemplate = await db.Logistics_FreightTemplates.FindAsync(id);
            if (logistics_FreightTemplate == null)
                return HttpNotFound();
            return View(logistics_FreightTemplate);
        }

        // POST: Logistics_FreightTemplate/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流运费模板编辑", "ab899632-4265-4871-a727-6bcaf62df578")]
        //[Bind(Include = "Id,Name,Price,Remark,IsDefault,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] 
        public async Task<ActionResult> Edit(Logistics_FreightTemplate model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsDefault)
                {
                    var Lst_Default = db.Logistics_FreightTemplates.AsNoTracking().Where(p => p.IsDefault).ToList();
                    if (Lst_Default.Count > 0)
                        Lst_Default.ForEach(p => { p.IsDefault = false; });
                }
                SetModelWithChangeStates(model, model.Id);

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }


        // GET: Logistics_FreightTemplate/Delete/5
        [AuditFilter("物流运费模板删除页面", "33af929a-9db4-459d-b5e8-4cc62fb003e7")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_FreightTemplate = await db.Logistics_FreightTemplates.FindAsync(id);
            if (logistics_FreightTemplate == null)
                return HttpNotFound();
            return View(logistics_FreightTemplate);
        }

        // POST: Logistics_FreightTemplate/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流运费模板删除", "c52bd595-205d-4531-bd0e-d12bf9ae1019")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var logistics_FreightTemplate = await db.Logistics_FreightTemplates.FindAsync(id);
            db.Logistics_FreightTemplates.Remove(logistics_FreightTemplate);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Logistics_FreightTemplate/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Logistics_FreightTemplate/BatchOperation/{operation}")]
        [AuditFilter("物流运费模板批量操作", "07cfc93d-574a-4c74-ab54-8b225e88b863")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Logistics_FreightTemplates.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Logistics_FreightTemplates.RemoveRange(models);
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