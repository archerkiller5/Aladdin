// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_KFCInfoController.cs
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
using Magicodes.Shop.Helpers;
using Magicodes.Shop.Models;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat
{
    public class WeiChat_KFCInfoController : TenantBaseController<WeiChat_KFCInfo>
    {
        // GET: WeiChat_KFCInfo
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.WeiChat_KFCInfos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.NickName.Contains(q) || p.HeadImgUrl.Contains(q));
            var pagedList = new PagedList<WeiChat_KFCInfo>(
                await queryable.OrderBy(p => p.Account)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: WeiChat_KFCInfo/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var weiChat_KFCInfo = await db.WeiChat_KFCInfos.FindAsync(id);
            if (weiChat_KFCInfo == null)
                return HttpNotFound();
            return View(weiChat_KFCInfo);
        }

        // GET: WeiChat_KFCInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WeiChat_KFCInfo/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddCustomViewModel weiChat_KFCInfo)
        {
            if (ModelState.IsValid)
            {
                var accout = string.Format("{0}@{1}", weiChat_KFCInfo.Account,
                    WeChatConfigManager.Current.GetConfig().WeiXinAccount);
                var result = WeChatApisContext.Current.CustomerServiceApi.AddCustomerAccount(accout,
                    weiChat_KFCInfo.NickName, weiChat_KFCInfo.Password);
                if (result.IsSuccess())
                {
                    var model = new WeiChat_KFCInfo
                    {
                        NickName = weiChat_KFCInfo.NickName,
                        Account = accout
                    };
                    SetModelWithChangeStates(model, default(int));
                    await db.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("", result.GetFriendlyMessage());
                    return View(weiChat_KFCInfo);
                }
                return RedirectToAction("Index");
            }

            return View(weiChat_KFCInfo);
        }

        // GET: WeiChat_KFCInfo/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var weiChat_KFCInfo = await db.WeiChat_KFCInfos.FindAsync(id);
            if (weiChat_KFCInfo == null)
                return HttpNotFound();
            return View(weiChat_KFCInfo);
        }

        // POST: WeiChat_KFCInfo/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int Id, string NickName, string Password)
        {
            var model = await db.WeiChat_KFCInfos.FindAsync(Id);
            var result = WeChatApisContext.Current.CustomerServiceApi.UpdateCustomerAccount(model.Account, NickName,
                Password);
            if (result.IsSuccess())
            {
                model.NickName = NickName;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", result.GetFriendlyMessage());
            return View(model);
        }

        // GET: WeiChat_KFCInfo/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            var weiChat_KFCInfo = await db.WeiChat_KFCInfos.FindAsync(id);
            if (weiChat_KFCInfo == null)
                return HttpNotFound();
            return View(weiChat_KFCInfo);
        }

        // POST: WeiChat_KFCInfo/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var weiChat_KFCInfo = await db.WeiChat_KFCInfos.FindAsync(id);
            db.WeiChat_KFCInfos.Remove(weiChat_KFCInfo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: WeiChat_KFCInfo/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("WeiChat_KFCInfo/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params string[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.WeiChat_KFCInfos.Where(p => ids.Contains(p.Account)).ToListAsync();
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
                            db.WeiChat_KFCInfos.RemoveRange(models);
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