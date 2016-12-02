// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Log_WithdrawController.cs
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
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models.Log;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Log
{
    public class Log_WithdrawController : TenantBaseController<Log_Withdraw>
    {
        [RoleMenuFilter("提现日志", "{B59A9957-0E60-41D7-A000-34ED57B54B7B}", "Admin,TenantManager,ShopManager",
             url: "/Log_Withdraw", parentId: "{61C8B1C8-354B-4D1A-A29E-1488DF254319}")]
        // GET: Log_Withdraw
        public async Task<ActionResult> Index(string openId, DateTime? startDate, DateTime? endDate, int pageIndex = 1,
            int pageSize = 10)
        {
            var queryable = db.Log_Withdraws.AsQueryable();
            if (!string.IsNullOrEmpty(openId))
                queryable = queryable.Where(p => p.OpenId.Contains(openId));

            //按时间段查询
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);
            if ((startDate == null) && (endDate == null))
            {
                queryable = queryable.Where(p => (p.CreateTime >= start) && (p.CreateTime <= end));
            }
            else if ((startDate != null) && (endDate == null))
            {
                var tmpStart = new DateTime(startDate.Value.Year, startDate.Value.Month, 1);
                var tmpEnd = start.AddMonths(1).AddDays(-1);
                queryable = queryable.Where(p => (p.CreateTime >= startDate) && (p.CreateTime <= tmpEnd));
            }
            else if ((startDate == null) && (endDate != null))
            {
                var tmpStart = new DateTime(endDate.Value.Year, endDate.Value.Month, 1);
                queryable = queryable.Where(p => (p.CreateTime >= start) && (p.CreateTime <= endDate));
            }
            else
            {
                queryable = queryable.Where(p => (p.CreateTime >= startDate) && (p.CreateTime <= endDate));
            }

            var pagedList = new PagedList<Log_Withdraw>(
                await queryable.OrderBy(p => p.OpenId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex,
                pageSize,
                await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Log_Withdraw/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_Recharge = await db.Log_Withdraws.FindAsync(id);
            if (log_Recharge == null)
                return HttpNotFound();
            return View(log_Recharge);
        }

        //get:Log_Withdraw/Approve/1
        public async Task<ActionResult> Approve(int id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_Withdraw = await db.Log_Withdraws.FindAsync(id);
            if (log_Withdraw == null)
                return HttpNotFound();
            return View(log_Withdraw);
        }

        [HttpPost]
        public async Task<ActionResult> Approve(int id, string approvalComments)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_Withdraw = await db.Log_Withdraws.FindAsync(id);
            if (log_Withdraw == null)
                return HttpNotFound();
            log_Withdraw.Status = WithdrawApprovalStatus.Approved;
            log_Withdraw.ApprovalComments = approvalComments;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //get:Log_Withdraw/Reject/1
        public async Task<ActionResult> Reject(int id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_Withdraw = await db.Log_Withdraws.FindAsync(id);
            if (log_Withdraw == null)
                return HttpNotFound();
            return View(log_Withdraw);
        }

        [HttpPost]
        public async Task<ActionResult> Reject(int id, string approvalComments)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_Withdraw = await db.Log_Withdraws.FindAsync(id);
            if (log_Withdraw == null)
                return HttpNotFound();
            log_Withdraw.Status = WithdrawApprovalStatus.Reject;
            log_Withdraw.ApprovalComments = approvalComments;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}