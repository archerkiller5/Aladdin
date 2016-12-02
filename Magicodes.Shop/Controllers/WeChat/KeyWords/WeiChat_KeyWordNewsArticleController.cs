// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_KeyWordNewsArticleController.cs
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
    public class WeiChat_KeyWordNewsArticleController : TenantBaseController<WeiChat_KeyWordNewsArticle>
    {
        // GET: WeiChat_KeyWordNewsArticle
        public ActionResult Index(Guid? ContentId, int pageIndex = 1, int pageSize = 10)
        {
            var content = db.WeiChat_KeyWordNewsContents.Include(w => w.Articles).FirstOrDefault(p => p.Id == ContentId);
            if ((content == null) || (content.Articles == null))
                return View(new PagedList<WeiChat_KeyWordNewsArticle>(
                    new List<WeiChat_KeyWordNewsArticle>()
                        .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList(),
                    pageIndex, pageSize, 0));
            var queryable = content.Articles.ToList();

            var pagedList = new PagedList<WeiChat_KeyWordNewsArticle>(
                queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: WeiChat_KeyWordNewsArticle/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordNewsArticle = await db.WeiChat_KeyWordNewsArticles.FindAsync(id);
            if (weiChat_KeyWordNewsArticle == null)
                return HttpNotFound();
            return View(weiChat_KeyWordNewsArticle);
        }

        // GET: WeiChat_KeyWordNewsArticle/Create
        public ActionResult Create(Guid? ContentId)
        {
            return View();
        }

        // POST: WeiChat_KeyWordNewsArticle/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(
            [Bind(Include = "Id,Title,Description,PicUrl,Url")] WeiChat_KeyWordNewsArticle weiChat_KeyWordNewsArticle,
            Guid? ContentId)
        {
            WeiChat_KeyWordNewsContent content;
            if (ContentId == null)
                content = new WeiChat_KeyWordNewsContent();
            else
                content = db.WeiChat_KeyWordNewsContents.Include("Articles")
                    .FirstOrDefault(p => p.Id == ContentId.Value);
            if (content != null)
                SetModelWithSaveChanges(content, ContentId == null ? default(Guid) : ContentId.Value);
            else
                return HttpNotFound();
            if (ModelState.IsValid)
            {
                SetModel(weiChat_KeyWordNewsArticle, default(long));
                content.Articles.Add(weiChat_KeyWordNewsArticle);
                db.SaveChanges();
                return RedirectToAction("Index", new {ContentId = content.Id});
            }

            return View(weiChat_KeyWordNewsArticle);
        }

        // GET: WeiChat_KeyWordNewsArticle/Edit/5
        public async Task<ActionResult> Edit(long? id, Guid ContentId)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordNewsArticle = await db.WeiChat_KeyWordNewsArticles.FindAsync(id);
            if (weiChat_KeyWordNewsArticle == null)
                return HttpNotFound();
            return View(weiChat_KeyWordNewsArticle);
        }

        // POST: WeiChat_KeyWordNewsArticle/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Title,Description,PicUrl,Url,CreateTime,UpdateTime,CreateBy,UpdateBy,AppId")] WeiChat_KeyWordNewsArticle weiChat_KeyWordNewsArticle, Guid ContentId)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(weiChat_KeyWordNewsArticle, weiChat_KeyWordNewsArticle.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new {ContentId});
            }
            return View(weiChat_KeyWordNewsArticle);
        }

        // GET: WeiChat_KeyWordNewsArticle/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordNewsArticle =
                await
                    db.WeiChat_KeyWordNewsArticles.Include(p => p.CreateUser)
                        .Include(p => p.UpdateUser)
                        .FirstOrDefaultAsync(p => p.Id == id);
            if (weiChat_KeyWordNewsArticle == null)
                return HttpNotFound();
            return View(weiChat_KeyWordNewsArticle);
        }

        // POST: WeiChat_KeyWordNewsArticle/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long? id)
        {
            var weiChat_KeyWordNewsArticle = await db.WeiChat_KeyWordNewsArticles.FindAsync(id);
            db.WeiChat_KeyWordNewsArticles.Remove(weiChat_KeyWordNewsArticle);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_KeyWordNewsArticle/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_KeyWordNewsArticle/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params long?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_KeyWordNewsArticles.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.WeiChat_KeyWordNewsArticles.RemoveRange(models);
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