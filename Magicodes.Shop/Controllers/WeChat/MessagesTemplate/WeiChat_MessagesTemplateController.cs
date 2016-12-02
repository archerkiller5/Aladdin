// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_MessagesTemplateController.cs
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
using Magicodes.Shop.Helpers;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Apis.TemplateMessage;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Data.Models.Site;

namespace Magicodes.Shop.Controllers.WeChat.MessagesTemplate
{
    public class WeiChat_MessagesTemplateController : TenantBaseController<WeiChat_MessagesTemplate>
    {
        // GET: WeiChat_MessagesTemplate
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.WeiChat_MessagesTemplates.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.Title.Contains(q) || p.OneIndustry.Contains(q) || p.TwoIndustry.Contains(q) ||
                            p.Content.Contains(q));
            var pagedList = new PagedList<WeiChat_MessagesTemplate>(
                await queryable.OrderBy(p => p.TemplateNo)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: WeiChat_MessagesTemplate/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_MessagesTemplate = await db.WeiChat_MessagesTemplates.FindAsync(id);
            if (weiChat_MessagesTemplate == null)
                return HttpNotFound();
            return View(weiChat_MessagesTemplate);
        }

        // GET: WeiChat_MessagesTemplate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_MessagesTemplate/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string shortNo)
        {
            var weiChat_MessagesTemplate = new WeiChat_MessagesTemplate {ShortNo = shortNo};
            {
                if (db.WeiChat_MessagesTemplates.Any(p => p.ShortNo == shortNo))
                {
                    ModelState.AddModelError("ShortNo", "该模板消息已存在！");
                    return View(weiChat_MessagesTemplate);
                }
                var result = WeChatApisContext.Current.TemplateMessageApi.AddTemplate(weiChat_MessagesTemplate.ShortNo);
                if (result.IsSuccess())
                {
                    weiChat_MessagesTemplate.TemplateNo = result.TemplateId;
                    weiChat_MessagesTemplate.Title = "temp";
                    SetModelWithSaveChanges(weiChat_MessagesTemplate, default(int));
                    var tenantId = TenantId;
                    var userId = UserId;
                    var paramStr = tenantId + ";" + userId;
                    WeiChatApplicationContext.Current.TaskManager.Start(WeiChat_SyncTypes.SyncMessagesTemplatesTask.ToString(), paramStr, new Site_Notify()
                    {
                        CreateBy = UserId,
                        CreateTime = DateTime.Now,
                        IconCls = "fa fa-circle-o",
                        UpdateTime = DateTime.Now,
                        IsTaskFinish = false,
                        Receiver = "Tenant_" + tenantId,
                    });
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", result.GetFriendlyMessage());
            }
            return View(weiChat_MessagesTemplate);
        }

        // GET: WeiChat_MessagesTemplate/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_MessagesTemplate = await db.WeiChat_MessagesTemplates.FindAsync(id);
            if (weiChat_MessagesTemplate == null)
                return HttpNotFound();
            return View(weiChat_MessagesTemplate);
        }

        // GET: WeiChat_MessagesTemplate/Send/5
        public async Task<ActionResult> Send(int id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_MessagesTemplate = await db.WeiChat_MessagesTemplates.FindAsync(id);
            if (weiChat_MessagesTemplate == null)
                return HttpNotFound();
            return View(weiChat_MessagesTemplate);
        }

        // POST: WeiChat_MessagesTemplate/Send/5
        [HttpPost]
        public async Task<ActionResult> Send(int id, string url, string[] receiverId)
        {
            var ajaxResponse = new AjaxResponse<Guid>
            {
                Success = true,
                Message = "操作成功！"
            };
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if ((receiverId == null) || (receiverId.Length == 0))
            {
                ajaxResponse.Success = false;
                ajaxResponse.Message = "请选择接收人！";
                return Json(ajaxResponse);
            }
            var weiChat_MessagesTemplate = await db.WeiChat_MessagesTemplates.FindAsync(id);
            if (weiChat_MessagesTemplate == null)
                return HttpNotFound();
            //接收人openId，多个请以分号分隔
            var receiverIds = receiverId.Aggregate("", (current, item) => current + (item.Split(';')[1] + ';'));
            //模板消息模型
            var tmm = new TemplateMessageCreateModel
            {
                MessagesTemplateNo = weiChat_MessagesTemplate.TemplateNo,
                Data = new Dictionary<string, TemplateDataItem>(),
                ReceiverIds = receiverIds.Trim(';'),
                Url = url
            };

            foreach (var item in Request.Form.AllKeys)
                if (item.EndsWith(".DATA"))
                {
                    var color = Request.Form[item + "_COLOR"];
                    tmm.Data.Add(item.Split('.')[0], new TemplateDataItem(Request.Form[item], color));
                }
            if (tmm.Data.Count == 0)
            {
                ajaxResponse.Success = false;
                ajaxResponse.Message = "请填写模板信息！";
                return Json(ajaxResponse);
            }
            ajaxResponse.Result = WeChatApisContext.Current.TemplateMessageApi.Create(tmm);
            return Json(ajaxResponse);
        }

        // POST: WeiChat_MessagesTemplate/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,ShortNo,TemplateNo,Title,OneIndustry,TwoIndustry,Content,Demo")] WeiChat_MessagesTemplate weiChat_MessagesTemplate)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(weiChat_MessagesTemplate, weiChat_MessagesTemplate.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(weiChat_MessagesTemplate);
        }

        // GET: WeiChat_MessagesTemplate/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_MessagesTemplate = await db.WeiChat_MessagesTemplates.FindAsync(id);
            if (weiChat_MessagesTemplate == null)
                return HttpNotFound();
            return View(weiChat_MessagesTemplate);
        }

        // POST: WeiChat_MessagesTemplate/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var weiChat_MessagesTemplate = await db.WeiChat_MessagesTemplates.FindAsync(id);
            db.WeiChat_MessagesTemplates.Remove(weiChat_MessagesTemplate);
            var result = WeChatApisContext.Current.TemplateMessageApi.Delete(weiChat_MessagesTemplate.TemplateNo);
            if (result.IsSuccess())
                await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_MessagesTemplate/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_MessagesTemplate/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params int[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_MessagesTemplates.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.WeiChat_MessagesTemplates.RemoveRange(models);
                            foreach (var item in models)
                                WeChatApisContext.Current.TemplateMessageApi.Delete(item.TemplateNo);
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