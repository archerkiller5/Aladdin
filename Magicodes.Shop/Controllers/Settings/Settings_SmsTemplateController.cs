using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.Shop.Controllers;
using Magicodes.WeiChat.Infrastructure.Services;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.Mvc.AuditFilter;

namespace Magicodes.Shop.Controllers.Settings
{
    public class Settings_SmsTemplateController : TenantBaseController<Settings_SmsTemplate>
    {
        [RoleMenuFilter("阿里短信模板设置", "A1384D4D-1BE4-48AB-8A23-4E8FE33D0FC5", "Admin,TenantManager,ShopManager",
            url: "/Settings_SmsTemplate", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        // GET: Settings_SmsTemplate
        [AuditFilter("阿里短信模板设置页","82742d28-7ec7-444c-a8ea-36dc0740b27a")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Settings_SmsTemplates.Include(s => s.CreateUser).Include(s => s.UpdateUser).AsQueryable();
			if (!string.IsNullOrWhiteSpace(q))
            {
				//请替换为相应的搜索逻辑
				                queryable = queryable.Where(p => p.TemplateCode.Contains(q)|| p.SignName.Contains(q)|| p.Content.Contains(q)|| p.Demo.Contains(q)|| p.Remark.Contains(q));
				            }
			var pagedList = new PagedList<Settings_SmsTemplate>(
							 await queryable.OrderBy(p => p.Id)
							 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
							 pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Settings_SmsTemplate/Details/5
		[AuditFilter("阿里短信模板详情页","eb4af3ce-40ac-4a13-bfca-def50dc49321")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Settings_SmsTemplate settings_SmsTemplate = await db.Settings_SmsTemplates.FindAsync(id);
            if (settings_SmsTemplate == null)
            {
                return HttpNotFound();
            }
            return View(settings_SmsTemplate);
        }

        // GET: Settings_SmsTemplate/Create
		[AuditFilter("阿里短信模板详情创建页","8c687481-f8c9-426b-9197-307eba87758b")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Settings_SmsTemplate/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
		[AuditFilter("阿里短信模板创建","bb8e3158-100e-4439-bf55-a5fb03898df0")]
        public async Task<ActionResult> Create([Bind(Include = "Id,TemplateCode,SignName,Content,Demo,SmsType,Remark,CreateTime,CreateBy,UpdateTime,UpdateBy,TenantId")] Settings_SmsTemplate settings_SmsTemplate)
        {
            if (ModelState.IsValid)
            {
                var checkVal = db.Settings_SmsTemplates.Where(p => p.SmsType == settings_SmsTemplate.SmsType).FirstOrDefault();
                if (checkVal == null)
                {
                    settings_SmsTemplate.Id = Guid.NewGuid();
                    SetModelWithChangeStates(settings_SmsTemplate, default(Guid?));
                    db.Settings_SmsTemplates.Add(settings_SmsTemplate);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError("", "已经存在相同业务的短信模板!");
            }
            return View(settings_SmsTemplate);
        }
         
        // GET: Settings_SmsTemplate/Delete/5
		[AuditFilter("阿里短信模板删除页","222b3fb8-8114-48ed-9cc3-c13ca559b403")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Settings_SmsTemplate settings_SmsTemplate = await db.Settings_SmsTemplates.FindAsync(id);
            if (settings_SmsTemplate == null)
            {
                return HttpNotFound();
            }
            return View(settings_SmsTemplate);
        }

        // POST: Settings_SmsTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[AuditFilter("阿里短信模板删除功能","c87417fa-8c5f-4e2f-8024-4cf76b329fac")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            Settings_SmsTemplate settings_SmsTemplate = await db.Settings_SmsTemplates.FindAsync(id);
            db.Settings_SmsTemplates.Remove(settings_SmsTemplate);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

		// POST: Settings_SmsTemplate/BatchOperation/{operation}
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Settings_SmsTemplate/BatchOperation/{operation}")]
		[AuditFilter("批量删除阿里短信模板","e541408b-6bd9-4bf6-b94d-75bba8c4f8ba")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Settings_SmsTemplates.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Settings_SmsTemplates.RemoveRange(models);
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

        // GET: Settings_SmsTemplate/Demo/
        [AuditFilter("阿里短信模板预览页", "0CBD12B5-345F-41E1-B513-013430B9453C")]
        public async Task<ActionResult> Demo(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Settings_SmsTemplate settings_SmsTemplate = await db.Settings_SmsTemplates.FindAsync(id);
            if (settings_SmsTemplate == null)
            {
                return HttpNotFound();
            }
            return View(settings_SmsTemplate);
        }

        // GET: Settings_SmsTemplate/Create
        [AuditFilter("阿里短信模板详情创建页", "8c687481-f8c9-426b-9197-307eba87758b")]
        public ActionResult TestMsg()
        {
            return View();
        }

        [AuditFilter("阿里短信接口测试", "809C6345-4FE4-4C8F-A6C0-84A5EBFD04D8")]
        [HttpPost]
        public ActionResult TestMsg(string phoneNumber)
        {           
            var ajaxResponse = new AjaxResponse();
            try
            {
                var aliMsg =
                    db.Settings_AliMsgs.FirstOrDefault(p => p.TenantId == WeiChatApplicationContext.Current.TenantId);
                if (aliMsg == null)
                    throw new Exception("未对短信接口进行配置!");
                log.Log(Magicodes.Logger.LoggerLevels.Debug, "开始调用测试短信服务");
                ShopSmsService smsServer = new ShopSmsService();
                smsServer.SendTestMsg(phoneNumber);
                
                log.Log(Magicodes.Logger.LoggerLevels.Debug, "调用测试短信服务完毕");
                ajaxResponse.Success = true;
                ajaxResponse.Message = "测试完毕";           
            }
            catch (Exception ea)
            {
                ajaxResponse.Success = false;
                ajaxResponse.Message = "测试失败!原因:" + ea.Message;               
            }
            return Json(ajaxResponse);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
