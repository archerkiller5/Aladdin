// ======================================================================
// 
//         Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//         All rights reserved
// 
//         filename :Activity_LotteryPrizeOptionController.cs
//         description :
// 
//         created by 李文强 at  2016/09/17 12:51
//         Blog：http://www.cnblogs.com/codelove/
//         Home：http://xin-lai.com
// 
// ======================================================================

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Activity;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Activity
{
    public class Activity_LotteryPrizeOptionController : TenantBaseController<Activity_LotteryPrizeOption>
    {
        // GET: Activity_LotteryPrizeOption
        public async Task<ActionResult> Index(int lotteryId, int pageIndex = 1, int pageSize = 10)
        {
            var lottery =
                await db.Activity_Lotteries.Include(a => a.PrizeOptions).FirstOrDefaultAsync(p => p.Id == lotteryId);
            if (lottery == null)
                throw new Exception("抽奖活动不存在！");

            return View(new PagedList<Activity_LotteryPrizeOption>(lottery.PrizeOptions, 1, lottery.PrizeOptions.Count));
        }

        // GET: Activity_LotteryPrizeOption/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var activityLotteryPrizeOption = await db.Activity_LotteryPrizeOptions.Include(p => p.CreateUser).Include(p => p.UpdateUser).FirstOrDefaultAsync(p => p.Id == id);
            if (activityLotteryPrizeOption == null)
                return HttpNotFound();

            return View(activityLotteryPrizeOption);
        }

        // GET: Activity_LotteryPrizeOption/Create
        public ActionResult Create(int lotteryId)
        {
            return View();
        }

        // POST: Activity_LotteryPrizeOption/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int lotteryId, Activity_LotteryPrizeOption activityLotteryPrizeOption)
        {
            var lottery =
               await db.Activity_Lotteries.Include(a => a.PrizeOptions).FirstOrDefaultAsync(p => p.Id == lotteryId);
            if (!ModelState.IsValid) return View(activityLotteryPrizeOption);
            SetModelWithChangeStates(activityLotteryPrizeOption, default(int?));
            activityLotteryPrizeOption.OverCount = activityLotteryPrizeOption.PrizeCount;
            lottery.PrizeOptions.Add(activityLotteryPrizeOption);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { lotteryId = lotteryId });
        }

        // GET: Activity_LotteryPrizeOption/Edit/5
        public async Task<ActionResult> Edit(int lotteryId, int? id)
        {
            var activityLotteryPrizeOption = await db.Activity_LotteryPrizeOptions.FindAsync(id);
            if (activityLotteryPrizeOption == null)
                return HttpNotFound();
            return View(activityLotteryPrizeOption);
        }

        // POST: Activity_LotteryPrizeOption/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int lotteryId, Activity_LotteryPrizeOption activity_LotteryPrizeOption)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(activity_LotteryPrizeOption, activity_LotteryPrizeOption.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index",new { lotteryId = lotteryId });
            }
            return View(activity_LotteryPrizeOption);
        }

        // GET: Activity_LotteryPrizeOption/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var activity_LotteryPrizeOption = await db.Activity_LotteryPrizeOptions.FindAsync(id);
            if (activity_LotteryPrizeOption == null)
                return HttpNotFound();
            return View(activity_LotteryPrizeOption);
        }

        // POST: Activity_LotteryPrizeOption/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            var activityLotteryPrizeOption = await db.Activity_LotteryPrizeOptions.FindAsync(id);
            db.Activity_LotteryPrizeOptions.Remove(activityLotteryPrizeOption);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Activity_LotteryPrizeOption/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Activity_LotteryPrizeOption/BatchOperation/{operation}")]
        public async Task<ActionResult> BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Activity_LotteryPrizeOptions.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Activity_LotteryPrizeOptions.RemoveRange(models);
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