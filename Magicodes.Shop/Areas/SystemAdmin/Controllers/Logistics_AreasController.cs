// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Logistics_AreasController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:02
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
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.Shop.Controllers;
using Magicodes.WeiChat.Data.Models.Logistics;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Areas.SystemAdmin.Controllers
{
    [RoleMenuFilter("物流地区管理", "8250B784-992C-44E4-8B71-55F0B275684D", "Admin", iconCls: "fa fa-send",
         tag: "System")]
    public class Logistics_AreasController : BaseController
    {
        // GET: Logistics_Areas 
        [RoleMenuFilter("地区设置", "88736E50-EC62-4846-9981-FC2FF583CEF1", "Admin", url: "/SystemAdmin/Logistics_Areas",
             parentId: "8250B784-992C-44E4-8B71-55F0B275684D", tag: "System")]
        [AuditFilter("物流地区管理页面", "83e28a94-c7ff-4d38-9ca1-d93b152c7132")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Logistics_Areas.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.AreaName.Contains(q) || p.ParentId.Contains(q) || p.Pinyinma.Contains(q) ||
                            p.PostCode.Contains(q));
            var pagedList = new PagedList<Logistics_Area>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Logistics_Areas/Details/5
        [AuditFilter("物流地区详情页面", "0091ec6a-be61-4d60-bd68-d7fd2f998af4")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_Areas = await db.Logistics_Areas.FindAsync(id);
            if (logistics_Areas == null)
                return HttpNotFound();
            return View(logistics_Areas);
        }

        // GET: Logistics_Areas/Create
        [AuditFilter("物流地区创建页面", "a0770e82-c0f4-42c7-b415-a6743e96ecbe")]
        public ActionResult Create()
        {
            //获取当前的省级集合
            //List<Logistics_Areas> lst_c = db.Logistics_Areass.Where(P => P.AreaLevel == Logistics_AreaLevel.Country).OrderBy(p => p.SortNumber).ToList();

            var queryable = db.Logistics_Areas.AsQueryable();
            var lst_c =
                queryable.Where(p => p.AreaLevel == Logistics_AreaLevel.Country).OrderBy(p => p.SortNumber).ToList();
            var lst_p =
                queryable.Where(p => p.AreaLevel == Logistics_AreaLevel.Province).OrderBy(p => p.SortNumber).ToList();
            var lst_ct =
                queryable.Where(p => p.AreaLevel == Logistics_AreaLevel.City).OrderBy(p => p.SortNumber).ToList();
            ViewBag.Country = lst_c;
            ViewBag.Province = lst_p;
            ViewBag.City = lst_ct;
            return View();
        }

        public ActionResult GetAreas(string ParaentId)
        {
            var queryable = db.Logistics_Areas.AsQueryable();
            var lst_area = queryable.Where(p => p.ParentId == ParaentId).OrderBy(p => p.SortNumber).ToList();
            return Json(lst_area);
        }

        // POST: Logistics_Areas/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流地区创建", "205180fb-a38c-4bde-8225-f9ab83421edf")]
        public async Task<ActionResult> Create(
            [Bind(
                 Include =
                     "Id,AreaName,AreaLevel,ParentId,Pinyinma,PostCode,SortNumber,CreateTime,UpdateTime,CreateBy,UpdateBy"
             )] Logistics_Area logistics_Areas)
        {
            if (ModelState.IsValid)
            {
                logistics_Areas.CreateTime = DateTime.Now;
                logistics_Areas.CreateBy = UserId;
                //id和排序序号要后台生成..
                var id = GetPrimaryKey(logistics_Areas);
                logistics_Areas.SortNumber = GetSortNumber(logistics_Areas);
                logistics_Areas.Id = id;
                db.Logistics_Areas.Add(logistics_Areas);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(logistics_Areas);
        }

        private string GetPrimaryKey(Logistics_Area area)
        {
            var result = "";
            var queryable = db.Logistics_Areas.AsQueryable();
            var maxId =
                queryable.Where(p => (p.AreaLevel == area.AreaLevel) && (p.ParentId == area.ParentId)).Max(p => p.Id);
            var headValue = 0;
            switch (area.AreaLevel)
            {
                case Logistics_AreaLevel.Country:
                    headValue = int.Parse(maxId.Substring(0, maxId.Length - 5));
                    headValue++;
                    result = headValue + "00000";
                    break;
                case Logistics_AreaLevel.Province:
                    headValue = int.Parse(maxId.Substring(0, maxId.Length - 4));
                    headValue++;
                    result = headValue + "0000"; //maxId.Substring(headValue.ToString().Length - 1);
                    break;
                case Logistics_AreaLevel.City:
                    headValue = int.Parse(maxId.Substring(0, maxId.Length - 2));
                    headValue++;
                    result = headValue + "00"; //maxId.Substring(3) + headValue.ToString();
                    break;
                default:
                    headValue = int.Parse(maxId) + 1;
                    result = headValue.ToString();
                    break;
            }

            return result;
        }

        private int GetSortNumber(Logistics_Area area)
        {
            var queryable = db.Logistics_Areas.AsQueryable();
            var sort = queryable.Max(p => p.SortNumber);
            sort++;
            return sort;
        }

        // GET: Logistics_Areas/Edit/5
        [AuditFilter("物流地区维护页面", "ad58c148-ec1c-487b-99fd-453b15000cc0")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_Areas = await db.Logistics_Areas.FindAsync(id);
            if (logistics_Areas == null)
                return HttpNotFound();
            var queryable = db.Logistics_Areas.AsQueryable();
            var lst_c =
                queryable.Where(p => p.AreaLevel == Logistics_AreaLevel.Country).OrderBy(p => p.SortNumber).ToList();
            var lst_p =
                queryable.Where(p => p.AreaLevel == Logistics_AreaLevel.Province).OrderBy(p => p.SortNumber).ToList();
            var lst_ct =
                queryable.Where(p => p.AreaLevel == Logistics_AreaLevel.City).OrderBy(p => p.SortNumber).ToList();
            ViewBag.Country = lst_c;
            ViewBag.Province = lst_p;
            ViewBag.City = lst_ct;

            return View(logistics_Areas);
        }

        // POST: Logistics_Areas/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流地区维护", "f9bd0821-dd4f-4e32-962b-497aae135133")]
        public async Task<ActionResult> Edit(
            [Bind(
                 Include =
                     "Id,AreaName,AreaLevel,ParentId,Pinyinma,PostCode,SortNumber,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId"
             )] Logistics_Area logistics_Areas)
        {
            if (ModelState.IsValid)
            {
                logistics_Areas.CreateTime = DateTime.Now;
                logistics_Areas.CreateBy = UserId;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(logistics_Areas);
        }

        // GET: Logistics_Areas/Delete/5
        [AuditFilter("物流地区删除页面", "6cd23988-29e1-4083-9fde-f0b9df528e1f")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var logistics_Areas = await db.Logistics_Areas.FindAsync(id);
            if (logistics_Areas == null)
                return HttpNotFound();
            return View(logistics_Areas);
        }

        // POST: Logistics_Areas/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("物流地区删除", "b903912d-abd6-492b-abd3-6a4ac86a4008")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var logistics_Areas = await db.Logistics_Areas.FindAsync(id);
            db.Logistics_Areas.Remove(logistics_Areas);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Logistics_Areas/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Logistics_Areas/BatchOperation/{operation}")]
        [AuditFilter("物流地区批量操作", "6a0f7548-5bd5-4018-9b2f-b8f9e365ffbf")]
        public async Task<ActionResult> BatchOperation(string operation, params string[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Logistics_Areas.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Logistics_Areas.RemoveRange(models);
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