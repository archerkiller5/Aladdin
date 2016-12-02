// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_AppController.cs
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
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat
{
    public class WeiChat_AppController : BaseController
    {
        // GET: WeiChat_App
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.WeiChat_Apps.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.AppId.Contains(q) || p.AppSecret.Contains(q) || p.WeiXinAccount.Contains(q) ||
                            p.CopyrightInformation.Contains(q) || p.CustomerInformation.Contains(q) ||
                            p.CreateBy.Contains(q) || p.UpdateBy.Contains(q));
            var pagedList = new PagedList<WeiChat_App>(
                await queryable.OrderByDescending(p => p.CreateTime)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: WeiChat_App/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_App = await db.WeiChat_Apps.FindAsync(id);
            if (weiChat_App == null)
                return HttpNotFound();
            return View(weiChat_App);
        }

        // GET: WeiChat_App/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_App/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(
                 Include =
                     "Id,AppId,AppSecret,WeiXinAccount,CopyrightInformation,CustomerInformation,CreateBy,CreateTime,UpdateBy,UpdateTime"
             )] WeiChat_App weiChat_App)
        {
            if (ModelState.IsValid)
            {
                db.WeiChat_Apps.Add(weiChat_App);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(weiChat_App);
        }

        // GET: WeiChat_App/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_App = await db.WeiChat_Apps.FindAsync(id);
            if (weiChat_App == null)
                return HttpNotFound();
            return View(weiChat_App);
        }

        // POST: WeiChat_App/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(
                 Include =
                     "Id,AppId,AppSecret,WeiXinAccount,CopyrightInformation,CustomerInformation,CreateBy,CreateTime,UpdateBy,UpdateTime"
             )] WeiChat_App weiChat_App)
        {
            if (ModelState.IsValid)
            {
                db.Entry(weiChat_App).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(weiChat_App);
        }

        // GET: WeiChat_App/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_App = await db.WeiChat_Apps.FindAsync(id);
            if (weiChat_App == null)
                return HttpNotFound();
            return View(weiChat_App);
        }

        // POST: WeiChat_App/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            var weiChat_App = await db.WeiChat_Apps.FindAsync(id);
            db.WeiChat_Apps.Remove(weiChat_App);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_App/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_App/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_Apps.Where(p => ids.Contains(p.TenantId)).ToListAsync();
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
                            db.WeiChat_Apps.RemoveRange(models);
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