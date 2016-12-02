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
using Magicodes.WeiChat.Data.Models.WeChatStore;
using Senparc.Weixin.MP.AdvancedAPIs;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Infrastructure;
using System.IO;
using Magicodes.Logger;
using Magicodes.WeChat.SDK.Apis.POI;
using System.Text;

namespace Magicodes.Shop.Controllers.Card
{
    [RoleMenuFilter("卡券管理", "{7B5BEA6B-04BA-4099-83B4-EF5FAD826A65}", "Admin,TenantManager,ShopManager",
     iconCls: "fa fa-cc-discover")]
    public class Store_InfoController : TenantBaseController<Store_Info>
    {
        [HttpPost]
        public ActionResult Upload()
        {
            var ajaxMessage = new AjaxResponse { Success = true, Message = "上传成功！" };
            foreach (var fileKey in Request.Files.AllKeys)
            {
                var file = Request.Files[fileKey];
                try
                {
                    var uploadFileBytes = new byte[file.ContentLength];
                    using (file.InputStream)
                    {
                        var result = WeChatApisContext.Current.POIApi.UploadImage(Path.GetFileName(file.FileName), file.InputStream);
                        if (result.IsSuccess())
                            return Json(result, JsonRequestBehavior.AllowGet);
                        else
                        {
                            ajaxMessage.Success = false;
                            ajaxMessage.Message = result.GetFriendlyMessage();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ajaxMessage.Success = false;
                    ajaxMessage.Message = ex.Message;
                    Logger.Log(LoggerLevels.Error, ex.ToString());
                }
            }
            if (!ajaxMessage.Success)
            {
                Response.StatusCode = 400;
                return Content(ajaxMessage.Message);
            }
            return Json(ajaxMessage);
        }
        public ActionResult GetStoreInfo(int? id)
        {
            if (id == null)
            {
                return Json(new Store_Info() { }, JsonRequestBehavior.AllowGet);
            }
            return Json(db.Store_Info.Find(id.Value), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取门店类目
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCategoryList()
        {
            var result = WeChatApisContext.Current.POIApi.GetCategoryList();
            if (result.IsSuccess())
            {
                return Json(result.CategoryList, JsonRequestBehavior.AllowGet);
            }
            Logger.Log(Magicodes.Logger.LoggerLevels.Error, result.DetailResult);
            return Json(new AjaxResponse() { Success = false, Message = "获取类目失败！" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(Store_Info store_Info)
        {
            var ajaxMessage = new AjaxResponse { Success = true, Message = "操作成功！" };
            if (!ModelState.IsValid)
            {
                ajaxMessage.Success = false;
                var sb = new StringBuilder();
                foreach (var item in ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        sb.AppendFormat("{0}<br />", error.ErrorMessage);
                    }
                }
                ajaxMessage.Message = sb.ToString();
                return Json(ajaxMessage);
            }

            var model = new POIInfo()
            {
                Address = store_Info.Address,
                AvgPrice = store_Info.AvgPrice,
                BranchName = store_Info.BranchName,
                Categorys = store_Info.Categorys.Split(';'),
                City = store_Info.City,
                District = store_Info.District,
                Introduction = store_Info.Introduction,
                Latitude = store_Info.Latitude,
                Longitude = store_Info.Longitude,
                Name = store_Info.Name,
                OpenTime = store_Info.OpenTime,
                Province = store_Info.Province,
                Recommend = store_Info.Recommend,
                Special = store_Info.Special,
                SID = Guid.NewGuid().ToString("N"),
                Telephone = store_Info.Telephone,
                PhotoList = new List<POIPhotoInfo>()
            };
            if (!string.IsNullOrEmpty(store_Info.PhotoList))
            {
                var photos = store_Info.PhotoList.Split(',');
                foreach (var item in photos)
                {
                    model.PhotoList.Add(new POIPhotoInfo() { PhotoUrl = item });
                }
            }

            var poiResult = WeChatApisContext.Current.POIApi.Add(model);
            if (!poiResult.IsSuccess())
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = poiResult.GetFriendlyMessage();
            }
            else
            {
                SetModelWithChangeStates(store_Info, default(int));
                db.SaveChanges();
            }
            return Json(ajaxMessage);
        }

        // GET: Store_Info
        [AuditFilter("门店查询", "f704a588-6656-41cf-aec2-5673cf48d9c9")]
        [RoleMenuFilter("门店管理", "f704a588-6656-41cf-aec2-5673cf48d9c9", "Admin,TenantManager,ShopManager",
             url: "/Store_Info", parentId: "{7B5BEA6B-04BA-4099-83B4-EF5FAD826A65}")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Store_Info.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                //请替换为相应的搜索逻辑
                queryable = queryable.Where(p => p.Name.Contains(q) || p.BranchName.Contains(q) || p.Province.Contains(q) || p.City.Contains(q) || p.District.Contains(q) || p.Address.Contains(q) || p.Telephone.Contains(q) || p.Categorys.Contains(q) || p.Recommend.Contains(q) || p.Special.Contains(q) || p.Introduction.Contains(q));
            }
            var pagedList = new PagedList<Store_Info>(
                             await queryable.OrderBy(p => p.Id)
                             .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                             pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Store_Info/Details/5
        [AuditFilter("门店详细", "d5787a95-19d8-4f71-bcbe-c27b60ccda43")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store_Info store_Info = await db.Store_Info.Include(path => path.CreateUser).Include(path => path.UpdateUser).FirstOrDefaultAsync(p => p.Id == id);
            if (store_Info == null)
            {
                return HttpNotFound();
            }
            return View(store_Info);
        }

        // GET: Store_Info/Create
        [AuditFilter("门店待创建", "9b10e49e-54bc-48db-a6c4-ee51e666653a")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Store_Info/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("门店创建", "c38e6447-fb2d-4953-b1d7-ed7a5fb3f567")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,BranchName,Province,City,District,Address,Telephone,OneCategory,TwoCategory,Longitude,Latitude,Recommend,Special,Introduction,OpenTime,AvgPrice,PhotoList,CreateBy,CreateTime,UpdateBy,UpdateTime,TenantId")] Store_Info store_Info)
        {
            if (ModelState.IsValid)
            {
                //PoiApi.UploadImage
                //PoiApi.DeletePoi
                SetModelWithChangeStates(store_Info, default(int?));
                db.Store_Info.Add(store_Info);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(store_Info);
        }

        // GET: Store_Info/Edit/5
        [AuditFilter("门店待编辑", "c891d4bf-cb2e-455c-a8aa-b523a26e9ca3")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store_Info store_Info = await db.Store_Info.FindAsync(id);
            if (store_Info == null)
            {
                return HttpNotFound();
            }
            return View(store_Info);
        }

        // POST: Store_Info/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("门店编辑", "389bcfd0-2e3e-4ee1-9f4e-7c3648dc3482")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,BranchName,Province,City,District,Address,Telephone,OneCategory,TwoCategory,Longitude,Latitude,Recommend,Special,Introduction,OpenTime,AvgPrice,PhotoList,CreateBy,CreateTime,UpdateBy,UpdateTime,TenantId")] Store_Info store_Info)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(store_Info, store_Info.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(store_Info);
        }

        // GET: Store_Info/Delete/5
        [AuditFilter("门店待删除", "9d8db288-66e1-4c5e-bb84-ceeacf4643ec")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store_Info store_Info = await db.Store_Info.Include(path => path.CreateUser).Include(path => path.UpdateUser).FirstOrDefaultAsync(p => p.Id == id);
            if (store_Info == null)
            {
                return HttpNotFound();
            }
            return View(store_Info);
        }

        // POST: Store_Info/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("门店删除", "1f509632-e562-4410-974f-db60b91f0e53")]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            Store_Info store_Info = await db.Store_Info.FindAsync(id);
            db.Store_Info.Remove(store_Info);
            if (!string.IsNullOrEmpty(store_Info.PoiId))
                WeChatApisContext.Current.POIApi.Remove(store_Info.PoiId);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Store_Info/BatchOperation/{operation}
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Store_Info/BatchOperation/{operation}")]
        [AuditFilter("门店批量操作", "7c319b2f-3b68-4adc-a213-669ad50a426c")]
        public async Task<ActionResult> BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Store_Info.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Store_Info.RemoveRange(models);
                                foreach (var item in models)
                                {
                                    if (!string.IsNullOrEmpty(item.PoiId))
                                        WeChatApisContext.Current.POIApi.Remove(item.PoiId);
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
