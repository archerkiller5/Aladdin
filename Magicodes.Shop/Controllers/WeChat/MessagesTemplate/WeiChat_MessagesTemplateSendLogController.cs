// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_MessagesTemplateSendLogController.cs
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
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat.MessagesTemplate
{
    public class WeiChat_MessagesTemplateSendLogController : TenantBaseController<WeiChat_MessagesTemplateSendLog>
    {
        // GET: WeiChat_MessagesTemplateSendLog
        public async Task<ActionResult> Index(string q, int? templateId = null, Guid? batchNumber = null,
            int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.WeiChat_MessagesTemplateSendLogs.Include(w => w.MessagesTemplate).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Content.Contains(q) || p.TopColor.Contains(q) || p.Result.Contains(q));
            if (templateId != null)
                queryable = queryable.Where(p => p.MessagesTemplateId == templateId);
            if (batchNumber != null)
                queryable = queryable.Where(p => p.BatchNumber == batchNumber);

            var pagedList = new PagedList<WeiChat_MessagesTemplateSendLog>(
                await queryable.OrderByDescending(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            foreach (var item in pagedList)
                item.Receiver = db.WeiChat_Users.Find(item.ReceiverId);
            return View(pagedList);
        }

        // GET: WeiChat_MessagesTemplateSendLog/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_MessagesTemplateSendLog =
                await
                    db.WeiChat_MessagesTemplateSendLogs.Include(p => p.MessagesTemplate)
                        .FirstOrDefaultAsync(p => p.Id == id);
            if (weiChat_MessagesTemplateSendLog == null)
                return HttpNotFound();
            weiChat_MessagesTemplateSendLog.Receiver = db.WeiChat_Users.Find(weiChat_MessagesTemplateSendLog.ReceiverId);
            return View(weiChat_MessagesTemplateSendLog);
        }

        //// GET: WeiChat_MessagesTemplateSendLog/Create
        //public ActionResult Create()
        //{
        //    ViewBag.MessagesTemplateNo = new SelectList(db.WeiChat_MessagesTemplates, "TemplateNo", "Title");
        //    ViewBag.ReceiverId = new SelectList(db.WeiChat_Users, "OpenId", "NickName");
        //    return View();
        //}

        //// POST: WeiChat_MessagesTemplateSendLog/Create
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,BatchNumber,MessagesTemplateNo,Content,CreateTime,ReceiverId,TopColor,Url,Result")] WeiChat_MessagesTemplateSendLog weiChat_MessagesTemplateSendLog)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.WeiChat_MessagesTemplateSendLogs.Add(weiChat_MessagesTemplateSendLog);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.MessagesTemplateNo = new SelectList(db.WeiChat_MessagesTemplates, "TemplateNo", "Title", weiChat_MessagesTemplateSendLog.MessagesTemplateNo);
        //    ViewBag.ReceiverId = new SelectList(db.WeiChat_Users, "OpenId", "NickName", weiChat_MessagesTemplateSendLog.ReceiverId);
        //    return View(weiChat_MessagesTemplateSendLog);
        //}

        //// GET: WeiChat_MessagesTemplateSendLog/Edit/5
        //public async Task<ActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    WeiChat_MessagesTemplateSendLog weiChat_MessagesTemplateSendLog = await db.WeiChat_MessagesTemplateSendLogs.FindAsync(id);
        //    if (weiChat_MessagesTemplateSendLog == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.MessagesTemplateNo = new SelectList(db.WeiChat_MessagesTemplates, "TemplateNo", "Title", weiChat_MessagesTemplateSendLog.MessagesTemplateNo);
        //    ViewBag.ReceiverId = new SelectList(db.WeiChat_Users, "OpenId", "NickName", weiChat_MessagesTemplateSendLog.ReceiverId);
        //    return View(weiChat_MessagesTemplateSendLog);
        //}

        //// POST: WeiChat_MessagesTemplateSendLog/Edit/5
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,BatchNumber,MessagesTemplateNo,Content,CreateTime,ReceiverId,TopColor,Url,Result")] WeiChat_MessagesTemplateSendLog weiChat_MessagesTemplateSendLog)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(weiChat_MessagesTemplateSendLog).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MessagesTemplateNo = new SelectList(db.WeiChat_MessagesTemplates, "TemplateNo", "Title", weiChat_MessagesTemplateSendLog.MessagesTemplateNo);
        //    ViewBag.ReceiverId = new SelectList(db.WeiChat_Users, "OpenId", "NickName", weiChat_MessagesTemplateSendLog.ReceiverId);
        //    return View(weiChat_MessagesTemplateSendLog);
        //}

        // GET: WeiChat_MessagesTemplateSendLog/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_MessagesTemplateSendLog = await db.WeiChat_MessagesTemplateSendLogs.FindAsync(id);
            if (weiChat_MessagesTemplateSendLog == null)
                return HttpNotFound();
            return View(weiChat_MessagesTemplateSendLog);
        }

        // POST: WeiChat_MessagesTemplateSendLog/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long? id)
        {
            var weiChat_MessagesTemplateSendLog = await db.WeiChat_MessagesTemplateSendLogs.FindAsync(id);
            db.WeiChat_MessagesTemplateSendLogs.Remove(weiChat_MessagesTemplateSendLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_MessagesTemplateSendLog/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_MessagesTemplateSendLog/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params long?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_MessagesTemplateSendLogs.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.WeiChat_MessagesTemplateSendLogs.RemoveRange(models);
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