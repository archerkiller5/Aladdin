// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_MenuController.cs
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
using System.Web.Mvc;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Areas.SystemAdmin.Controllers
{
    [RoleMenuFilter("菜单管理", "ACA4A2AF-0AE9-4467-A7D4-74FDCB72D422", "Admin", iconCls: "fa fa-navicon",
         tag: "System")]
    public class Site_MenuController : SystemAdminBase<WeiChat_App, int>
    {
        // GET: SystemAdmin/Site_Menu
        [RoleMenuFilter("角色管理", "EA032F2C-665A-429D-AD9B-8EA80B322891", "Admin", url: "/SystemAdmin/Site_menu",
             parentId: "ACA4A2AF-0AE9-4467-A7D4-74FDCB72D422", tag: "System")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Site_Menus.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Title.Contains(q) || p.Controller.Contains(q) || p.Action.Contains(q));
            var pagedList = new PagedList<Site_Menu>(
                await queryable.OrderBy(p => p.Path)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: SystemAdmin/Site_Menu/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_Menu = await db.Site_Menus.FindAsync(id);
            if (site_Menu == null)
                return HttpNotFound();
            return View(site_Menu);
        }

        // GET: SystemAdmin/Site_Menu/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.Site_Menus.Where(p => p.ParentId == null).ToList(),
                dataTextField: "Title", dataValueField: "Id");
            return View();
        }

        // POST: SystemAdmin/Site_Menu/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Title,Url,Controller,Action,IconCls,ParentId")] Site_Menu site_Menu)
        {
            if (ModelState.IsValid)
            {
                site_Menu.Path = site_Menu.ParentId == null
                    ? site_Menu.Id.ToString("N")
                    : string.Format("{0}-{1}", site_Menu.ParentId.Value.ToString("N"), site_Menu.Id.ToString("N"));
                db.Site_Menus.Add(site_Menu);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.Site_Menus.Where(p => p.ParentId == null).ToList(),
                dataTextField: "Title", dataValueField: "Id", selectedValue: site_Menu.ParentId);
            return View(site_Menu);
        }

        // GET: SystemAdmin/Site_Menu/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_Menu = await db.Site_Menus.FindAsync(id);
            if (site_Menu == null)
                return HttpNotFound();
            ViewBag.ParentId = new SelectList(db.Site_Menus.Where(p => p.ParentId == null).ToList(),
                dataTextField: "Title", dataValueField: "Id", selectedValue: site_Menu.ParentId);
            return View(site_Menu);
        }

        // POST: SystemAdmin/Site_Menu/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Title,Url,Controller,Action,IconCls,ParentId")] Site_Menu site_Menu)
        {
            if (ModelState.IsValid)
            {
                site_Menu.Path = site_Menu.ParentId == null
                    ? site_Menu.Id.ToString("N")
                    : string.Format("{0}-{1}", site_Menu.ParentId.Value.ToString("N"), site_Menu.Id.ToString("N"));
                db.Set<Site_Menu>().Attach(site_Menu);
                db.Entry(site_Menu).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.Site_Menus.Where(p => p.ParentId == null).ToList(),
                dataTextField: "Title", dataValueField: "Id", selectedValue: site_Menu.ParentId);
            return View(site_Menu);
        }

        // GET: SystemAdmin/Site_Menu/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_Menu = await db.Site_Menus.FindAsync(id);
            if (site_Menu == null)
                return HttpNotFound();
            return View(site_Menu);
        }

        // POST: SystemAdmin/Site_Menu/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var site_Menu = await db.Site_Menus.FindAsync(id);
            db.Site_Menus.Remove(site_Menu);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: SystemAdmin/Site_Menu/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("SystemAdmin/Site_Menu/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Site_Menus.Where(p => ids.Contains(p.Id)).ToListAsync();
                    if (models.Count == 0)
                    {
                        ajaxResponse.Success = false;
                        ajaxResponse.Message = "没有找到匹配的项，项已被删除或不存在！";
                        return Json(ajaxResponse);
                    }
                    if (db.Site_Menus.Any(p => ids.Contains(p.ParentId)))
                    {
                        ajaxResponse.Success = false;
                        ajaxResponse.Message = "需要先删除子菜单才能删除父级菜单！";
                        return Json(ajaxResponse);
                    }
                    switch (operation.ToUpper())
                    {
                        case "DELETE":

                            #region 删除

                            {
                                db.Site_Menus.RemoveRange(models);
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
    }
}