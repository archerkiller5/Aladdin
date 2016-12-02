// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_KeyWordTextContentController.cs
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
    public class WeiChat_KeyWordTextContentController : TenantBaseController<WeiChat_KeyWordTextContent>
    {
        // GET: WeiChat_KeyWordTextContent
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var appId = AppId;
            var queryable =
                db.WeiChat_KeyWordTextContents.Include(w => w.CreateUser).Include(w => w.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Text.Contains(q));
            var pagedList = new PagedList<WeiChat_KeyWordTextContent>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: WeiChat_KeyWordTextContent/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordTextContent =
                await
                    db.WeiChat_KeyWordTextContents.Include(p => p.UpdateUser)
                        .Include(p => p.CreateUser)
                        .FirstOrDefaultAsync(p => p.Id == id);
            if (weiChat_KeyWordTextContent == null)
                return HttpNotFound();
            return View(weiChat_KeyWordTextContent);
        }

        // GET: WeiChat_KeyWordTextContent/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_KeyWordTextContent/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Text,CreateTime,UpdateTime,CreateBy,UpdateBy,AppId")] WeiChat_KeyWordTextContent
                weiChat_KeyWordTextContent)
        {
            if (ModelState.IsValid)
            {
                weiChat_KeyWordTextContent.Id = Guid.NewGuid();
                weiChat_KeyWordTextContent.CreateBy = UserId;
                weiChat_KeyWordTextContent.CreateTime = DateTime.Now;
                weiChat_KeyWordTextContent.TenantId = TenantId;
                db.WeiChat_KeyWordTextContents.Add(weiChat_KeyWordTextContent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(weiChat_KeyWordTextContent);
        }

        // GET: WeiChat_KeyWordTextContent/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordTextContent = await db.WeiChat_KeyWordTextContents.FindAsync(id);
            if (weiChat_KeyWordTextContent == null)
                return HttpNotFound();
            return View(weiChat_KeyWordTextContent);
        }

        // POST: WeiChat_KeyWordTextContent/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Text")] WeiChat_KeyWordTextContent weiChat_KeyWordTextContent)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(weiChat_KeyWordTextContent, weiChat_KeyWordTextContent.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(weiChat_KeyWordTextContent);
        }

        // GET: WeiChat_KeyWordTextContent/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KeyWordTextContent =
                await
                    db.WeiChat_KeyWordTextContents.Include(p => p.UpdateUser)
                        .Include(p => p.CreateUser)
                        .FirstOrDefaultAsync(p => p.Id == id);
            if (weiChat_KeyWordTextContent == null)
                return HttpNotFound();
            return View(weiChat_KeyWordTextContent);
        }

        // POST: WeiChat_KeyWordTextContent/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var weiChat_KeyWordTextContent = await db.WeiChat_KeyWordTextContents.FindAsync(id);
            db.WeiChat_KeyWordTextContents.Remove(weiChat_KeyWordTextContent);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_KeyWordTextContent/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_KeyWordTextContent/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_KeyWordTextContents.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.WeiChat_KeyWordTextContents.RemoveRange(models);
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