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
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Site;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Shop.Controllers.Site
{
    public class Site_NotifyController : BaseController
    {

        // GET: Site_Notify
        [AuditFilter("通知查询", "c394a874-020b-46b5-a84b-20e6d66df855")]
        [RoleMenuFilter("站内通知", "{F8851A9D-C2B2-4F6E-A022-64F588B1E5C3}", "Admin,TenantManager,ShopManager",
     iconCls: "fa fa-bell-o", parentId: "{99A31780-B8C5-4AF6-ACBB-4598C8A60661}", url: "/Site_Notify")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var userId = UserId;
            var userKey = "User_" + userId;
            var tenantId = TenantId;
            var tenantKey = "Tenant_" + tenantId;

            var queryable = db.Site_Notifies.Where(p => (p.Receiver == userKey || p.Receiver == tenantKey));
            if (!string.IsNullOrWhiteSpace(q))
            {
                //请替换为相应的搜索逻辑
                queryable = queryable.Where(p => p.Message.Contains(q) || p.Title.Contains(q));
            }
            var pagedList = new PagedList<Site_Notify>(
                             queryable.OrderByDescending(p => p.UpdateTime)
                             .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList().Select(p => new Site_Notify
                             {
                                 CreateTime = p.CreateTime,
                                 CreateBy = db.Users.FirstOrDefault(p1 => p1.Id == p.CreateBy).UserName,
                                 HasRead = db.Site_ReadNotifies.Any(p1 => p1.NotifyId == p.Id && p1.CreateBy == userId),
                                 Id = p.Id,
                                 Href = p.Href,
                                 IconCls = p.IconCls,
                                 IsTaskFinish = p.IsTaskFinish,
                                 Message = p.Message,
                                 TaskPercentage = p.TaskPercentage,
                                 Receiver = p.Receiver,
                                 Title = p.Title,
                                 UpdateTime = p.UpdateTime
                             }),
                             pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Site_Notify/Details/5
        [AuditFilter("通知详细", "68ae0f2a-5f1f-4321-be64-c4afe805f571")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Site_Notify site_Notify = await db.Site_Notifies.FindAsync(id);
            if (site_Notify == null)
            {
                return HttpNotFound();
            }
            return View(site_Notify);
        }

        // POST: Site_Notify/BatchOperation/{operation}
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Site_Notify/BatchOperation/{operation}")]
        [AuditFilter("通知批量操作", "7cba9dcd-80c0-4613-b9f4-0755c9b76425")]
        public async Task<ActionResult> BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Site_Notifies.Where(p => ids.Contains(p.Id)).ToListAsync();
                    if (models.Count == 0)
                    {
                        ajaxResponse.Success = false;
                        ajaxResponse.Message = "没有找到匹配的项，项已被删除或不存在！";
                        return Json(ajaxResponse);
                    }
                    switch (operation.ToUpper())
                    {
                        case "READ":
                            #region 设置为已读
                            {
                                foreach (var item in models)
                                {
                                    var uid = UserId;
                                    if (!db.Site_ReadNotifies.Any(p => p.NotifyId == item.Id && p.CreateBy == uid))
                                    {
                                        var read = new Site_ReadNotify()
                                        {
                                            CreateBy = uid,
                                            CreateTime = DateTime.Now,
                                            NotifyId = item.Id
                                        };
                                        db.Site_ReadNotifies.Add(read);
                                    }
                                }
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
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
