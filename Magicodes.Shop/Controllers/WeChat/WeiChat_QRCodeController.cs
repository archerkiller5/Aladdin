// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_QRCodeController.cs
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
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Apis.QRCode;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat
{
    public class WeiChat_QRCodeController : TenantBaseController<WeiChat_QRCode>
    {
        // GET: WeiChat_QRCode
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.WeiChat_QRCodes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Remark.Contains(q));
            var pagedList = new PagedList<WeiChat_QRCode>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: WeiChat_QRCode/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_QRCode = await db.WeiChat_QRCodes.FindAsync(id);
            if (weiChat_QRCode == null)
                return HttpNotFound();
            return View(weiChat_QRCode);
        }

        // GET: WeiChat_QRCode/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_QRCode/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(WeiChat_QRCode weiChat_QRCode)
        {
            if (ModelState.IsValid)
            {
                var value = 0;
                QRCodeCreateApiResult result;
                if (int.TryParse(weiChat_QRCode.ParamsValue, out value))
                    result = WeChatApisContext.Current.QrCodeApi.CreateByNumberValue(value);
                else
                    result = WeChatApisContext.Current.QrCodeApi.CreateByStringValue(weiChat_QRCode.ParamsValue);
                weiChat_QRCode.ExpireTime = result.ExpireTime;
                weiChat_QRCode.Ticket = result.Ticket;
                SetModelWithChangeStates(weiChat_QRCode, default(int));
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(weiChat_QRCode);
        }

        //// GET: WeiChat_QRCode/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    WeiChat_QRCode weiChat_QRCode = await db.WeiChat_QRCode.FindAsync(id);
        //    if (weiChat_QRCode == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(weiChat_QRCode);
        //}

        //// POST: WeiChat_QRCode/Edit/5
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "Id,ParamsValue,ExpireSeconds,Url")] WeiChat_QRCode weiChat_QRCode)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(weiChat_QRCode).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(weiChat_QRCode);
        //}

        // GET: WeiChat_QRCode/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_QRCode = await db.WeiChat_QRCodes.FindAsync(id);
            if (weiChat_QRCode == null)
                return HttpNotFound();
            return View(weiChat_QRCode);
        }

        // POST: WeiChat_QRCode/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            var weiChat_QRCode = await db.WeiChat_QRCodes.FindAsync(id);
            db.WeiChat_QRCodes.Remove(weiChat_QRCode);
            //TODO：移除二维码图片
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_QRCode/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_QRCode/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_QRCodes.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.WeiChat_QRCodes.RemoveRange(models);
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