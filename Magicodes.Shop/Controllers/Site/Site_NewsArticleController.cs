// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_NewsArticleController.cs
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
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.GroupMessage;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Site
{
    public class Site_NewsArticleController : TenantBaseController<Site_NewsArticle>
    {
        // GET: Site_NewsArticle
        public ActionResult Index(Guid? newsId = null, int pageIndex = 1, int pageSize = 10)
        {
            if (newsId == null)
                return HttpNotFound();
            var content =
                db.Site_News.Include(w => w.Articles)
                    //.Include("Articles.Article")
                    //.Include("Articles.FrontCoverImage")
                    .FirstOrDefault(p => p.Id == newsId);
            //为了兼容MySQL
            foreach (var item in content.Articles)
            {
                item.Article = db.Site_Articles.Find(item.SiteArticleId);
                item.FrontCoverImage = db.Site_Images.Find(item.SiteImageId);
            }
            if ((content == null) || (content.Articles == null))
                return View(new PagedList<Site_NewsArticle>(
                    new List<Site_NewsArticle>()
                        .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList(),
                    pageIndex, pageSize, 0));
            var queryable = content.Articles.ToList();

            var pagedList = new PagedList<Site_NewsArticle>(
                queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Site_NewsArticle/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_NewsArticle = await db.Site_NewsArticles.FindAsync(id);
            if (site_NewsArticle == null)
                return HttpNotFound();
            return View(site_NewsArticle);
        }

        // GET: Site_NewsArticle/Create
        public ActionResult Create(Guid? newsId = null)
        {
            if (newsId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return View();
        }

        // POST: Site_NewsArticle/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Site_NewsArticle site_NewsArticle, Guid? newsId = null)
        {
            if (ModelState.IsValid)
            {
                var news = db.Site_News.Include(p => p.Articles).FirstOrDefault(p => p.Id == newsId);
                if (news == null)
                    return HttpNotFound("多图文信息不存在！");
                if (news.Articles.Count > 8)
                {
                    ModelState.AddModelError("", "文章数不能超过8条！");
                    return View(site_NewsArticle);
                }
                SetModel(site_NewsArticle, default(Guid));
                news.Articles.Add(site_NewsArticle);
                await db.SaveChangesAsync();
                SaveNewsToWeChat(newsId.Value);
                return RedirectToAction("Index", new {newsId});
            }
            return View(site_NewsArticle);
        }

        private void SaveNewsToWeChat(Guid newsId)
        {
            var news =
                db.Site_News.Include(w => w.Articles)
                    //.Include("Articles.Article")
                    //.Include("Articles.FrontCoverImage")
                    .FirstOrDefault(p => p.Id == newsId);
            //为了兼容MySQL
            foreach (var item in news.Articles)
            {
                item.Article = db.Site_Articles.Find(item.SiteArticleId);
                item.FrontCoverImage = db.Site_Images.Find(item.SiteImageId);
            }
            var newsList = new List<NewsModel>();
            foreach (var model in news.Articles)
            {
                var newsModel = new NewsModel
                {
                    author = model.Article.Author,
                    content = model.Article.Content,
                    content_source_url = model.Article.OriginalUrl,
                    digest = model.Article.Summary,
                    show_cover_pic = model.IsShowInText ? "1" : "0",
                    title = model.Article.Name,
                    thumb_media_id = model.FrontCoverImage.MediaId
                };
                newsList.Add(newsModel);
            }
            if (string.IsNullOrWhiteSpace(news.MediaId))
            {
                var result = MediaApi.UploadNews(AccessToken, Config.TIME_OUT, newsList.ToArray());
                news.MediaId = result.media_id;
                news.Url = result.url;
                news.FrontCoverImageUrl = news.Articles.First().FrontCoverImage.SiteUrl;
                db.SaveChanges();
            }
            else
            {
                MediaApi.DeleteForeverMedia(AccessToken, news.MediaId);
                var result = MediaApi.UploadNews(AccessToken, Config.TIME_OUT, newsList.ToArray());
                news.MediaId = result.media_id;
                news.Url = result.url;
                news.FrontCoverImageUrl = news.Articles.First().FrontCoverImage.SiteUrl;
                db.SaveChanges();
                //TODO:受微信接口限制，无法实现新增图文，故删除再建
                //var index = 0;
                //foreach (var item in newsList)
                //{
                //    var result = MediaApi.UpdateForeverNews(AccessToken, news.MediaId, index, item);
                //    index++;
                //}
            }
        }

        // GET: Site_NewsArticle/Edit/5
        public ActionResult Edit(long? id, Guid? newsId = null)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (newsId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_NewsArticle =
                db.Site_NewsArticles.Include(m => m.Article)
                    .Include(m => m.FrontCoverImage)
                    .FirstOrDefault(p => p.Id == id);
            if (site_NewsArticle == null)
                return HttpNotFound();
            return View(site_NewsArticle);
        }

        // POST: Site_NewsArticle/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Site_NewsArticle site_NewsArticle, Guid? newsId = null)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(site_NewsArticle, site_NewsArticle.Id);
                db.SaveChanges();
                SaveNewsToWeChat(newsId.Value);
                return RedirectToAction("Index", new {newsId});
            }
            return View(site_NewsArticle);
        }

        // GET: Site_NewsArticle/Delete/5
        public async Task<ActionResult> Delete(long? id, Guid? newsId)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_NewsArticle = await db.Site_NewsArticles.FindAsync(id);
            if (site_NewsArticle == null)
                return HttpNotFound();
            return View(site_NewsArticle);
        }

        // POST: Site_NewsArticle/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long? id, Guid? newsId)
        {
            var site_NewsArticle = await db.Site_NewsArticles.FindAsync(id);
            db.Site_NewsArticles.Remove(site_NewsArticle);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new {newsId});
        }

        // POST: Site_NewsArticle/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Site_NewsArticle/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params long?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Site_NewsArticles.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.Site_NewsArticles.RemoveRange(models);
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