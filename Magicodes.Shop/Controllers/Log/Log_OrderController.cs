// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Log_OrderController.cs
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
using Magicodes.Shop.Models;
using Magicodes.WeiChat.Data.Models.Log;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Log
{
    public class Log_OrderController : TenantBaseController<Log_Order>
    {
        [RoleMenuFilter("支付日志", "{D05C361F-363F-4628-8EDC-EE879E930572}", "Admin,TenantManager,ShopManager",
             url: "/Log_Order", parentId: "{61C8B1C8-354B-4D1A-A29E-1488DF254319}")]
        // GET: Log_Order
        public async Task<ActionResult> Index(string openId, DateTime? sDatePicker, DateTime? eDatePicker,
            int pageIndex = 1, int pageSize = 10)
        {
            var data = from o in db.Log_Orders
                       join u in db.User_Infos on o.OpenId equals u.OpenId into temp
                       from tt in temp.DefaultIfEmpty()
                       select new LogOrderViewModel
                       {
                           OpenId = o.OpenId,
                           Mobile = tt.Mobile,
                           OrderId = o.OrderId,
                           Amount = o.Amount,
                           CreateTime = o.CreateTime,
                           PaymentType = o.PaymentType
                       };

            var queryable = data.AsQueryable();
            if (!string.IsNullOrEmpty(openId))
                queryable = queryable.Where(p => p.OpenId.Contains(openId));

            //按时间段查询
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

            var pagedList = new PagedList<LogOrderViewModel>(
                await queryable.OrderBy(p => p.OpenId).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex,
                pageSize,
                await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Log_Order/Details/5
        [AuditFilter("请输入审计关键描述，比如“订单详细”", "98cc3d51-4f87-444e-922e-4a6d38f4c9d6")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_User = await db.Log_Orders.FindAsync(id);
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