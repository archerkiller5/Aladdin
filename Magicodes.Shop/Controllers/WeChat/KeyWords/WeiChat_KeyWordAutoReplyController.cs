// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_KeyWordAutoReplyController.cs
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
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.WeiChat.Unity;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat.KeyWords
{
    [Authorize]
    public class WeiChat_KeyWordAutoReplyController : TenantBaseController<WeiChat_KeyWordAutoReply>
    {
        // GET: WeiChat_KeyWordAutoReplay
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.WeiChat_KeyWordAutoReplies.Include(w => w.CreateUser).Include(w => w.UpdateUser);
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.KeyWord.Contains(q));
            var currentItems =
                await
                    queryable.OrderByDescending(p => p.CreateTime)
                        .Skip((pageIndex - 1)*pageSize)
                        .Take(pageSize)
                        .ToListAsync();
            var pagedList = new PagedList<WeiChat_KeyWordAutoReply>(currentItems, pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: WeiChat_KeyWordAutoReplay/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordAutoReplay = await db.WeiChat_KeyWordAutoReplies.FindAsync(id);
            if (weiChat_KeyWordAutoReplay == null)
                return HttpNotFound();
            return View(weiChat_KeyWordAutoReplay);
        }

        // GET: WeiChat_KeyWordAutoReplay/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_KeyWordAutoReplay/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,KeyWord,MatchType,KeyWordContentType,AllowEventKey,ContentId")] WeiChat_KeyWordAutoReply
                weiChat_KeyWordAutoReplay)
        {
            if (ModelState.IsValid)
            {
                if (
                    db.WeiChat_KeyWordAutoReplies.Any(
                        p => (p.TenantId == TenantId) && (p.KeyWord == weiChat_KeyWordAutoReplay.KeyWord)))
                {
                    ModelState.AddModelError("KeyWord", "关键字重复，请重新输入！");
                    return View(weiChat_KeyWordAutoReplay);
                }
                switch (weiChat_KeyWordAutoReplay.KeyWordContentType)
                {
                    case KeyWordContentTypes.Text:
                        break;
                    case KeyWordContentTypes.Image:
                    {
                        var mediaId = Request.Form["mediaId"];
                        if (string.IsNullOrEmpty(mediaId))
                        {
                            ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                            return View(weiChat_KeyWordAutoReplay);
                        }
                        var image = new WeiChat_KeyWordImageContent
                        {
                            ImageMediaId = mediaId
                        };
                        SetModelWithSaveChanges(image, default(Guid));
                        weiChat_KeyWordAutoReplay.ContentId = image.Id;
                    }
                        break;
                    case KeyWordContentTypes.Voice:
                    {
                        var mediaId = Request.Form["mediaId"];
                        if (string.IsNullOrEmpty(mediaId))
                        {
                            ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                            return View(weiChat_KeyWordAutoReplay);
                        }
                        var model = new WeiChat_KeyWordVoiceContent
                        {
                            VoiceMediaId = mediaId
                        };
                        SetModelWithSaveChanges(model, default(Guid));
                        weiChat_KeyWordAutoReplay.ContentId = model.Id;
                        break;
                    }
                    case KeyWordContentTypes.Video:
                    {
                        var mediaId = Request.Form["mediaId"];
                        if (string.IsNullOrEmpty(mediaId))
                        {
                            ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                            return View(weiChat_KeyWordAutoReplay);
                        }
                        var model = new WeiChat_KeyWordVideoContent
                        {
                            MediaId = mediaId
                        };
                        SetModelWithSaveChanges(model, default(Guid));
                        weiChat_KeyWordAutoReplay.ContentId = model.Id;
                        break;
                    }
                    case KeyWordContentTypes.News:
                        break;
                    default:
                        break;
                }
                SetModel(weiChat_KeyWordAutoReplay, default(Guid));
                db.WeiChat_KeyWordAutoReplies.Add(weiChat_KeyWordAutoReplay);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(weiChat_KeyWordAutoReplay);
        }

        // GET: WeiChat_KeyWordAutoReplay/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordAutoReplay = await db.WeiChat_KeyWordAutoReplies.FindAsync(id);
            if (weiChat_KeyWordAutoReplay == null)
                return HttpNotFound();
            return View(weiChat_KeyWordAutoReplay);
        }

        // POST: WeiChat_KeyWordAutoReplay/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,KeyWord,AllowEventKey,MatchType,KeyWordContentType,ContentId,AppId")] WeiChat_KeyWordAutoReply weiChat_KeyWordAutoReply)
        {
            if (ModelState.IsValid)
            {
                SetModel(weiChat_KeyWordAutoReply, weiChat_KeyWordAutoReply.Id);
                var oldValues = db.Entry(weiChat_KeyWordAutoReply).GetDatabaseValues();
                if ((oldValues.GetValue<string>("KeyWord") != weiChat_KeyWordAutoReply.KeyWord) &&
                    db.WeiChat_KeyWordAutoReplies.Any(
                        p => (p.TenantId == TenantId) && (p.KeyWord == weiChat_KeyWordAutoReply.KeyWord)))
                {
                    ModelState.AddModelError("KeyWord", "关键字重复，请重新输入！");
                    return View(weiChat_KeyWordAutoReply);
                }
                var oldContentId = oldValues.GetValue<Guid?>("ContentId");
                var oldType = oldValues.GetValue<KeyWordContentTypes>("KeyWordContentType");

                #region 更新回复内容

                switch (weiChat_KeyWordAutoReply.KeyWordContentType)
                {
                    case KeyWordContentTypes.Text:
                        break;
                    case KeyWordContentTypes.Image:
                    {
                        var mediaId = Request.Form["mediaId"];
                        if (string.IsNullOrEmpty(mediaId))
                        {
                            ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                            return View(weiChat_KeyWordAutoReply);
                        }

                        var image = new WeiChat_KeyWordImageContent
                        {
                            ImageMediaId = mediaId
                        };
                        SetModelWithSaveChanges(image, default(Guid));
                        weiChat_KeyWordAutoReply.ContentId = image.Id;
                    }
                        break;
                    case KeyWordContentTypes.Voice:
                    {
                        var mediaId = Request.Form["mediaId"];
                        if (string.IsNullOrEmpty(mediaId))
                        {
                            ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                            return View(weiChat_KeyWordAutoReply);
                        }
                        var model = new WeiChat_KeyWordVoiceContent
                        {
                            VoiceMediaId = mediaId
                        };
                        SetModelWithSaveChanges(model, default(Guid));
                        weiChat_KeyWordAutoReply.ContentId = model.Id;
                        break;
                    }
                    case KeyWordContentTypes.Video:
                    {
                        var mediaId = Request.Form["mediaId"];
                        if (string.IsNullOrEmpty(mediaId))
                        {
                            ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                            return View(weiChat_KeyWordAutoReply);
                        }

                        var model = new WeiChat_KeyWordVideoContent
                        {
                            MediaId = mediaId
                        };
                        SetModelWithSaveChanges(model, default(Guid));
                        weiChat_KeyWordAutoReply.ContentId = model.Id;
                        break;
                    }
                    case KeyWordContentTypes.News:
                        break;
                    default:
                        break;
                }

                #endregion

                #region 删除之前的资源

                if (oldContentId != null)
                    switch (oldType)
                    {
                        case KeyWordContentTypes.Text:
                            break;
                        case KeyWordContentTypes.Image:
                        {
                            var oldContent = db.WeiChat_KeyWordImageContents.Find(oldContentId);
                            if (oldContent != null)
                                db.WeiChat_KeyWordImageContents.Remove(oldContent);
                        }
                            break;
                        case KeyWordContentTypes.Voice:
                        {
                            var oldContent = db.WeiChat_KeyWordVoiceContents.Find(oldContentId);
                            if (oldContent != null)
                                db.WeiChat_KeyWordVoiceContents.Remove(oldContent);
                        }
                            break;
                        case KeyWordContentTypes.Video:
                        {
                            var oldContent = db.WeiChat_KeyWordVideoContents.Find(oldContentId);
                            if (oldContent != null)
                                db.WeiChat_KeyWordVideoContents.Remove(oldContent);
                        }
                            break;
                        case KeyWordContentTypes.News:
                            break;
                        case KeyWordContentTypes.CustomerService:
                            break;
                        default:
                            break;
                    }

                #endregion

                db.Entry(weiChat_KeyWordAutoReply).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(weiChat_KeyWordAutoReply);
        }

        // GET: WeiChat_KeyWordAutoReplay/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordAutoReplay = await db.WeiChat_KeyWordAutoReplies.FindAsync(id);
            if (weiChat_KeyWordAutoReplay == null)
                return HttpNotFound();
            return View(weiChat_KeyWordAutoReplay);
        }

        // POST: WeiChat_KeyWordAutoReplay/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var weiChat_KeyWordAutoReplay = await db.WeiChat_KeyWordAutoReplies.FindAsync(id);
            db.WeiChat_KeyWordAutoReplies.Remove(weiChat_KeyWordAutoReplay);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult GetKeyWordContentTypes()
        {
            return Json(typeof(KeyWordContentTypes).GetEnumDiplayDictionary(), JsonRequestBehavior.AllowGet);
        }

        [Route("WeiChat_KeyWordAutoReplay/Data/{type}/{contentId}")]
        public ActionResult GetJsonDataByContentId(KeyWordContentTypes type, Guid contentId)
        {
            switch (type)
            {
                case KeyWordContentTypes.Text:
                {
                    return Json(db.WeiChat_KeyWordTextContents.Find(contentId), JsonRequestBehavior.AllowGet);
                }
                    break;
                case KeyWordContentTypes.Image:
                {
                    var content = db.WeiChat_KeyWordImageContents.Find(contentId);
                    if (content != null)
                        return Json(db.Site_Images.FirstOrDefault(p => p.MediaId == content.ImageMediaId),
                            JsonRequestBehavior.AllowGet);
                }
                    break;
                case KeyWordContentTypes.Voice:
                {
                    var content = db.WeiChat_KeyWordVoiceContents.Find(contentId);
                    if (content != null)
                        return Json(db.Site_Voices.FirstOrDefault(p => p.MediaId == content.VoiceMediaId),
                            JsonRequestBehavior.AllowGet);
                }
                    break;
                case KeyWordContentTypes.Video:
                {
                    var content = db.WeiChat_KeyWordVideoContents.Find(contentId);
                    if (content != null)
                        return Json(db.Site_Videos.FirstOrDefault(p => p.MediaId == content.MediaId),
                            JsonRequestBehavior.AllowGet);
                }
                    break;
                case KeyWordContentTypes.News:
                {
                    var content = db.WeiChat_KeyWordNewsContents.Find(contentId);
                    return Json(content, JsonRequestBehavior.AllowGet);
                }
                    break;
                default:
                    break;
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        // POST: WeiChat_KeyWordAutoReplay/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_KeyWordAutoReplay/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_KeyWordAutoReplies.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.WeiChat_KeyWordAutoReplies.RemoveRange(models);
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