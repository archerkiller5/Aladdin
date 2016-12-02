// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppRolesController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.Identity;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.SystemManager
{
    public class AppRolesController : BaseController
    {
        private IdentityManager _identityManager = new IdentityManager();
        // GET: AppRoles
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 20)
        {
            var queryable = db.Set<AppRole>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q));
            var pagedList = new PagedList<AppRole>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: AppRoles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appRole = await db.Set<AppRole>().FindAsync(id);
            if (appRole == null)
                return HttpNotFound();
            return View(appRole);
        }

        // GET: AppRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppRoles/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] AppRole appRole)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(appRole);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(appRole);
        }

        // GET: AppRoles/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appRole = await db.Set<AppRole>().FindAsync(id);
            if (appRole == null)
                return HttpNotFound();
            return View(appRole);
        }

        // POST: AppRoles/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] AppRole appRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(appRole);
        }

        // GET: AppRoles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appRole = await db.Set<AppRole>().FindAsync(id);
            if (appRole == null)
                return HttpNotFound();
            return View(appRole);
        }

        // POST: AppRoles/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var appRole = await db.Set<AppRole>().FindAsync(id);
            db.Set<AppRole>().Remove(appRole);
            await db.SaveChangesAsync();
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