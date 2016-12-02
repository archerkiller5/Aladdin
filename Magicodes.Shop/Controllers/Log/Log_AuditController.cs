// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Log_AuditController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:10
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
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Log
{
    [RoleMenuFilter("日志监控", "F348A29F-9F4C-476A-9932-23C607A4F9FB", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-code")]
    public class Log_AuditController : TenantBaseController<Log_Audit>
    {
        // GET: Log_Audit
        [AuditFilter("请输入审计关键描述，比如“订单查询”", "132a34c6-4d48-4d2f-bc23-b2bebee035ff")]
        [RoleMenuFilter("审计日志", "132a34c6-4d48-4d2f-bc23-b2bebee035ff", "Admin,TenantManager,ShopManager",
             url: "/Log_Audit", parentId: "F348A29F-9F4C-476A-9932-23C607A4F9FB")]
        public async Task<ActionResult> Index(string q, DateTime? sDatePicker, DateTime? eDatePicker, int pageIndex = 1,
            int pageSize = 10)
        {
            var queryable = db.Log_Audits.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.Code.Contains(q) || p.Remark.Contains(q) || p.RequestUrl.Contains(q) ||
                            p.FormData.Contains(q) || p.Title.Contains(q) || p.ClientIpAddress.Contains(q) ||
                            p.Exception.Contains(q) || p.BrowserInfo.Contains(q) || p.ClientName.Contains(q) ||
                            p.CustomData.Contains(q) || p.CreateBy.Contains(q));

            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            if ((sDatePicker == null) && (eDatePicker == null))
            {
                queryable = queryable.Where(p => (p.CreateTime >= startDate) && (p.CreateTime <= endDate));
            }
            else if ((sDatePicker != null) && (eDatePicker == null))
            {
                var start = new DateTime(sDatePicker.Value.Year, sDatePicker.Value.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);
                queryable = queryable.Where(p => (p.CreateTime >= sDatePicker) && (p.CreateTime <= end));
            }
            else if ((sDatePicker == null) && (eDatePicker != null))
            {
                var start = new DateTime(eDatePicker.Value.Year, eDatePicker.Value.Month, 1);
                queryable = queryable.Where(p => (p.CreateTime >= start) && (p.CreateTime <= eDatePicker));
            }
            else
            {
                queryable = queryable.Where(p => (p.CreateTime >= sDatePicker) && (p.CreateTime <= eDatePicker));
            }

            var pagedList = new PagedList<Log_Audit>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Log_Audit/Details/5
        [AuditFilter("请输入审计关键描述，比如“订单详细”", "f0a93fcd-04ba-44b2-ae04-4c6d7e54432b")]
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_Audit = await db.Log_Audits.FindAsync(id);
            if (log_Audit == null)
                return HttpNotFound();
            return View(log_Audit);
        }

        // GET: Log_Audit/Create
        [AuditFilter("请输入审计关键描述，比如“订单待创建”", "7f126374-8884-4ff0-96e9-cd3abda46878")]
        public ActionResult Create()
        {
            return View();
        }

        // GET: Log_Audit/Delete/5
        [HttpGet]
        [ActionName("Clean")]
        public async Task<ActionResult> Delete(string type)
        {
            try
            {
                var date = new DateTime();
                switch (type)
                {
                    case "oneYearBefore":
                        date = DateTime.Now.AddYears(-1);
                        break;
                    case "halfYearBefore":
                        date = DateTime.Now.AddMonths(-6);
                        break;
                }

                var list = await db.Log_Audits.Where(p => p.CreateTime <= date).ToListAsync();
                db.Log_Audits.RemoveRange(list);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Log_Audit/Delete/5
        [AuditFilter("请输入审计关键描述，比如“订单待删除”", "ef76aa05-04e6-4165-904f-8a8a3a5703ba")]
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_Audit = await db.Log_Audits.FindAsync(id);
            if (log_Audit == null)
                return HttpNotFound();
            return View(log_Audit);
        }

        // POST: Log_Audit/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("请输入审计关键描述，比如“订单删除”", "7ee9737f-98e2-4ba7-9169-c1959426c18e")]
        public async Task<ActionResult> DeleteConfirmed(long? id)
        {
            var log_Audit = await db.Log_Audits.FindAsync(id);
            db.Log_Audits.Remove(log_Audit);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Log_Audit/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Log_Audit/BatchOperation/{operation}")]
        [AuditFilter("请输入审计关键描述，比如“订单批量操作”", "341d31ad-efeb-40f3-815e-d0c0243a6bb1")]
        public async Task<ActionResult> BatchOperation(string operation, params long?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Log_Audits.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Log_Audits.RemoveRange(models);
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