// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : CustomController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Magicodes.Shop.Models;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data.Models;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat
{
    [Authorize]
    public class CustomController : TenantBaseController<WeiChat_KFCInfo>
    {
        // GET: Custom
        public ActionResult Index(int pageIndex = 1, int pageSize = 20)
        {
            var data = CustomServiceApi.GetCustomBasicInfo(AccessToken);
            var dataList = data.kf_list
                .Select(p =>
                    new CustomViewModel
                    {
                        Id = p.kf_id,
                        NickName = p.kf_nick,
                        Account = p.kf_account,
                        HeadImgUrl = p.kf_headimgurl
                    });
            var pagedList = new PagedList<CustomViewModel>(dataList, pageIndex, pageSize, dataList.Count());
            return View(pagedList);
        }

        public ActionResult Add()
        {
            return View();
        }

        [Route("Edit/{id}")]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var kf = db.WeiChat_KFCInfos.FirstOrDefault(p => p.Account == id);
            if (kf == null)
            {
                ModelState.AddModelError("", "该账号不存在！");
            }
            else
            {
                var addModel = new AddCustomViewModel
                {
                    Account = kf.Account.Split('@')[0],
                    NickName = kf.NickName
                };
                return View(addModel);
            }
            return View(new AddCustomViewModel());
        }

        [Route("Edit/{id}")]
        [HttpPost]
        public ActionResult Edit(string id, AddCustomViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var result = CustomServiceApi.UpdateCustom(
                    AccessToken,
                    id,
                    model.NickName,
                    model.Password);
                if (result.errcode != ReturnCode.请求成功)
                {
                    ViewBag.Messages = new List<MessageInfo>
                    {
                        new MessageInfo
                        {
                            Message = result.errmsg,
                            MessageType = MessageTypes.Danger
                        }
                    };
                    return View(model);
                }
                var kf = db.WeiChat_KFCInfos.First(p => p.Account == id);
                kf.NickName = model.NickName;
                db.SaveChanges();
            }
            catch (ErrorJsonResultException ex)
            {
                if (ex.JsonResult.errcode == ReturnCode.客服帐号已存在kf_account_exsited)
                {
                    ViewBag.Messages = new List<MessageInfo>
                    {
                        new MessageInfo
                        {
                            Message = "客服账号已存在！",
                            MessageType = MessageTypes.Danger
                        }
                    };
                    return View(model);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddCustomViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var result = CustomServiceApi.AddCustom(
                    WeChatConfigManager.Current.GetAccessToken(),
                    string.Format("{0}@{1}", model.Account, WeChatConfigManager.Current.GetConfig().WeiXinAccount),
                    model.NickName,
                    model.Password);
                if (result.errcode != ReturnCode.请求成功)
                {
                    ViewBag.Messages = new List<MessageInfo>
                    {
                        new MessageInfo
                        {
                            Message = result.errmsg,
                            MessageType = MessageTypes.Danger
                        }
                    };
                    return View(model);
                }
                var kf = new WeiChat_KFCInfo
                {
                    Account = string.Format("{0}@{1}", model.Account, WeChatConfigManager.Current.GetConfig().WeiXinAccount),
                    HeadImgUrl = "",
                    NickName = model.NickName
                };
                db.WeiChat_KFCInfos.Add(kf);
                db.SaveChanges();
            }
            catch (ErrorJsonResultException ex)
            {
                if (ex.JsonResult.errcode == ReturnCode.客服帐号已存在kf_account_exsited)
                {
                    ViewBag.Messages = new List<MessageInfo>
                    {
                        new MessageInfo
                        {
                            Message = "客服账号已存在！",
                            MessageType = MessageTypes.Danger
                        }
                    };
                    return View(model);
                }
                if (ex.JsonResult.errcode == ReturnCode.客服帐号个数超过限制)
                {
                    ViewBag.Messages = new List<MessageInfo>
                    {
                        new MessageInfo
                        {
                            Message = "客服帐号个数超过限制！",
                            MessageType = MessageTypes.Danger
                        }
                    };
                    return View(model);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Remove(RemoveCustomViewModel model)
        {
            var message = new MessageInfo
            {
                Message = "操作成功！",
                MessageType = MessageTypes.Success
            };
            if (!ModelState.IsValid)
            {
                message.Message = "客服账号输入有误！";
                message.MessageType = MessageTypes.Danger;
                return Json(message);
            }
            try
            {
                //TODO:切换SDK
                var result = CustomServiceApi.DeleteCustom(
                    WeChatConfigManager.Current.GetAccessToken(),
                    model.Account);

                if (result.errcode != ReturnCode.请求成功)
                {
                    message.Message = result.errmsg;
                    message.MessageType = MessageTypes.Danger;
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