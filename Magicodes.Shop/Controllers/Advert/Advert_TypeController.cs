// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Advert_TypeController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:10
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Advert;
using Magicodes.WeiChat.Data.Models.PhotoGallery;
using Magicodes.WeiChat.Domain.PhotoGallery;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Advert
{
    [RoleMenuFilter("广告管理", "9C7C95C5-B7B6-41B0-AC45-5916791148EF", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-wordpress")]
    public class Advert_TypeController : TenantBaseController<Advert_Type>
    {
        // GET: Advert_Type
        [AuditFilter("广告查询", "fef658e4-c634-41e3-9183-856a4d43498c")]
        [RoleMenuFilter("广告管理", "F84EB368-86BC-4931-AC5C-82B242B5C76E", "Admin,TenantManager,ShopManager",
             url: "/Advert_Type", parentId: "9C7C95C5-B7B6-41B0-AC45-5916791148EF")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Advert_Types.Include(a => a.CreateUser).Include(a => a.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q));
            var pagedList = new PagedList<Advert_Type>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Advert_Type/Details/5
        [AuditFilter("广告详细", "74368df5-b0d8-4ecf-ba2d-c824639270e5")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var advert_Type = await db.Advert_Types.FindAsync(id);
            if (advert_Type == null)
                return HttpNotFound();
            return View(advert_Type);
        }

        // GET: Advert_Type/Create
        [AuditFilter("广告待创建", "02d334dd-a5d6-4390-92e9-a78bc5722d2f")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Advert_Type/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("广告创建", "8ef24063-e404-4e58-b059-863edde73b21")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Name,Width,Height,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Advert_Type
                advert_Type)
        {
            if (ModelState.IsValid)
            {
                advert_Type.Id = Guid.NewGuid();
                SetModelWithChangeStates(advert_Type, default(Guid?));
                db.Advert_Types.Add(advert_Type);

                #region 创建广告相册

                var photoGallery = new Site_PhotoGallery();
                photoGallery.Id = advert_Type.Id;
                photoGallery.Title = "【" + advert_Type.Name + "】-广告相册";
                SetModelWithChangeStates(photoGallery, default(Guid?));
                db.Site_PhotoGallerys.Add(photoGallery);

                #endregion

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(advert_Type);
        }

        // GET: Advert_Type/Edit/5
        [AuditFilter("广告待编辑", "3ad7fedb-0f95-42f1-a31a-6f2a59529e2b")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var advert_Type = await db.Advert_Types.FindAsync(id);
            if (advert_Type == null)
                return HttpNotFound();
            return View(advert_Type);
        }

        // POST: Advert_Type/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("广告编辑", "9460a00b-3aea-4b61-aae1-e4467959e4e9")]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Name,Width,Height,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Advert_Type
                advert_Type)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(advert_Type, advert_Type.Id);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(advert_Type);
        }

        // GET: Advert_Type/Delete/5
        [AuditFilter("广告待删除", "a467e6a3-9eb1-4718-be43-78264633ad16")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var advert_Type = await db.Advert_Types.FindAsync(id);
            if (advert_Type == null)
                return HttpNotFound();
            return View(advert_Type);
        }

        // POST: Advert_Type/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("广告删除", "6e6ab198-2fe8-45c7-b775-348f229cd968")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var advert_Type = await db.Advert_Types.FindAsync(id);
            db.Advert_Types.Remove(advert_Type);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Advert_Type/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Advert_Type/BatchOperation/{operation}")]
        [AuditFilter("广告批量操作", "b92af589-3201-4201-90d0-495e7508ec06")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Advert_Types.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Advert_Types.RemoveRange(models);
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

        /// <summary>
        ///     跳转产品图片页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuditFilter("商品相册编辑", "DE368549-B726-4CD4-9E09-3B627E1770E9")]
        public async Task<ActionResult> Images(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var advert_Type = await db.Advert_Types.FindAsync(id);
            ViewBag.TypeName = advert_Type.Name;
            ViewBag.TypeId = id;
            if (advert_Type == null)
                return HttpNotFound();
            return View(advert_Type);
        }

        /// <summary>
        ///     根据广告类型ID查找该广告所有图片
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ListItems(Guid? Id)
        {
            if (Id == null)
                return HttpNotFound();
            return Json(db.Site_Photos.Where(p => (p.GalleryId == Id) && (p.IsDeleted == false)).ToList(),
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     跳转产品图片上传页面
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("Advert_Type/Upload/{Id}")]
        public ActionResult Upload(Guid? Id)
        {
            //可以接受的文件类型
            //图片（image）: 2M
            //语音（voice）：5M，播放长度不超过60s
            //视频（video）：10MB，支持MP4格式
            //缩略图（thumb）：64KB，支持JPG格式
            var acceptedFilesDic = new Dictionary<int, string>
            {
                {0, ".jpg,.png,.bmp,.jpeg,.gif"}
            };
            //最大上传大小，单位：M
            var maxFilesizeDic = new Dictionary<int, double>
            {
                {0, 10}
            };
            ViewBag.acceptedFiles = acceptedFilesDic[0];
            ViewBag.maxFilesize = maxFilesizeDic[0];
            return View();
        }

        [HttpPost]
        [Route("Advert_Type/Upload/{Id}", Name = "UploadAdvertImagesRoute")]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(Guid Id, string message = null)
        {
            var ajaxMessage = new AjaxResponse { Success = true, Message = "上传成功！" };
            if (Id == null)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = "广告位置不存在或已删除！";
                return Json(ajaxMessage);
            }
            foreach (var fileKey in Request.Files.AllKeys)
            {
                var file = Request.Files[fileKey];
                try
                {
                    if (file != null)
                    {
                        var photoDO = new PhotoDO();
                        photoDO.Add(Id, "广告相册" + Path.GetExtension(file.FileName), file.InputStream);
                    }
                }
                catch (Exception ex)
                {
                    ajaxMessage.Success = false;
                    ajaxMessage.Message = ex.Message;
                }
            }
            return Json(ajaxMessage);
        }
    }
}