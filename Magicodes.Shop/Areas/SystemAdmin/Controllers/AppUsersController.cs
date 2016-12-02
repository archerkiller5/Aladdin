// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppUsersController.cs
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
using System.Web;
using System.Web.Mvc;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.Shop.Areas.SystemAdmin.Models;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.Identity;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Areas.SystemAdmin.Controllers
{
    public class AppUsersController : SystemAdminBase<WeiChat_App, int>
    {
        private readonly IdentityManager _identityMangaer = new IdentityManager();

        public AppUserManager UserManager
        {
            get { return _identityMangaer.UserManager; }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        // GET: AppUsers
        [RoleMenuFilter("租户成员管理", "F4DF1F7A-32D6-408F-B8BA-4171531E4E16", "Admin", url: "/SystemAdmin/AppUsers",
             parentId: "A1199F15-8859-425D-9148-B0D85F4272F2", tag: "System")]
        public async Task<ActionResult> Index(string q, int? tenantId = null, int? check = null, string roleId = null,
            int pageIndex = 1, int pageSize = 20)
        {
            var queryable = UserManager.Users;
            if (tenantId != null)
            {
                queryable = queryable.Where(p => p.TenantId == tenantId);
            }
            else
            {
                //如果是系统管理员
                if (!db.Admin_Tenants.Any(p => (p.Id == UserTenantId) && p.IsSystemTenant))
                    throw new Exception("您没有权限进行相关操作！");
            }
            if (!string.IsNullOrEmpty(roleId))
                if (check != null)
                    queryable = queryable.Where(p => !p.Roles.Any(p1 => p1.RoleId == roleId));
                else
                    queryable = queryable.Where(p => p.Roles.Any(p1 => p1.RoleId == roleId));
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(p => p.Email.Contains(q) || p.PhoneNumber.Contains(q) || p.UserName.Contains(q));
            var pagedList = new PagedList<AppUser>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: AppUsers/Bind
        public ActionResult Bind(string id)
        {
            var appUser = UserManager.FindById(id);
            if (appUser == null)
                return HttpNotFound();
            var qrCode =
                db.WeiChat_QRCodes.FirstOrDefault(
                    p => (p.ParamsValue == id) && (p.UserFor == QRCodeUseForTypes.BindManager));
            if (qrCode == null)
            {
                var result = WeChatApisContext.Current.QrCodeApi.CreateByStringValue(id);
                if (result.IsSuccess())
                {
                    qrCode = new WeiChat_QRCode
                    {
                        CreateBy = appUser.Id,
                        CreateTime = DateTime.Now,
                        ExpireSeconds = 0,
                        ParamsValue = id,
                        TenantId = appUser.TenantId,
                        Ticket = result.Ticket,
                        UserFor = QRCodeUseForTypes.BindManager,
                        Remark = "绑定微信管理员"
                    };
                    db.WeiChat_QRCodes.Add(qrCode);
                    db.SaveChanges();
                }
            }
            return View(qrCode);
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
        public ActionResult Create(int? tenantId = null)
        {
            ViewBag.TenantId = new SelectList(db.Admin_Tenants.ToList(), "Id", "Name", tenantId);
            return View();
        }

        // POST: AppUsers/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<AppUser, string>(new AppUserStore(new AppDbContext())
                {
                    TenantId = model.TenantId
                });
                await userManager.CreateAsync(new AppUser
                {
                    Id = Guid.NewGuid().ToString("N"),
                    UserName = model.UserName,
                    Email = model.Email,
                    TenantId = model.TenantId,
                    PhoneNumber = model.PhoneNumber
                }, model.Password);
                return RedirectToAction("Index", new { tenantId = model.TenantId });
            }
            ViewBag.TenantId = new SelectList(db.Admin_Tenants.ToList(), "Id", "Name", model.TenantId);
            return View(model);
        }

        // GET: AppUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appUser = db.Users.FirstOrDefault(p => p.Id == id);
            if (appUser == null)
                return HttpNotFound();
            ViewBag._TenantId = appUser.TenantId;
            ViewBag.TenantId = new SelectList(db.Admin_Tenants.ToList(), "Id", "Name", appUser.TenantId);
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
                     "Id,OpenId,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,TenantId"
             )] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                var oldUser = await db.Users.FirstAsync(p => p.Id == appUser.Id);
                //UserManager.ChangePhoneNumberAsync()
                oldUser.OpenId = appUser.OpenId;
                oldUser.Email = appUser.Email;
                //oldUser.UserName
                oldUser.PhoneNumber = appUser.PhoneNumber;
                oldUser.TwoFactorEnabled = appUser.TwoFactorEnabled;
                oldUser.TenantId = appUser.TenantId;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { tenantId = oldUser.TenantId });
            }
            ViewBag.TenantId = new SelectList(db.Admin_Tenants.ToList(), "Id", "Name", appUser.TenantId);
            return View(appUser);
        }

        public async Task<ActionResult> ChangePassword(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appUser = await UserManager.FindByIdAsync(id);
            if (appUser == null)
                return HttpNotFound();
            var model = new ChangePasswordViewModel
            {
                Email = appUser.Email,
                UserName = appUser.UserName,
                Id = appUser.Id
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var appUser = await UserManager.FindByIdAsync(model.Id);
            if (appUser == null)
                return HttpNotFound();
            var userManager = new UserManager<AppUser, string>(new AppUserStore(new AppDbContext())
            {
                TenantId = appUser.TenantId
            });
            //先移除密码，再加上密码
            var result = await userManager.RemovePasswordAsync(model.Id);
            if (result.Succeeded)
                result = await userManager.AddPasswordAsync(model.Id, model.Password);
            if (result.Succeeded)
                return RedirectToAction("Index", new { tenantId = appUser.TenantId });
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
            return View(model);
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
            if ((db.Users.Count(p => p.TenantId == appUser.TenantId) == 1) &&
                (db.Admin_Tenants.Count(p => (p.Id == appUser.TenantId) && p.IsSystemTenant) == 1))
                throw new Exception("至少需要保留一个管理员账户！");
            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var appUser = await UserManager.FindByIdAsync(id);
            await UserManager.DeleteAsync(appUser);
            return RedirectToAction("Index", new { tenantId = appUser.TenantId });
        }

        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("SystemAdmin/AppUsers/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, string param, params string[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Users.Where(p => ids.Contains(p.Id)).ToListAsync();
                    if (models.Count == 0)
                    {
                        ajaxResponse.Success = false;
                        ajaxResponse.Message = "没有找到匹配的项，项已被删除或不存在！";
                        return Json(ajaxResponse);
                    }

                    switch (operation.ToUpper())
                    {
                        case "ADDUSERSTOROLE":

                            #region 添加用户到指定角色

                            {
                                var role = await db.Roles.FirstOrDefaultAsync(p => p.Id == param);
                                if (role == null)
                                {
                                    ajaxResponse.Success = false;
                                    ajaxResponse.Message = "没有找到对应的角色，角色已被删除或不存在！";
                                    return Json(ajaxResponse);
                                }
                                var success = 0;
                                foreach (var item in models)
                                {
                                    var result = _identityMangaer.UserManager.AddToRole(item.Id, role.Name);
                                    if (result.Succeeded)
                                        success++;
                                }
                                if (ids.Contains(UserId))
                                {
                                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                                    var identity =
                                        await
                                            UserManager.CreateIdentityAsync(models.First(p => p.Id == UserId),
                                                DefaultAuthenticationTypes.ApplicationCookie);
                                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false },
                                        identity);
                                }
                                ajaxResponse.Success = true;
                                ajaxResponse.Message = string.Format("已成功操作{0}项！", success);
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

        #region 设置角色

        public ActionResult Roles(string userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        public ActionResult GetRoles(string userId)
        {
            using (var db = new AppDbContext())
            {
                var q = db.Roles.ToList();
                var userRole = db.Users.Include(p => p.Roles).FirstOrDefault(p => p.Id == userId).Roles.ToList();
                return Json(q.Select(p => new
                {
                    id = p.Id,
                    text = p.Name,
                    state = new
                    {
                        selected = userRole.Any(p1 => p1.RoleId == p.Id),
                        opened = true
                    },
                    children = false
                }).ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SetRoles(string userId, string[] roleIds)
        {
            var ajaxResponse = new AjaxResponse();
            try
            {
                //TODO:后续修改为EF批量处理代码
                db.Database.ExecuteSqlCommand("Delete from mwc.Admin_UserRoles where [UserId]={0}", userId);
                if (roleIds != null)
                {
                    var user = db.Users.Find(userId);
                    foreach (var item in roleIds)
                        user.Roles.Add(new AppUserRole { RoleId = item, UserId = userId });
                    db.SaveChanges();
                }

                ajaxResponse.Message = "处理成功！";
                ajaxResponse.Success = true;
            }
            catch (Exception ex)
            {
                ajaxResponse.Message = ex.Message;
                ajaxResponse.Success = false;
            }
            return Json(ajaxResponse);
        }

        #endregion
    }
}