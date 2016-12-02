// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_KeyWordReplyLogController.cs
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
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat.KeyWords
{
    public class WeiChat_KeyWordReplyLogController : TenantBaseController<WeiChat_KeyWordReplyLog>
    {
        // GET: WeiChat_KeyWordReplyLog
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.WeiChat_KeyWordReplyLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(p => p.ReceiveWords.Contains(q) || p.KeyWord.Contains(q) || p.Error.Contains(q));
            var pagedList = new PagedList<WeiChat_KeyWordReplyLog>(
                await queryable.OrderByDescending(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: WeiChat_KeyWordReplyLog/Details/5
        public ActionResult Details(long? id)
        {
            var weiChat_KeyWordReplyLog = db.WeiChat_KeyWordReplyLogs.Find(id);
            if (weiChat_KeyWordReplyLog == null)
                return HttpNotFound();
            return View(weiChat_KeyWordReplyLog);
        }

        public ActionResult KeyWordAutoReplyDetails(Guid? id)
        {
            if (id == null)
                return Content("");
            var detail = db.WeiChat_KeyWordAutoReplies.Find(id);
            if (detail == null)
                return Content("");
            return View(detail);
        }

        // GET: WeiChat_KeyWordReplyLog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_KeyWordReplyLog/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(
                 Include =
                     "Id,ReceiveWords,KeyWord,WeiChat_KeyWordAutoReplyId,ContentId,TenantId,CreateTime,IsSuccess,Error")
            ] WeiChat_KeyWordReplyLog weiChat_KeyWordReplyLog)
        {
            if (ModelState.IsValid)
            {
                db.WeiChat_KeyWordReplyLogs.Add(weiChat_KeyWordReplyLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(weiChat_KeyWordReplyLog);
        }

        // GET: WeiChat_KeyWordReplyLog/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordReplyLog = await db.WeiChat_KeyWordReplyLogs.FindAsync(id);
            if (weiChat_KeyWordReplyLog == null)
                return HttpNotFound();
            return View(weiChat_KeyWordReplyLog);
        }

        // POST: WeiChat_KeyWordReplyLog/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(
                 Include =
                     "Id,ReceiveWords,KeyWord,WeiChat_KeyWordAutoReplyId,ContentId,TenantId,CreateTime,IsSuccess,Error")
            ] WeiChat_KeyWordReplyLog weiChat_KeyWordReplyLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(weiChat_KeyWordReplyLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(weiChat_KeyWordReplyLog);
        }

        // GET: WeiChat_KeyWordReplyLog/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordReplyLog = await db.WeiChat_KeyWordReplyLogs.FindAsync(id);
            if (weiChat_KeyWordReplyLog == null)
                return HttpNotFound();
            return View(weiChat_KeyWordReplyLog);
        }

        // POST: WeiChat_KeyWordReplyLog/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long? id)
        {
            var weiChat_KeyWordReplyLog = await db.WeiChat_KeyWordReplyLogs.FindAsync(id);
            db.WeiChat_KeyWordReplyLogs.Remove(weiChat_KeyWordReplyLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_KeyWordReplyLog/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_KeyWordReplyLog/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params long?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_KeyWordReplyLogs.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.WeiChat_KeyWordReplyLogs.RemoveRange(models);
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