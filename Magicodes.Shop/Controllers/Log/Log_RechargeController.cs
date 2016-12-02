// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Log_RechargeController.cs
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
    public class Log_RechargeController : TenantBaseController<Log_Recharge>
    {
        [RoleMenuFilter("充值日志", "{F918B927-5C45-4CED-BB8C-8710A8B3BCE8}", "Admin,TenantManager,ShopManager",
             url: "/Log_Recharge", parentId: "{61C8B1C8-354B-4D1A-A29E-1488DF254319}")]
        // GET: Log_Recharge
        public async Task<ActionResult> Index(string openId, DateTime? startDate, DateTime? endDate, int pageIndex = 1,
            int pageSize = 10)
        {
            var queryable = db.Log_Recharges.AsQueryable();
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

            var pagedList = new PagedList<Log_Recharge>(
                await queryable.OrderBy(p => p.OpenId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex,
                pageSize,
                await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Log_Recharge/Details/5        
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_User = await db.Log_Recharges.FindAsync(id);
            if (weiChat_User == null)
                return HttpNotFound();
            return View(weiChat_User);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}