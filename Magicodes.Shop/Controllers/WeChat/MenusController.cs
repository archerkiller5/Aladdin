// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MenusController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Apis.Menu;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.WeiChat.Infrastructure.MvcExtension.ModelBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Magicodes.Shop.Controllers.WeChat
{
    public class MenusController : TenantBaseController<WeiChat_App>
    {
        // GET: Menus
        public ActionResult Index()
        {
            return View();
        }

        [Route("Menus/Get/{id}")]
        public ActionResult GetByVersion(int id)
        {
            return Json(JsonConvert.DeserializeObject<MenuInfo>(db.WeiChat_Menus.Find(id).MenuData),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get()
        {
            var result = WeChatApisContext.Current.MenuApi.Get();
            var lastData = db.WeiChat_Menus.OrderByDescending(p => p.Id).FirstOrDefault();

            if (result.IsSuccess() && (result.Menu != null) && (result.Menu.Button != null))
            {
                var menuData = JsonConvert.SerializeObject(result.Menu);

                var menu1 = JObject.Parse(result.DetailResult);
                if (lastData != null && lastData.MenuData != null)
                {
                    var menu2 = JObject.Parse(lastData.MenuData);
                    if (!JToken.DeepEquals(menu1, menu2))
                    {
                        //如果不存在历史版本或者菜单内容不一致，则生成一个历史版本保存
                        var menu = new WeiChat_Menu
                        {
                            CreateBy = UserId,
                            CreateTime = DateTime.Now,
                            IsCurrent = true,
                            MenuData = menuData,
                            Remark =
                                string.Format("{0}--{1}", DateTime.Now.ToString("yy-MM-dd HH:mm:ss"),
                                    lastData == null ? "初始化" : "版本冲突"),
                            TenantId = TenantId
                        };
                        db.WeiChat_Menus.Add(menu);
                        db.SaveChanges();
                    }
                }
                return Json(result.Menu, JsonRequestBehavior.AllowGet);
            }
            if (lastData != null)
            {
                var data = JsonConvert.DeserializeObject<MenuInfo>(lastData.MenuData);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json(new List<string>(), JsonRequestBehavior.AllowGet);
        }

        [Route("Menus/VersionList")]
        public ActionResult GetVersionList()
        {
            return Json(
                db.WeiChat_Menus.OrderByDescending(p => p.CreateTime).Select(p => new { p.Remark, p.Id }).ToList(),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPut]
        public ActionResult Put([JsonBinder]MenuInfo menuInfo)
        {
            var ajaxResponse = new AjaxResponse { Success = true, Message = "同步成功！" };
            var result = WeChatApisContext.Current.MenuApi.Create(menuInfo);

            var menu = new WeiChat_Menu
            {
                CreateBy = UserId,
                CreateTime = DateTime.Now,
                IsCurrent = true,
                MenuData = JsonConvert.SerializeObject(menuInfo),
                Remark = string.Format("{0}--用户推送", DateTime.Now.ToString("yy-MM-dd HH:mm:ss")),
                TenantId = TenantId
            };
            menu.Result = result.GetFriendlyMessage();
            db.WeiChat_Menus.Add(menu);
            db.SaveChanges();

            #region 最多只保存30条历史记录

            var count = db.WeiChat_Menus.Count();
            var takCount = 30;
            if (count > takCount)
            {
                db.WeiChat_Menus.RemoveRange(db.WeiChat_Menus.OrderByDescending(p => p.CreateTime).Skip(takCount));
                db.SaveChanges();
            }

            #endregion

            if (!result.IsSuccess())
            {
                ajaxResponse.Success = false;
                ajaxResponse.Message = result.GetFriendlyMessage();
                return Json(ajaxResponse);
            }
            return Json(ajaxResponse);
        }
    }
}