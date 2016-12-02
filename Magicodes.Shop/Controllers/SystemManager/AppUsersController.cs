// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppUsersController.cs
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
    public class AppUsersController : BaseController
    {
        private readonly IdentityManager _identityManager = new IdentityManager();

        public AppUserManager UserManager
        {
            get { return _identityManager.UserManager; }
        }

        // GET: AppUsers
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 20)
        {
            var queryable = UserManager.Users;
            //如果是系统管理员
            if (db.Admin_Tenants.Any(p => (p.Id == UserTenantId) && p.IsSystemTenant))
            {
            }
            else
            {
                queryable = queryable.Where(p => p.TenantId == UserTenantId);
            }
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.OpenId.Contains(q) || p.Email.Contains(q) || p.PasswordHash.Contains(q) ||
                            p.SecurityStamp.Contains(q) || p.PhoneNumber.Contains(q) || p.UserName.Contains(q));
            var pagedList = new PagedList<AppUser>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: AppUsers/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appUser = await UserManager.FindByIdAsync(id);
            if (appUser == null)
                return HttpNotFound();
            return View(appUser);
        }


        // GET: AppUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(
                 Include =
                     "Id,OpenId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName"
             )] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(appUser);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appUser = await UserManager.FindByIdAsync(id);
            if (appUser == null)
                return HttpNotFound();
            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(
                 Include =
                     "Id,OpenId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName"
             )] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appUser = await UserManager.FindByIdAsync(id);
            if (appUser == null)
                return HttpNotFound();
            //至少需要保存一个管理员账户
            if (db.Admin_Tenants.Count(p => (p.Id == appUser.TenantId) && p.IsSystemTenant) == 1)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "至少需要保留一个管理员账户！");
            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var appUser = await UserManager.FindByIdAsync(id);
            db.Users.Remove(appUser);
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