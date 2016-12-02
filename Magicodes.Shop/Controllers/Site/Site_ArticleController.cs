// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_ArticleController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using MvcCheckBoxList.Model;
using System.Collections.Generic;

namespace Magicodes.Shop.Controllers.Site
{
    public class Site_ArticleController : TenantBaseController<Site_Article>
    {
        // GET: Site_Article
        public async Task<ActionResult> Index(string q, Guid? type = null, int pageIndex = 1, int pageSize = 10)
        {
            if (
                !db.Site_ResourceTypes.Any(
                    p => (p.ResourceType == SiteResourceTypes.Article) && p.IsSystemResource && (p.Title == "默认")))
            {
                var article = new Site_ResourceType
                {
                    Title = "默认",
                    IsSystemResource = true,
                    ResourceType = SiteResourceTypes.Article
                };
                SetModel(article, default(Guid));
                db.Site_ResourceTypes.Add(article);
                db.SaveChanges();
            }
            if (type == null)
            {
                var typeId = db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).First().Id;
                var model = new {type = typeId};
                if (!string.IsNullOrEmpty(Request.QueryString["lightLayout"]))
                    return RedirectToAction("Index", new {type = typeId, lightLayout = 1});
                return RedirectToAction("Index", new {type = typeId});
            }
            

            ViewBag.ArticleTypes =
                db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).ToList();
            var queryable = db.Site_Articles.Include(s => s.CreateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Content.Contains(q) || p.Name.Contains(q));
            queryable = queryable.Where(p => p.ResourcesTypeId == type.Value);
            var pagedList = new PagedList<Site_Article>(
                await queryable.OrderByDescending(p => p.CreateTime).Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());

            return View(pagedList);
        }

        [AllowAnonymous]
        public async Task<ActionResult> IndexList(string q, string type = null, int pageIndex = 1, int pageSize =5)
        {
            if (
                !db.Site_ResourceTypes.Any(
                    p => (p.ResourceType == SiteResourceTypes.Article) && p.IsSystemResource && (p.Title == "默认")))
            {
                var article = new Site_ResourceType
                {
                    Title = "默认",
                    IsSystemResource = true,
                    ResourceType = SiteResourceTypes.Article
                };
                SetModel(article, default(Guid));
                db.Site_ResourceTypes.Add(article);
                db.SaveChanges();
            }
            if (type == null)
            {
                var typeId = db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).First().Id;
                var model = new { type = typeId };
                if (!string.IsNullOrEmpty(Request.QueryString["lightLayout"]))
                    return RedirectToAction("IndexList", new { type = typeId, lightLayout = 1 });
                return RedirectToAction("IndexList", new { type = typeId });
            }
            ViewBag.ArticleTypes =
                db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).ToList();
            var queryable = db.Site_Articles.AsQueryable();
           
            //if (!string.IsNullOrWhiteSpace(q))
            //    queryable = queryable.Where(p => p.Content.Contains(q) || p.Name.Contains(q));
            //queryable = queryable.Where(p => p.ResourcesTypeId == type.v);
            Guid? tiel;
            if (pageIndex == 1)
            {
                ViewBag.title = tiel = db.Site_ResourceTypes.Where(p => p.Title == type).First().Id;
                queryable = queryable.Where(p => p.ResourcesTypeId == tiel);
            }
            else {
                queryable = queryable.Where(p => p.ResourcesTypeId == new Guid( type));
            }
            var pagedList = new PagedList<Site_Article>(
                await queryable.OrderByDescending(p => p.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            if (pageIndex == 1)
               
            return PartialView( pagedList);
            else
            return Json(pagedList,typeof (Site_Article).ToString(),null,JsonRequestBehavior.AllowGet);
        }


      

        // GET: Site_Article/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_Article = await db.Site_Articles.FindAsync(id);
            if (site_Article == null)
                return HttpNotFound();
            return View(site_Article);
        }
       [WeChatOAuth]
       [AllowAnonymous]
        public async Task<ActionResult> DetailContent(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_Article = await db.Site_Articles.FindAsync(id);

            if (site_Article == null)
                return HttpNotFound();
            if (site_Article.UserGroups == "0")
            {
                if (site_Article.OriginalUrl != null || site_Article.OriginalUrl != "")
                    return Redirect(site_Article.OriginalUrl);
                else
                    return PartialView(site_Article);
            }
            else
            {
                string str=WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                
                WeiChat_User us = db.WeiChat_Users.Find(str);
                if (us != null)
                {
                    if (us.GroupId.ToString() == site_Article.UserGroups)
                    {
                        if (site_Article.OriginalUrl != null || site_Article.OriginalUrl != "")
                            return Redirect(site_Article.OriginalUrl);
                        else
                            return PartialView(site_Article);
                    }
                    
                } 
                return Content("没有权限查看");
            }
        }
        // GET: Site_Article/Create
        public ActionResult Create(Guid resourcesTypeId)
        {
            //Models.ArticleUserGroupViewModel ArtGroup = new Models.ArticleUserGroupViewModel();

            //ArtGroup.Article = new Site_Article();
            //ArtGroup.AllGroups = db.WeiChat_UserGroups.ToList();
            //ArtGroup.SelectedGroupIds = new[] { 0 };
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).ToList(),
                    dataTextField: "Title", dataValueField: "Id", selectedValue: resourcesTypeId);

            //List<WeiChat_UserGroup> currentArticleGroup = new List<WeiChat_UserGroup>();
            //foreach (var item in ArtGroup.SelectedGroupIds)
            //{
            //    var temp = ArtGroup.AllGroups.Where(u => u.Id == item).FirstOrDefault();
            //    currentArticleGroup.Add(temp);
            //}
            //ArtGroup.ArticleGroups = currentArticleGroup;
            ViewBag.UserGroup = new SelectList( db.WeiChat_UserGroups.ToList() ,dataTextField:"Name", dataValueField: "GroupId", 
                selectedValue:0);

            return View();
        }

        //Get:

        private void GetImgUrl(Site_Article site_Article)
        {
            db.Site_Images.Find();
        }
        // POST: Site_Article/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Content,ResourcesTypeId,Summary,Name,SiteUrl,Url,CreateTime,OriginalUrl,FrontCoverImageUrl,UserGroups")] Site_Article
                site_Article)
        {
            if (ModelState.IsValid)
            {
                //site_Article.Content = site_Article.Content.Replace("\"", "'");
                SetImgUrls(site_Article);
                
                SetModel(site_Article, default(Guid));
                db.Site_Articles.Add(site_Article);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new {type = site_Article.ResourcesTypeId});
            }
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).ToList(),
                    dataTextField: "Title", dataValueField: "Id", selectedValue: site_Article.ResourcesTypeId);

            ViewBag.UserGroup = new SelectList(db.WeiChat_UserGroups.ToList(), dataTextField: "Name", dataValueField: "GroupId",
                selectedValue: site_Article.UserGroups );
            return View(site_Article);
        }


        /// <summary>
        /// 2016年11月24日屏蔽代码更改图片链接为微信服务器链接
        /// </summary>
        /// <param name="site_Article"></param>
        /// <param name="isLoad"></param>
        private void SetImgUrls(Site_Article site_Article, bool isLoad = false)
        {
            //// 定义正则表达式用来匹配 img 标签   
            //var regImg =
            //    new Regex(
            //        @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>",
            //        RegexOptions.IgnoreCase);
            //// 搜索匹配的字符串   
            //var matches = regImg.Matches(site_Article.Content);
            //var hostUrl = string.Format("{0}://{1}{2}/", Request.Url.Scheme, Request.Url.Host,
            //    Request.Url.Port == 80 ? "" : ":" + Request.Url.Port);
            //var i = 0;
            //// 取得匹配项列表   
            //foreach (Match match in matches)
            //{
            //    var url = match.Groups["imgUrl"].Value;
            //    if (string.IsNullOrWhiteSpace(url))
            //        continue;
            //    if (!isLoad && !url.StartsWith(hostUrl))
            //        continue;

            //    Site_Image img = null;
            //    if (isLoad)
            //    {
            //        img = db.Site_Images.FirstOrDefault(p => p.Url == url);
            //        site_Article.Content = site_Article.Content.Replace(url, hostUrl + img.SiteUrl);
            //    }
            //    else
            //    {
            //        var siteUrl = url.Remove(0, hostUrl.Length);
            //        img = db.Site_Images.FirstOrDefault(p => p.SiteUrl == siteUrl);
            //        site_Article.Content = site_Article.Content.Replace(url, img.Url);
            //    }
            //}
        }

        // GET: Site_Article/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_Article = await db.Site_Articles.FindAsync(id);
            if (site_Article == null)
                return HttpNotFound();
            SetImgUrls(site_Article, true);
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).ToList(),
                    dataTextField: "Title", dataValueField: "Id", selectedValue: site_Article.ResourcesTypeId);


            ViewBag.UserGroup = new SelectList(db.WeiChat_UserGroups.ToList(), dataTextField: "Name", dataValueField: "GroupId",
    selectedValue: site_Article.UserGroups);
            return View(site_Article);
        }

        // POST: Site_Article/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Content,ResourcesTypeId,Summary,Name,SiteUrl,Url,CreateTime,OriginalUrl,TenantId,FrontCoverImageUrl,UserGroups")] Site_Article site_Article)
        {
            if (ModelState.IsValid)
            {
                //site_Article.Content = site_Article.Content.Replace("\"", "'");
                SetImgUrls(site_Article);
                SetModelWithChangeStates(site_Article, site_Article.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new {type = site_Article.ResourcesTypeId});
            }
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).ToList(),
                    dataTextField: "Title", dataValueField: "Id", selectedValue: site_Article.ResourcesTypeId);

            ViewBag.UserGroup = new SelectList(db.WeiChat_UserGroups.ToList(), dataTextField: "Name", dataValueField: "GroupId",
    selectedValue: site_Article.UserGroups);

            return View(site_Article);
        }
        
        // GET: Site_Article/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_Article = await db.Site_Articles.FindAsync(id);
            if (site_Article == null)
                return HttpNotFound();
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Article).ToList(),
                    dataTextField: "Title", dataValueField: "Id");
            return View(site_Article);
        }

        // POST: Site_Article/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var site_Article = await db.Site_Articles.FindAsync(id);
            db.Site_Articles.Remove(site_Article);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Site_Article/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Site_Article/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Site_Articles.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.Site_Articles.RemoveRange(models);
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