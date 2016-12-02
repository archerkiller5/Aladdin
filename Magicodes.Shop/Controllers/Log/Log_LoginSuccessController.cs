// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Log_LoginSuccessController.cs
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
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models.Log;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Log
{
    public class Log_LoginSuccessController : TenantBaseController<Log_LoginSuccess>
    {
        // GET: Log_LoginSuccess  
        [RoleMenuFilter("登录成功日志", "{CB3B9204-B570-48FD-8C82-2A6D75B8FB7D}", "Admin,TenantManager,ShopManager",
             url: "/Log_LoginSuccess", parentId: "F348A29F-9F4C-476A-9932-23C607A4F9FB")]
        public async Task<ActionResult> Index(string q, DateTime? sDatePicker, DateTime? eDatePicker, int pageIndex = 1,
            int pageSize = 10)
        {
            var queryable = db.Log_LoginSuccess.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.LoginName.Contains(q) || p.ClientIpAddress.Contains(q));

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

            var pagedList = new PagedList<Log_LoginSuccess>(
                await queryable.OrderBy(p => p.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex,
                pageSize,
                await queryable.CountAsync()
            );
            return View(pagedList);
        }

        // GET: Log_LoginSuccess/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var log_LoginSuccess = await db.Log_LoginSuccess.FindAsync(id);
            if (log_LoginSuccess == null)
                return HttpNotFound();
            return View(log_LoginSuccess);
        }

        // GET: Log_Login/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Log_Login/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Log_Login/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Log_Login/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Log_LoginSuccess/Delete       
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

                var list = await db.Log_LoginSuccess.Where(p => p.CreateTime <= date).ToListAsync();
                db.Log_LoginSuccess.RemoveRange(list);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}