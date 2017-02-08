// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Product_InfoController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:11
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
using Magicodes.WeiChat.Data.Models.PhotoGallery;
using Magicodes.WeiChat.Data.Models.Product;
using Magicodes.WeiChat.Domain.PhotoGallery;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.Shop.Models;
using Senparc.Weixin.Exceptions;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Product
{
    public class Product_InfoController : TenantBaseController<Product_Info>
    {
        [RoleMenuFilter("商品信息管理", "C1C4ABE8-DC94-47E5-B6C8-02141F6E2F57", "Admin,TenantManager,ShopManager",
             url: "/Product_Info", parentId: "704B112A-3FF1-4985-ACED-611FF4DAC71B")]
        // GET: Product_Info
        [AuditFilter("商品信息查询", "08b599e6-0580-43a9-99ed-980bdfea46bc")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable =
                db.Product_Infos.Include(p => p.Category)
                    .Include(p => p.CreateUser)
                    .Include(p => p.UpdateUser)
                    .AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q) || p.Intro.Contains(q) || p.Des.Contains(q));
            var pagedList = new PagedList<Product_Info>(
                await queryable.OrderByDescending(p => p.CreateTime)
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Product_Info/Details/5
        [AuditFilter("商品信息详细", "ac28d363-f79c-4ecf-a89d-d56cffc50f91")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Info =
                await
                    db.Product_Infos.Include(p => p.UpdateUser)
                        .Include(p => p.CreateUser)
                        .FirstOrDefaultAsync(p => p.Id == id);
            if (product_Info == null)
                return HttpNotFound();
            return View(product_Info);
        }

        // GET: Product_Info/Create
        [AuditFilter("商品信息待创建", "f73bc1ea-2b8a-42d1-a6f0-7612718e87c1")]
        public ActionResult Create()
        {
            ViewBag.ResourcesCategoryId = GetCategoryList(null);
            ViewBag.ResourcesTypeId = GetTypeList(null);
            ViewBag.ResourcesTagId = getTagList();
            return View();
        }

        // POST: Product_Info/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AuditFilter("商品信息创建", "7e5deed4-5cad-4ca6-84fc-4bfcb84f4d42")]
        public async Task<ActionResult> Create(Product_Info model, Guid[] attributeId = null,
            string[] attributeName = null, decimal[] attributePrice = null, string[] tagId = null)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                model.State = ProductState.SoldOut; //商品状态初始为下架
                SetModelWithChangeStates(model, default(Guid?));
                db.Product_Infos.Add(model);
                #region 保存商品标签

                if (tagId != null)
                    for (var i = 0; i < tagId.Length; i++)
                    {
                        var pt = new Product_ProductTag();
                        pt.Id = Guid.NewGuid();
                        SetModelWithChangeStates(pt, default(Guid?));
                        pt.ProductId = model.Id;
                        pt.TagId = new Guid(tagId[i]);
                        db.Product_ProductTags.Add(pt);
                    }

                #endregion
                #region 保存商品属性
                if (attributeId != null)
                    for (var i = 0; i < attributeId.Length; i++)
                    {
                        var pa = new Product_ProductAttribute();
                        pa.Id = Guid.NewGuid();
                        SetModelWithChangeStates(pa, default(Guid?));
                        pa.ProductId = model.Id;
                        pa.AttributeName = attributeName[i];
                        pa.AttributeId = attributeId[i];
                        pa.AttributePrice = attributePrice[i];
                        pa.AttributeSort = i;
                        db.Product_ProductAttributes.Add(pa);
                    }

                #endregion
                #region 创建商品相册
                var photoGallery = new Site_PhotoGallery();
                photoGallery.Id = model.Id;
                photoGallery.Title = "【" + model.Name + "】-商品相册";
                SetModelWithChangeStates(photoGallery, default(Guid?));
                db.Site_PhotoGallerys.Add(photoGallery);
                #endregion
                await db.SaveChangesAsync();
                return RedirectToActionPermanent("Images", new { id = model.Id });
            }
            ViewBag.ResourcesCategoryId = GetCategoryList(null);
            ViewBag.ResourcesTypeId = GetTypeList(null);
            ViewBag.ResourcesTagId = getTagList();
            return View(model);
        }

        // GET: Product_Info/Edit/5
        [AuditFilter("商品信息待编辑", "126e2be4-7009-492c-9608-e99f096b8e43")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Info = await db.Product_Infos.FindAsync(id);
            if (product_Info == null)
                return HttpNotFound();
            //获取商品关联标签
            var resourcesTagId = getTagList();
            if (resourcesTagId != null)
                for (var i = 0; i < resourcesTagId.Count; i++)
                {
                    var tagId = resourcesTagId[i].Id;
                    var tag = db.Product_ProductTags.FirstOrDefault(p => (p.TagId == tagId) && (p.ProductId == id));
                    if (tag != null)
                        resourcesTagId[i].IsChecked = true;
                }
            var tagList = db.Product_ProductTags.Where(p => p.ProductId == id).ToList();

            //根据商品关联属性类别获取所有属性列表
            var attributeList = db.Product_Attributes.Where(p => p.TypeId == product_Info.TypeId).ToList();
            if (attributeList != null)
                for (var i = 0; i < attributeList.Count; i++)
                {
                    var attributeId = attributeList[i].Id;
                    //该属性是否有值
                    var productAttr =
                        db.Product_ProductAttributes.FirstOrDefault(
                            p => (p.AttributeId == attributeId) && (p.ProductId == id));
                    if (productAttr != null)
                    {
                        attributeList[i].IsChecked = true;
                        attributeList[i].Price = productAttr.AttributePrice;
                    }
                }
            ViewBag.TagList = tagList;
            ViewBag.AttrList = attributeList;
            ViewBag.ResourcesCategoryId = GetCategoryList(product_Info.CategoryId);
            ViewBag.ResourcesTypeId = GetTypeList(product_Info.TypeId);
            ViewBag.ResourcesTagId = resourcesTagId;
            return View(product_Info);

        }

        // POST: Product_Info/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [AuditFilter("商品信息编辑", "94e65968-ca7d-43de-8b34-ac65b0f6d9b6")]
        public async Task<ActionResult> Edit(Product_Info model, Guid[] attributeId = null,
            string[] attributeName = null, decimal[] attributePrice = null, string[] tagId = null)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(model, model.Id);

                #region 修改商品相册信息

                var photoGallery = db.Site_PhotoGallerys.Find(model.Id);
                photoGallery.Title = "【" + model.Name + "】-商品相册";
                SetModelWithChangeStates(photoGallery, photoGallery.Id);

                #endregion

                await db.SaveChangesAsync();

                #region 保存商品标签

                //先删除所有商品表情
                var tagModels = await db.Product_ProductTags.Where(p => p.ProductId == model.Id).ToListAsync();
                if (tagModels.Count != 0)
                {
                    db.Product_ProductTags.RemoveRange(tagModels);
                    await db.SaveChangesAsync();
                }
                if (tagId != null)
                    for (var i = 0; i < tagId.Length; i++)
                    {
                        var pt = new Product_ProductTag();
                        pt.Id = Guid.NewGuid();
                        SetModelWithChangeStates(pt, default(Guid?));
                        pt.ProductId = model.Id;
                        pt.TagId = new Guid(tagId[i]);
                        db.Product_ProductTags.Add(pt);
                    }
                await db.SaveChangesAsync();

                #endregion

                #region 保存商品属性

                //先删除所有商品属性
                var attrModels = await db.Product_ProductAttributes.Where(p => p.ProductId == model.Id).ToListAsync();
                if (attrModels.Count != 0)
                {
                    db.Product_ProductAttributes.RemoveRange(attrModels);
                    await db.SaveChangesAsync();
                }
                if (attributeId != null)
                    for (var i = 0; i < attributeId.Length; i++)
                    {
                        var pa = new Product_ProductAttribute();
                        pa.Id = Guid.NewGuid();
                        SetModelWithChangeStates(pa, default(Guid?));
                        pa.ProductId = model.Id;
                        pa.AttributeId = attributeId[i];
                        pa.AttributeName = attributeName[i];
                        pa.AttributePrice = attributePrice[i];
                        pa.AttributeSort = i;
                        db.Product_ProductAttributes.Add(pa);
                    }
                await db.SaveChangesAsync();

                #endregion

                return RedirectToAction("Index");
            }
            return View(model);
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
            var product_Info = await db.Product_Infos.FindAsync(id);
            ViewBag.ProductName = product_Info.Name;
            ViewBag.ProductId = id;
            if (product_Info == null)
                return HttpNotFound();
            return View(product_Info);
        }

        /// <summary>
        ///     根据产品ID查找该产品所有图片
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
        /// 跳转产品图片上传页面
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("Product_Info/Upload/{Id}")]
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
        [Route("Product_Info/Upload/{Id}", Name = "UploadProductImagesRoute")]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(Guid Id, string message = null)
        {
            var ajaxMessage = new AjaxResponse { Success = true, Message = "上传成功！" };
            if (Id == null)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = "产品不存在或已删除！";
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
                        photoDO.Add(Id, "商品相册" + Path.GetExtension(file.FileName), file.InputStream);
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

        // GET: Product_Info/Delete/5
        [AuditFilter("商品信息待删除", "3c7e83f5-3029-46be-90e2-f73d35b39924")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product_Info = await db.Product_Infos.FindAsync(id);
            if (product_Info == null)
                return HttpNotFound();
            return View(product_Info);
        }

        /// <summary>
        ///     删除商品图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [AuditFilter("商品相片删除", "77B45B23-B274-4FEB-994F-66712E746EC9")]
        public ActionResult Delete(Guid id)
        {
            var response = new AjaxResponse {
                Success = true,
                Message = "删除成功！"
            };
            var site_Photo = db.Site_Photos.Find(id);
            if (site_Photo == null)
                return HttpNotFound();
            //删除相关网站文件
            var dirPath = Server.MapPath("~" + site_Photo.Url);
            if (System.IO.File.Exists(dirPath))
                System.IO.File.Delete(dirPath);
            db.Site_Photos.Remove(site_Photo);
            db.SaveChanges();
            //如果该商品没有图片，则将商品状态改为下架
            if (!db.Site_Photos.Where(p => p.GalleryId == site_Photo.GalleryId).Any())
            {
                var product_Info = db.Product_Infos.Find(site_Photo.GalleryId);
                product_Info.State = ProductState.SoldOut;
                db.SaveChanges();
            }
            return Json(response);
        }

        // POST: Product_Info/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("商品信息删除", "c3c5c44f-0946-4185-bfc2-ee19584a4510")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var product_Info = await db.Product_Infos.FindAsync(id);
            db.Product_Infos.Remove(product_Info);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Product_Info/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Product_Info/BatchOperation/{operation}")]
        [AuditFilter("商品信息批量操作", "63afe612-bd3a-4cef-b9e1-a21699519f02")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Product_Infos.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Product_Infos.RemoveRange(models);
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

        public List<Product_Tag> getTagList()
        {
            return db.Product_Tags.OrderBy(p => p.CreateTime).ToList();
        }

        /// <summary>
        ///     获取商品属性类别列表
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public SelectList GetTypeList(Guid? typeID)
        {
            return new SelectList(db.Product_Types.OrderBy(p => p.CreateTime).ToList(), dataTextField: "Name",
                dataValueField: "Id", selectedValue: typeID);
        }

        /// <summary>
        ///     获取商品类目列表
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public SelectList GetCategoryList(Guid? categoryID)
        {
            return new SelectList(db.Product_Categorys.Where(p => p.IsDisplay).OrderBy(p => p.Sort).ToList(),
                dataTextField: "Name", dataValueField: "Id", selectedValue: categoryID);
        }

        /// <summary>
        ///     获取商品属性列表
        /// </summary>
        /// <param name="TypeId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetAttributeList(Guid? TypeId)
        {
            //var ajaxResponse = new AjaxResponse<List<Product_Attribute>>();
            //var attributeList =
            //    await db.Product_Attributes.Where(p => p.TypeId == TypeId).OrderBy(p => p.Sort).ToListAsync();
            //ajaxResponse.Result = attributeList;
            //ajaxResponse.Success = true;
            //return Json(ajaxResponse);
            var ajaxResponse = new AjaxResponse<List<Product_Type>>();
            var attributeList =
                await db.Product_Types.Where(p => p.Id == TypeId).ToListAsync();
            ajaxResponse.Result = attributeList;
            ajaxResponse.Success = true;
            return Json(ajaxResponse);
        }

        /// <summary>
        ///     商品上架、下架
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ActionResult EditProductState(Guid? productId, int state)
        {
            var message = new MessageInfo
            {
                Message = "操作成功！",
                MessageType = MessageTypes.Success
            };
            try
            {
                //上架之前先判断是否有照片
                if(state == 0 && !db.Site_Photos.Where(p => p.GalleryId == productId).Any())
                {
                    message.Message = "至少上传一张照片商品才能上架";
                    message.MessageType = MessageTypes.Danger;
                }else
                {
                    var info = db.Product_Infos.Find(productId);
                    if (state == 0)
                        info.State = ProductState.OnSell;
                    else
                        info.State = ProductState.SoldOut;

                    if (db.SaveChanges() == 0)
                    {
                        message.Message = "操作失败";
                        message.MessageType = MessageTypes.Danger;
                    }
                }
            }
            catch (ErrorJsonResultException ex)
            {
                message.Message = ex.Message;
                message.MessageType = MessageTypes.Danger;
            }
            return Json(message);
        }
    }
}