// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppRolesController.cs
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
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure.Identity;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;
using System.Collections.Generic;

namespace Magicodes.Shop.Areas.SystemAdmin.Controllers
{
    [RoleMenuFilter("角色管理", "93D84E6B-B56A-4962-863E-0870CEAFEFED", "Admin", iconCls: "fa fa-user",
         tag: "System")]
    public class AppRolesController : SystemAdminBase<WeiChat_App, int>
    {
        private readonly IdentityManager _identityManager = new IdentityManager();

        [RoleMenuFilter("角色管理", "B81A9B47-4A13-492F-A224-0382B2D1885A", "Admin", url: "/SystemAdmin/AppRoles",
             parentId: "93D84E6B-B56A-4962-863E-0870CEAFEFED", tag: "System")]
        // GET: SystemAdmin/AppRoles
        public async Task<ActionResult> Index(string q, int? tenantId = null, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Roles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q));
            var pagedList = new PagedList<AppRole>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: SystemAdmin/AppRoles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appRole = await db.Roles.FirstOrDefaultAsync(p => p.Id == id);
            if (appRole == null)
                return HttpNotFound();
            return View(appRole);
        }

        // GET: SystemAdmin/AppRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemAdmin/AppRoles/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] AppRole appRole)
        {
            if (ModelState.IsValid)
                try
                {
                    appRole.Id = Guid.NewGuid().ToString();
                    var result = _identityManager.RoleManager.Create(appRole);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                        foreach (var item in error.ValidationErrors)
                            ModelState.AddModelError("", item.ErrorMessage);
                }
            return View(appRole);
        }

        // GET: SystemAdmin/AppRoles/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appRole = await db.Roles.FirstOrDefaultAsync(p => p.Id == id);
            if (appRole == null)
                return HttpNotFound();
            return View(appRole);
        }

        // POST: SystemAdmin/AppRoles/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,TenantId,Name")] AppRole appRole)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appRole).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(appRole);
        }

        // GET: SystemAdmin/AppRoles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var appRole = await db.Roles.FirstOrDefaultAsync(p => p.Id == id);
            if (appRole == null)
                return HttpNotFound();
            return View(appRole);
        }

        // POST: SystemAdmin/AppRoles/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            _identityManager.DeleteRole(id);
            return RedirectToAction("Index");
        }

        // POST: SystemAdmin/AppRoles/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("SystemAdmin/AppRoles/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params string[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Roles.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                foreach (var item in models)
                                    db.Roles.Remove(item);
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

        #region 设置菜单

        public ActionResult Menus(string roleId)
        {
            ViewBag.RoleId = roleId;
            return View();
        }

        public ActionResult GetMenus(string roleId, Guid? id)
        {
            using (var db = new AppDbContext())
            {
                var q = db.Site_Menus.Where(p => p.ParentId == id);
                return Json(q.Select(p => new
                {
                    id = p.Id,
                    text = p.Title,
                    state = new
                    {
                        selected = db.Role_Menus.Any(p1 => (p1.RoleId == roleId) && (p1.MenuId == p.Id)),
                        opened = true
                    },
                    children = db.Site_Menus.Any(p1 => p1.ParentId == p.Id)
                }).ToList(), JsonRequestBehavior.AllowGet);
            }
        }
        private void AddParentMenu(List<Site_Menu> menus, AppDbContext db, string roleId, Guid menuId)
        {
            var siteMenu = menus.FirstOrDefault(p => p.Id == menuId);
            if (siteMenu.ParentId.HasValue)
            {
                var menu = new Role_Menu
                {
                    MenuId = siteMenu.ParentId.Value,
                    RoleId = roleId
                };
                db.Role_Menus.Add(menu);
                var parentMenu = menus.FirstOrDefault(p => p.Id == siteMenu.ParentId.Value);
                if (parentMenu.ParentId.HasValue)
                {
                    AddParentMenu(menus, db, roleId, siteMenu.ParentId.Value);
                }
            }
        }

        [HttpPost]
        public ActionResult SetMenus(string roleId, Guid[] menuIds)
        {
            var ajaxResponse = new AjaxResponse();
            try
            {
                db.Role_Menus.RemoveRange(db.Role_Menus.Where(p => p.RoleId == roleId));
                db.SaveChanges();
                //var menus = db.Site_Menus.ToList();
                if (menuIds != null)
                {
                    foreach (var item in menuIds)
                    {
                        //AddParentMenu(menus, db, roleId, item);
                        var menu = new Role_Menu
                        {
                            MenuId = item,
                            RoleId = roleId
                        };
                        db.Role_Menus.Add(menu);
                    }
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