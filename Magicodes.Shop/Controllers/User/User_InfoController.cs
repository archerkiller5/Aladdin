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
using Magicodes.WeiChat.Data.Models.User;
using Magicodes.WeiChat.Controllers;

namespace Magicodes.Shop.Controllers.User
{
    [RoleMenuFilter(title: "会员管理", id: "6EBC8796-5F6A-45DD-A9D7-0CD661559179", roleNames: "Admin,TenantManager,ShopManager", iconCls: "fa fa-users")]
    public class User_InfoController : TenantBaseController<User_Info>
    {

        // GET: User_Info
        [AuditFilter("会员信息查询", "3F001666-C5AD-475B-84EF-F9C1DE27A47E")]
        [RoleMenuFilter(title: "会员管理", id: "FDEBF5C6-FC34-40A8-B08C-C12A8AF969C7", roleNames: "Admin,TenantManager,ShopManager", url: "/User_Info", parentId: "6EBC8796-5F6A-45DD-A9D7-0CD661559179")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.User_Infos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                //请替换为相应的搜索逻辑
                queryable = queryable.Where(p => p.Email.Contains(q) || p.Mobile.Contains(q) || p.TrueName.Contains(q));
            }
            var pagedList = new PagedList<User_Info>(
                             await queryable.OrderBy(p => p.OpenId)
                             .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                             pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: User_Info/Details/5
        [AuditFilter("会员信息详细", "955a880b-b3e0-4df5-87c6-6dbbb8013039")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Info user_Info = await db.User_Infos.FindAsync(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // GET: User_Info/Create
        [AuditFilter("会员信息待创建", "df65d3f4-8bdd-4841-a6da-3ed2d19ccfc4")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: User_Info/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("会员信息创建", "dd653235-36ae-4315-8f6b-025ac376bd3b")]
        public async Task<ActionResult> Create([Bind(Include = "OpenId,UserNo,Email,Mobile,State,Integral,Balance,LastLoginOn,LoginCount,TrueName,TenantId,CreateTime")] User_Info user_Info)
        {
            if (ModelState.IsValid)
            {
                user_Info.CreateTime = DateTime.Now;
                user_Info.TenantId = TenantId;
                db.User_Infos.Add(user_Info);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(user_Info);
        }

        // GET: User_Info/Edit/5
        [AuditFilter("会员信息待编辑", "895ee8d7-8b85-405b-b6f3-214fc1c9f23e")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Info user_Info = await db.User_Infos.FindAsync(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // POST: User_Info/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("会员信息编辑", "57313c76-0ca2-4a8b-89fd-60b6fac9b83c")]
        public async Task<ActionResult> Edit([Bind(Include = "OpenId,UserNo,Email,Mobile,State,Integral,Balance,LastLoginOn,LoginCount,TrueName,TenantId,CreateTime")] User_Info user_Info)
        {
            if (ModelState.IsValid)
            {
                var model = await db.User_Infos.FindAsync(user_Info.OpenId);
                model.TrueName = user_Info.TrueName;
                model.Email = user_Info.Email;
                model.Mobile = user_Info.Mobile;
                model.State = user_Info.State;
                model.Integral = user_Info.Integral;
                model.Balance = user_Info.Balance;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user_Info);
        }

        // GET: User_Info/Delete/5
        [AuditFilter("会员信息待删除", "ef95b7c2-58c2-42c3-bd3b-3ef6b491afa7")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Info user_Info = await db.User_Infos.FindAsync(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // POST: User_Info/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("会员信息删除", "d5c940f0-b565-4e29-8abf-67893a6aadc0")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            User_Info user_Info = await db.User_Infos.FindAsync(id);
            db.User_Infos.Remove(user_Info);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: User_Info/BatchOperation/{operation}
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("User_Info/BatchOperation/{operation}")]
        [AuditFilter("会员信息批量操作", "958a39d2-7e26-4b50-96ef-162acfd7420a")]
        public async Task<ActionResult> BatchOperation(string operation, params string[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.User_Infos.Where(p => ids.Contains(p.OpenId)).ToListAsync();
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
                                db.User_Infos.RemoveRange(models);
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
