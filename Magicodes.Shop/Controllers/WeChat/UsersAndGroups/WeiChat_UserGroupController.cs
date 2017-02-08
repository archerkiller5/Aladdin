// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_UserGroupController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
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
using Magicodes.Shop.Helpers;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat.UsersAndGroups
{
    public class WeiChat_UserGroupController : TenantBaseController<WeiChat_UserGroup>
    {
        // GET: WeiChat_UserGroup
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.WeiChat_UserGroups.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q));
            var pagedList = new PagedList<WeiChat_UserGroup>(
                await queryable.OrderBy(p => p.GroupId)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: WeiChat_UserGroup/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_UserGroup = await db.WeiChat_UserGroups.FindAsync(id);
            if (weiChat_UserGroup == null)
                return HttpNotFound();
            return View(weiChat_UserGroup);
        }

        // GET: WeiChat_UserGroup/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_UserGroup/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name")] WeiChat_UserGroup weiChat_UserGroup)
        {
            weiChat_UserGroup.UsersCount = 0;
            //多对多修改
            //weiChat_UserGroup.GroupIds.Add(-1);
            if (ModelState.IsValid)
            {
                if (db.WeiChat_UserGroups.Any(p => p.Name == weiChat_UserGroup.Name))
                {
                    ModelState.AddModelError(string.Empty, "已存在该用户组，请不要重复添加！");
                    return View(weiChat_UserGroup);
                }
                var result = WeChatApisContext.Current.UserGroupApi.Create(weiChat_UserGroup.Name);
                if (result.IsSuccess())
                {
                    //多对多修改
                    weiChat_UserGroup.GroupId=result.Group.Id;
                    weiChat_UserGroup.TenantId = TenantId;
                    db.WeiChat_UserGroups.Add(weiChat_UserGroup);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(weiChat_UserGroup);
        }

        // GET: WeiChat_UserGroup/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_UserGroup = await db.WeiChat_UserGroups.FindAsync(id);
            if (weiChat_UserGroup == null)
                return HttpNotFound();
            return View(weiChat_UserGroup);
        }

        // POST: WeiChat_UserGroup/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,GroupId,Name,UsersCount")] WeiChat_UserGroup weiChat_UserGroup)
        {
            if (ModelState.IsValid)
            {
                var result = WeChatApisContext.Current.UserGroupApi.Update(weiChat_UserGroup.GroupId,
                    weiChat_UserGroup.Name);
                if (result.IsSuccess())
                {
                    weiChat_UserGroup.TenantId = TenantId;
                    db.Entry(weiChat_UserGroup).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(weiChat_UserGroup);
        }

        // GET: WeiChat_UserGroup/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_UserGroup = await db.WeiChat_UserGroups.FindAsync(id);
            if (weiChat_UserGroup == null)
                return HttpNotFound();
            return View(weiChat_UserGroup);
        }

        // POST: WeiChat_UserGroup/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            var weiChat_UserGroup = await db.WeiChat_UserGroups.FindAsync(id);
            if (weiChat_UserGroup.UsersCount > 0)
                throw new Exception("该用户组下尚存在用户，无法删除！");
            var result = WeChatApisContext.Current.UserGroupApi.Delete(weiChat_UserGroup.GroupId);
            if (result.IsSuccess())
            {
                db.WeiChat_UserGroups.Remove(weiChat_UserGroup);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            throw new Exception(result.GetFriendlyMessage() ?? "删除失败！");
        }

        // POST: WeiChat_UserGroup/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_UserGroup/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_UserGroups.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            var successCount = 0;
                            foreach (var item in models)
                            {
                                //存在用户无法删除
                                if (item.UsersCount > 0)
                                    continue;
                                var result = WeChatApisContext.Current.UserGroupApi.Delete(item.GroupId);
                                if (result.IsSuccess())
                                    successCount++;
                            }
                            db.WeiChat_UserGroups.RemoveRange(models);
                            await db.SaveChangesAsync();
                            ajaxResponse.Success = true;
                            ajaxResponse.Message = string.Format("已成功操作{0}/{1}项！{2}", successCount, models.Count,
                                successCount != models.Count ? "部分分组下存在用户，请先移除用户后再行删除！" : "");
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