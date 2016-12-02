// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_NewsController.cs
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
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Senparc.Weixin.MP.AdvancedAPIs;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Site
{
    public class Site_NewsController : TenantBaseController<Site_News>
    {
        // GET: Site_News
        public async Task<ActionResult> Index(string q, Guid? type = null, int pageIndex = 1, int pageSize = 10)
        {
            if (
                !db.Site_ResourceTypes.Any(
                    p => (p.ResourceType == SiteResourceTypes.News) && p.IsSystemResource && (p.Title == "默认")))
            {
                var news = new Site_ResourceType
                {
                    Title = "默认",
                    IsSystemResource = true,
                    ResourceType = SiteResourceTypes.News
                };
                SetModel(news, default(Guid));
                db.Site_ResourceTypes.Add(news);
                db.SaveChanges();
            }
            if (type == null)
            {
                var typeId = db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.News).First().Id;
                return RedirectToAction("Index", new {type = typeId});
            }
            var queryable = db.Site_News.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q));
            ViewBag.q = q;
            queryable = queryable.Where(p => p.ResourcesTypeId == type.Value);
            var pagedList = new PagedList<Site_News>(
                await queryable.OrderByDescending(p => p.CreateTime)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Site_News/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_News = await db.Site_News.FindAsync(id);
            if (site_News == null)
                return HttpNotFound();
            return View(site_News);
        }

        // GET: Site_News/Create
        public ActionResult Create(Guid resourcesTypeId)
        {
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.News).ToList(),
                    dataTextField: "Title", dataValueField: "Id", selectedValue: resourcesTypeId);
            return View();
        }

        // POST: Site_News/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Site_News site_News)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(site_News, default(Guid));
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new {type = site_News.ResourcesTypeId});
            }
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.News).ToList(),
                    dataTextField: "Title", dataValueField: "Id", selectedValue: site_News.ResourcesTypeId);
            return View(site_News);
        }

        // GET: Site_News/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var site_News = await db.Site_News.FindAsync(id);
            if (site_News == null)
                return HttpNotFound();
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.News).ToList(),
                    dataTextField: "Title", dataValueField: "Id", selectedValue: site_News.ResourcesTypeId);
            return View(site_News);
        }

        // POST: Site_News/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Site_News site_News)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(site_News, site_News.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new {type = site_News.ResourcesTypeId});
            }
            ViewBag.ResourcesTypeId =
                new SelectList(db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.News).ToList(),
                    dataTextField: "Title", dataValueField: "Id");
            return View(site_News);
        }


        [HttpDelete]
        public ActionResult Delete(Guid? id)
        {
            var response = new AjaxResponse {Success = true, Message = "删除成功！"};
            var site_News = db.Site_News.Include(p => p.Articles).FirstOrDefault(p => p.Id == id);
            if (site_News.Articles.Any())
            {
                db.Site_NewsArticles.RemoveRange(site_News.Articles);
                db.SaveChanges();
            }
            db.Site_News.Remove(site_News);
            db.SaveChanges();
            MediaApi.DeleteForeverMedia(AccessToken, site_News.MediaId);
            return Json(response);
        }

        // POST: Site_News/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Site_News/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Site_News.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                MediaApi.DeleteForeverMedia(AccessToken, item.MediaId);
                            db.Site_News.RemoveRange(models);
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