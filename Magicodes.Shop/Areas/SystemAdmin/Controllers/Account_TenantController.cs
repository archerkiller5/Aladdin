// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Account_TenantController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:02
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
using Magicodes.Shop.Helpers;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.Logging;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Areas.SystemAdmin.Controllers
{
    [RoleMenuFilter("租户管理", "A1199F15-8859-425D-9148-B0D85F4272F2", "Admin", iconCls: "fa fa-users",
         tag: "System")]
    public class Account_TenantController : SystemAdminBase<WeiChat_App, int>
    {
        [AuditFilter("租户管理", "FE97493C-8EFE-417B-AEF1-B937B97F4B75")]
        [RoleMenuFilter("租户管理", "FE97493C-8EFE-417B-AEF1-B937B97F4B75", "Admin", url: "/SystemAdmin/Account_Tenant",
             parentId: "A1199F15-8859-425D-9148-B0D85F4272F2", tag: "System")]
        // GET: SystemAdmin/Account_Tenant1
        public ActionResult Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Admin_Tenants.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q) || p.Remark.Contains(q));
            var pagedList = new PagedList<Admin_Tenant>(
                queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        [AuditFilter("租户详情", "AF8749BC-FC69-49ED-83E2-23BE4B731BD7")]
        // GET: SystemAdmin/Account_Tenant1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var account_Tenant = db.Admin_Tenants.Find(id);
            if (account_Tenant == null)
                return HttpNotFound();
            return View(account_Tenant);
        }

        // GET: SystemAdmin/Account_Tenant1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemAdmin/Account_Tenant1/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IsSystemTenant,Name,Remark")] Admin_Tenant account_Tenant)
        {
            if (ModelState.IsValid)
            {
                db.Admin_Tenants.Add(account_Tenant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account_Tenant);
        }

        // GET: SystemAdmin/Account_Tenant1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var account_Tenant = db.Admin_Tenants.Find(id);
            if (account_Tenant == null)
                return HttpNotFound();
            return View(account_Tenant);
        }

        // POST: SystemAdmin/Account_Tenant1/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Remark")] Admin_Tenant account_Tenant)
        {
            if (ModelState.IsValid)
            {
                var model = db.Admin_Tenants.Find(account_Tenant.Id);
                model.Name = account_Tenant.Name;
                model.Remark = account_Tenant.Remark;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account_Tenant);
        }

        // GET: SystemAdmin/Account_Tenant1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var account_Tenant = db.Admin_Tenants.Find(id);
            if (account_Tenant == null)
                return HttpNotFound();
            if (account_Tenant.IsSystemTenant)
                throw new Exception("系统租户无法删除！");
            return View(account_Tenant);
        }

        // POST: SystemAdmin/Account_Tenant1/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            var account_Tenant = db.Admin_Tenants.Find(id);
            db.Admin_Tenants.Remove(account_Tenant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: SystemAdmin/Account_Tenant1/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("SystemAdmin/Account_Tenant/BatchOperation/{operation}")]
        public ActionResult BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = db.Admin_Tenants.Where(p => ids.Contains(p.Id)).ToList();
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
                                db.Admin_Tenants.RemoveRange(models);
                                db.SaveChanges();
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

        public ActionResult WeiChatAppConfig(int tenantId)
        {
            var model = db.WeiChat_Apps.FirstOrDefault(p => p.TenantId == tenantId) ?? new WeiChat_App
            {
                TenantId = tenantId,
                Token = Guid.NewGuid().ToString("N")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> WeiChatAppConfig(WeiChat_App model)
        {
            if (!ModelState.IsValid) return View(model);
            var isAdd = !db.WeiChat_Apps.Any(p => p.TenantId == model.TenantId);
            try
            {
                SaveModel(model, isAdd, model.TenantId);
                db.SaveChanges();
                WeChatConfigManager.Current.RefreshConfigAndAccessToken(model.TenantId, model);
                //var syncHelper = new SyncHelper(model.TenantId);
                //if (Request.Form["Sync_WeiChat_UserGroup"] == "1")
                //    await syncHelper.Sync(WeiChat_SyncTypes.Sync_WeiChat_UserGroup, true, UserId);
                //if (Request.Form["Sync_WeiChat_User"] == "1")
                //    await syncHelper.Sync(WeiChat_SyncTypes.Sync_WeiChat_User, true, UserId);
                //if (Request.Form["Sync_MKF"] == "1")
                //    await syncHelper.Sync(WeiChat_SyncTypes.Sync_MKF, true, UserId);
                //if (Request.Form["Sync_MessagesTemplates"] == "1")
                //    await syncHelper.Sync(WeiChat_SyncTypes.Sync_MessagesTemplates, true, UserId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // LogManager.GetCurrentClassLogger().LogException(ex);
                Loggers.Current.DefaultLogger.LogException(ex);
                ModelState.AddModelError("", "配置信息有误，请检查配置！");

                return View(model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}