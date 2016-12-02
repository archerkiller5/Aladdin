// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Shop_InfoController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models.PhotoGallery;
using Magicodes.WeiChat.Data.Models.Shop;
using Magicodes.WeiChat.Domain.PhotoGallery;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.Shop
{
    [RoleMenuFilter("店铺管理", "F68904FD-2A45-4E28-AC54-97AB948BBAFE", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-home")]
    public class Shop_InfoController : TenantBaseController<Shop_Info>
    {
        // GET: Shop_Info
        [AuditFilter("店铺信息查询", "ba3f4803-3aef-4e17-ac3b-5cc5aa6b33ba")]
        [RoleMenuFilter("店铺信息", "44146D3C-3F06-4182-967A-CF17B51B86C3", "Admin,TenantManager,ShopManager",
             url: "/Shop_Info", parentId: "F68904FD-2A45-4E28-AC54-97AB948BBAFE")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Shop_Infos.Include(s => s.CreateUser).Include(s => s.UpdateUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                queryable = queryable.Where(p => p.Name.Contains(q) || p.Contact.Contains(q) || p.Describe.Contains(q));
            var pagedList = new PagedList<Shop_Info>(
                await queryable.OrderBy(p => p.Id)
                    .Skip((pageIndex - 1)*pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, queryable.Count());
            return View(pagedList);
        }

        // GET: Shop_Info/Details/5
        [AuditFilter("店铺详细信息", "c913d7ff-3570-4302-b636-502dbe5919aa")]
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var shop_Info = await db.Shop_Infos.Include(p => p.CreateUser).FirstOrDefaultAsync(p => p.Id == id);
            if (shop_Info == null)
                return HttpNotFound();
            return View(shop_Info);
        }

        // GET: Shop_Info/Create
        [AuditFilter("店铺待创建", "c422d44e-070d-486f-b24a-4a88c0b35b49")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Shop_Info/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("店铺创建", "b47522f5-f549-4c5e-b896-297279c080b9")]
        public async Task<ActionResult> Create(
            [Bind(Include = "Id,Name,Log,Contact,Describe,Theme,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Shop_Info shop_Info)
        {
            if (ModelState.IsValid)
            {
                shop_Info.Id = Guid.NewGuid();
                SetModelWithChangeStates(shop_Info, default(Guid?));

                //创建店铺相册
                var photoGallery = new Site_PhotoGallery();
                photoGallery.Id = shop_Info.Id;
                photoGallery.Title = "【" + shop_Info.Name + "】-店铺相册";
                SetModelWithChangeStates(photoGallery, default(Guid?));
                db.Site_PhotoGallerys.Add(photoGallery);
                //上传店铺LOGO
                var file = Request.Files[0];
                if (file != null)
                {
                    var photoDO = new PhotoDO();
                    var logoGuid = photoDO.Add(shop_Info.Id,
                        "【" + shop_Info.Name + "】-店铺LOGO" + Path.GetExtension(file.FileName), file.InputStream);
                    shop_Info.Logo = logoGuid;
                }
                db.Shop_Infos.Add(shop_Info);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(shop_Info);
        }

        // GET: Shop_Info/Edit/5
        [AuditFilter("店铺待编辑", "5d71d114-acaf-4081-8518-7bc2bab48025")]
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var shop_Info = await db.Shop_Infos.FindAsync(id);
            if (shop_Info == null)
                return HttpNotFound();
            return View(shop_Info);
        }

        // POST: Shop_Info/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditFilter("店铺编辑", "4b08be5a-bc3e-4824-bdb9-f3da89fbc6c5")]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Id,Name,Log,Contact,Describe,Theme,CreateTime,UpdateTime,CreateBy,UpdateBy,TenantId")] Shop_Info shop_Info)
        {
            if (ModelState.IsValid)
            {
                SetModelWithChangeStates(shop_Info, shop_Info.Id);
                //上传店铺LOGO
                var file = Request.Files[0];
                if (file != null)
                {
                    var photo = db.Site_Photos.FirstOrDefault(p => p.GalleryId == shop_Info.Id);
                    db.Site_Photos.Remove(photo);
                    var photoDO = new PhotoDO();
                    var logoGuid = photoDO.Add(shop_Info.Id,
                        "【" + shop_Info.Name + "】-店铺LOGO" + Path.GetExtension(file.FileName), file.InputStream);
                    shop_Info.Logo = logoGuid;
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(shop_Info);
        }

        // GET: Shop_Info/Delete/5
        [AuditFilter("店铺待删除", "2c6800a9-f912-4aec-863a-fa91437b900b")]
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var shop_Info = await db.Shop_Infos.FindAsync(id);
            if (shop_Info == null)
                return HttpNotFound();
            return View(shop_Info);
        }

        // POST: Shop_Info/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("店铺删除", "339dc827-9f9d-436b-ae79-4191b2a9df4e")]
        public async Task<ActionResult> DeleteConfirmed(Guid? id)
        {
            var shop_Info = await db.Shop_Infos.FindAsync(id);
            db.Shop_Infos.Remove(shop_Info);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Shop_Info/BatchOperation/{operation}
        /// <summary>
        ///     批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Shop_Info/BatchOperation/{operation}")]
        [AuditFilter("店铺批量操作", "0d11c77c-d473-4edf-be73-af8d7c339023")]
        public async Task<ActionResult> BatchOperation(string operation, params Guid?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Shop_Infos.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                            db.Shop_Infos.RemoveRange(models);
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