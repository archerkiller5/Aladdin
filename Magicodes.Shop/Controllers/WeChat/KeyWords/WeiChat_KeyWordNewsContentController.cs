// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_KeyWordNewsContentController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat.KeyWords
{
    public class WeiChat_KeyWordNewsContentController : TenantBaseController<WeiChat_KeyWordNewsContent>
    {
        // GET: WeiChat_KeyWordNewsContent
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable =
                db.WeiChat_KeyWordNewsContents.Include(w => w.CreateUser).Include(w => w.Articles).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.UpdateBy.Contains(q));
            var pagedList = new PagedList<WeiChat_KeyWordNewsContent>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: WeiChat_KeyWordNewsContent/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordNewsContent = await db.WeiChat_KeyWordNewsContents.FindAsync(id);
            if (weiChat_KeyWordNewsContent == null)
                return HttpNotFound();
            return View(weiChat_KeyWordNewsContent);
        }

        // GET: WeiChat_KeyWordNewsContent/Create
        public ActionResult Create()
        {
            return RedirectToAction("Index", "WeiChat_KeyWordNewsArticle", new {ContentId = string.Empty});
        }


        // GET: WeiChat_KeyWordNewsContent/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordNewsContent = await db.WeiChat_KeyWordNewsContents.FindAsync(id);
            if (weiChat_KeyWordNewsContent == null)
                return HttpNotFound();
            return RedirectToAction("Index", "WeiChat_KeyWordNewsArticle", new {ContentId = id});
        }

        // GET: WeiChat_KeyWordNewsContent/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordNewsContent = await db.WeiChat_KeyWordNewsContents.FindAsync(id);
            if (weiChat_KeyWordNewsContent == null)
                return HttpNotFound();
            return View(weiChat_KeyWordNewsContent);
        }

        // POST: WeiChat_KeyWordNewsContent/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var weiChat_KeyWordNewsContent =
                await db.WeiChat_KeyWordNewsContents.Include(m => m.Articles).FirstOrDefaultAsync(p => p.Id == id);
            weiChat_KeyWordNewsContent.Articles = new List<WeiChat_KeyWordNewsArticle>();
            db.WeiChat_KeyWordNewsContents.Remove(weiChat_KeyWordNewsContent);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_KeyWordNewsContent/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_KeyWordNewsContent/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_KeyWordNewsContents.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.WeiChat_KeyWordNewsContents.RemoveRange(models);
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