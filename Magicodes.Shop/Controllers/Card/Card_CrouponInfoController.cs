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
using Magicodes.WeiChat.Data.Models.Card;
using Magicodes.WeChat.SDK.Apis.Card;
using Magicodes.WeiChat.Unity;

namespace Magicodes.Shop.Controllers.Card
{

    public class Card_CrouponInfoController : TenantBaseController<Card_CrouponInfo>
    {
        public ActionResult GetCardTypes()
        {
            return GetJsonByEnumType<CardTypes>();
        }

        public ActionResult GetCodeTypes()
        {
            return GetJsonByEnumType<CodeTypes>();
        }

        public ActionResult GetDateInfoTypes()
        {
            return GetJsonByEnumType<DateInfoTypes>();
        }

        private ActionResult GetJsonByEnumType<T>()
        {
            var list = new List<object>();
            var names = Enum.GetNames(typeof(T));
            foreach (var item in names)
            {
                var text = ((Enum)Enum.Parse(typeof(T), item)).GetEnumMemberDisplayName();
                list.Add(new
                {
                    value = item,
                    text = text
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Upload(string name)
        {
            var ajaxMessage = new AjaxResponse { Success = true, Message = "上传成功！" };
            var fileSource = Request.Unvalidated.Form["data"];
            byte[] bytes = Convert.FromBase64String(fileSource.Split(',')[1]);
            using (var ms = new MemoryStream(bytes))
            {
                var result = WeChatApisContext.Current.CardApi.UploadImage(name, ms);
                if (result.IsSuccess())
                    return Json(result, JsonRequestBehavior.AllowGet);
                else
                {
                    ajaxMessage.Success = false;
                    ajaxMessage.Message = result.GetFriendlyMessage();
                }
            }
            //if (!ajaxMessage.Success)
            //{
            //    Response.StatusCode = 400;
            //    return Content(ajaxMessage.Message);
            //}
            return Json(ajaxMessage);
        }

        [HttpPost]
        public ActionResult Add(string cardInfo)
        {
            var ajaxMessage = new AjaxResponse { Success = true, Message = "操作成功！" };

            var obj = WeChatApisContext.Current.CardApi.GetCardInfoByJson(cardInfo);
            var result = WeChatApisContext.Current.CardApi.Add(obj);
            if (!result.IsSuccess())
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = result.GetFriendlyMessage();
            }
            else
            {
                var croup = new Card_CrouponInfo()
                {
                    CardType = obj.CardType,
                    Data = cardInfo,
                };
                switch (obj.CardType)
                {
                    case CardTypes.GROUPON:
                        croup.Title = (obj as Groupon).Groupon_.BaseInfo.Title;
                        break;
                    case CardTypes.CASH:
                        croup.Title = (obj as Cash).cash.base_info.Title;
                        break;
                    case CardTypes.DISCOUNT:
                        croup.Title = (obj as Discount).discount.base_info.Title;
                        break;
                    case CardTypes.GIFT:
                        croup.Title = (obj as Gift).gift.base_info.Title;
                        break;
                    case CardTypes.GENERAL_COUPON:
                        croup.Title = (obj as GeneralCoupon).general_coupon.base_info.Title;
                        break;
                    default:
                        break;
                }
                SetModelWithChangeStates(croup, default(int));
                db.SaveChanges();
            }
            return Json(ajaxMessage);
        }

        // GET: Card_CrouponInfo
        [AuditFilter("卡券管理", "{32B2FC1E-B073-4791-8B55-66960E88149D}")]
        [RoleMenuFilter("卡券管理", "{32B2FC1E-B073-4791-8B55-66960E88149D}", "Admin,TenantManager,ShopManager",
             url: "/Card_CrouponInfo", parentId: "{7B5BEA6B-04BA-4099-83B4-EF5FAD826A65}")]
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10)
        {
            var queryable = db.Card_CrouponInfos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                //请替换为相应的搜索逻辑
                queryable = queryable.Where(p => p.Title.Contains(q));
            }
            var pagedList = new PagedList<Card_CrouponInfo>(
                             await queryable.OrderBy(p => p.Id)
                             .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                             pageIndex, pageSize, await queryable.CountAsync());
            return View(pagedList);
        }

        // GET: Card_CrouponInfo/Details/5
        [AuditFilter("卡券详细", "{B5A0C1B4-EBAD-41BC-877F-6F64AAAFC6E0}")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = await db.Card_CrouponInfos.Include(path => path.CreateUser).Include(path => path.UpdateUser).FirstOrDefaultAsync(p => p.Id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Card_CrouponInfo/Create
        [AuditFilter("卡券待创建", "{75D22363-6D9A-424E-B9B6-4874DD982F7F}")]
        public ActionResult Create()
        {
            return View();
        }



        // GET: Card_CrouponInfo/Delete/5
        [AuditFilter("卡券待删除", "{4C96DEA8-7E25-4575-BF07-57C29050D8CF}")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = await db.Card_CrouponInfos.Include(path => path.CreateUser).Include(path => path.UpdateUser).FirstOrDefaultAsync(p => p.Id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Card_CrouponInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditFilter("卡券删除", "{DD6CB12E-9CBC-4FA1-8803-49659CE3F78B}")]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            var model = await db.Card_CrouponInfos.FindAsync(id);
            db.Card_CrouponInfos.Remove(model);
            //if (!string.IsNullOrEmpty(model.PoiId))
            //    WeChatApisContext.Current.POIApi.Remove(store_Info.PoiId);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: Card_CrouponInfo/BatchOperation/{operation}
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="operation">操作方法</param>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Card_CrouponInfo/BatchOperation/{operation}")]
        [AuditFilter("卡券批量操作", "{A356C4DC-CB88-49FA-9D38-882D01434E16}")]
        public async Task<ActionResult> BatchOperation(string operation, params int?[] ids)
        {
            var ajaxResponse = new AjaxResponse();
            if (ids.Length > 0)
            {
                try
                {
                    var models = await db.Card_CrouponInfos.Where(p => ids.Contains(p.Id)).ToListAsync();
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
                                db.Card_CrouponInfos.RemoveRange(models);
                                foreach (var item in models)
                                {
                                    //if (!string.IsNullOrEmpty(item.PoiId))
                                    //    WeChatApisContext.Current.POIApi.Remove(item.PoiId);
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
