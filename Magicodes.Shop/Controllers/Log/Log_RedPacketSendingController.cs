// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Log_RedPacketSendingController.cs
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
    public class Log_RedPacketSendingController : TenantBaseController<Log_RedPacketSending>
    {
        [RoleMenuFilter("红包日志", "{D2184C46-99F0-4EF2-9134-727695610645}", "Admin,TenantManager,ShopManager",
             url: "/Log_RedPacketSending", parentId: "{61C8B1C8-354B-4D1A-A29E-1488DF254319}")]
        // GET: Log_RedPacketSending
        public async Task<ActionResult> Index(string openId, DateTime? startDate, DateTime? endDate, int pageIndex = 1,
            int pageSize = 10)
        {
            var queryable = db.Log_RedPacketSendings.AsQueryable();
            if (!string.IsNullOrWhiteSpace(openId))
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

            var pagedList = new PagedList<Log_RedPacketSending>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex,
                pageSize,
                await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Log_RedPacketSending/Details/5		
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_RedPacketSending = await db.Log_RedPacketSendings.FindAsync(id);
            if (log_RedPacketSending == null)
                return HttpNotFound();
            return View(log_RedPacketSending);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}