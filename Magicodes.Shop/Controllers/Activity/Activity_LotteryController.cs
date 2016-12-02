using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Activity;

namespace Magicodes.Shop.Controllers.Activity
{
    public class Activity_LotteryController : TenantBaseController<Activity_Lottery>
    {

        // GET: Activity_Lottery
        [AuditFilter("抽奖查询", "514b18eb-f562-47dd-9f47-63b885788be1")]
        [RoleMenuFilter("抽奖管理", "{1FB7CFEF-1640-4A1A-BDBA-451164DA03AD}", "Admin,TenantManager,ShopManager",
        parentId: "{8A337252-14B3-47A3-85BF-48FE964F88D2}", url: "/Activity_Lottery")]
        public ActionResult Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Activity_Lotteries.Include(a => a.CreateUser).Include(a => a.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                //请替换为相应的搜索逻辑
                queryable = queryable.Where(p => p.WinningTips.Contains(q) || p.Title.Contains(q) || p.Description.Contains(q));
            }
            var pagedList = new PagedList<Activity_Lottery>(
                             queryable.OrderBy(p => p.Id)
                             .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                             pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Activity_Lottery/Details/5
        [AuditFilter("抽奖详细", "536ad925-ae71-40d6-b7ab-2fd00400fda7")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity_Lottery activity_Lottery = db.Activity_Lotteries.Find(id);
            if (activity_Lottery == null)
            {
                return HttpNotFound();
            }
            return View(activity_Lottery);
        }

        // GET: Activity_Lottery/Create
        [AuditFilter("抽奖待创建”", "503902d7-29c3-46fe-a901-950f3b3a285b")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Activity_Lottery/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("创建抽奖", "7c9a6deb-c20e-42ce-8750-6501e4b9fdf8")]
        public ActionResult Create([Bind(Include = "Id,LotteryType,WinningTips,ExpectedNumber,AllowNumberPerPerson,AllowNumberPerDay,Title,StartTime,EndTime,Description,IsEnable,CreateTime,CreateBy,UpdateTime,UpdateBy,TenantId")] Activity_Lottery activity_Lottery)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(activity_Lottery, default(int?));
                db.Activity_Lotteries.Add(activity_Lottery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activity_Lottery);
        }

        // GET: Activity_Lottery/Edit/5
        [AuditFilter("抽奖待编辑", "bad06c3e-946c-40ae-9940-d1d851a3b557")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity_Lottery activity_Lottery = db.Activity_Lotteries.Find(id);
            if (activity_Lottery == null)
            {
                return HttpNotFound();
            }
            return View(activity_Lottery);
        }

        // POST: Activity_Lottery/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("抽奖编辑", "c0b6c554-d120-43b9-b463-a1bbd7b83bdb")]
        public ActionResult Edit([Bind(Include = "Id,LotteryType,WinningTips,ExpectedNumber,AllowNumberPerPerson,AllowNumberPerDay,Title,StartTime,EndTime,Description,IsEnable,CreateTime,CreateBy,UpdateTime,UpdateBy,TenantId")] Activity_Lottery activity_Lottery)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(activity_Lottery, activity_Lottery.Id);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activity_Lottery);
        }

        // GET: Activity_Lottery/Delete/5
        [AuditFilter("抽奖待删除", "88442431-f232-485c-927b-8ba0871855ac")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity_Lottery activity_Lottery = db.Activity_Lotteries.Find(id);
            if (activity_Lottery == null)
            {
                return HttpNotFound();
            }
            return View(activity_Lottery);
        }

        // POST: Activity_Lottery/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("删除抽奖", "ce4b3664-82f3-4e68-94b3-29e368840a75")]
        public ActionResult DeleteConfirmed(int? id)
        {
            Activity_Lottery activity_Lottery = db.Activity_Lotteries.Find(id);
            db.Activity_Lotteries.Remove(activity_Lottery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Activity_Lottery/BatchOperation/{operation}
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Activity_Lottery/BatchOperation/{operation}")]
        [AuditFilter("抽奖批量操作", "17657e0c-3ea9-4260-a1e9-873cff9bf585")]
        public ActionResult BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = db.Activity_Lotteries.Where(p => ids.Contains(p.Id)).ToList();
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
                                db.Activity_Lotteries.RemoveRange(models);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
