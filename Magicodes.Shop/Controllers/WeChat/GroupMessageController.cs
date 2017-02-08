// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : GroupMessageController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web.Mvc;
using Magicodes.WeChat.SDK.Apis.User;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Magicodes.Shop.Controllers.WeChat
{
    public class GroupMessageController : TenantBaseController<WeiChat_User>
    {
        public ActionResult Send(int? type)
        {
            ViewBag.UserGroupId = new SelectList(db.WeiChat_UserGroups.ToList(), "GroupId", "Name");
            var log = new WeiChat_GroupMessageLog();
            return View(log);
        }

        [HttpPost]
        public ActionResult Send(WeiChat_GroupMessageLog model)
        {
            var res = new AjaxResponse
            {
                Success = true,
                Message = "发送成功！"
            };
            if (string.IsNullOrWhiteSpace(model.MediaId))
            {
                res.Success = false;
                res.Message = "请选择素材！";
            }
            else
            {
                try
                {
                    var messageType = GroupMessageType.image;
                    switch (model.MessageType)
                    {
                        case SendMessageTypes.Text:
                            messageType = GroupMessageType.text;
                            break;
                        case SendMessageTypes.Image:
                            messageType = GroupMessageType.image;
                            break;
                        case SendMessageTypes.Voice:
                            messageType = GroupMessageType.voice;
                            break;
                        case SendMessageTypes.Video:
                            messageType = GroupMessageType.video;
                            break;
                        case SendMessageTypes.News:
                            messageType = GroupMessageType.mpnews;
                            break;
                        default:
                            break;
                    }
                    var q = db.WeiChat_Users.AsQueryable();
                    if (model.SexType != SexTypes.All)
                    {
                        var sexType = (WeChatSexTypes) ((int) model.SexType - 1);
                        q = q.Where(p => p.Sex == sexType);
                        if (!string.IsNullOrEmpty(model.UserGroupIds))
                        {
                            //var groupId = Convert.ToInt32(model.UserGroupIds);
                            //q = q.Where(p => p.GroupIds.Contains(groupId));
                            q = q.Where(p => p.GroupIds.Contains(model.UserGroupIds));
                        }
                    }
                    switch (model.MessageType)
                    {
                        case SendMessageTypes.Text:
                        case SendMessageTypes.Image:
                        case SendMessageTypes.Voice:
                        case SendMessageTypes.Video:
                        case SendMessageTypes.News:
                        {
                            SetModel(model, default(long));
                            if (model.SexType != SexTypes.All)
                            {
                                var openIds = q.Select(p => p.OpenId).ToArray();
                                if ((openIds == null) || (openIds.Length == 0))
                                {
                                    res.Success = false;
                                    res.Message = "没有找到符合该条件的粉丝，请手动同步粉丝后或者修改条件后再试！";
                                }
                                var result = GroupMessageApi.SendGroupMessageByOpenId(AccessToken, messageType,
                                    model.MediaId, 10000, openIds);
                                if (result.errcode != ReturnCode.请求成功)
                                {
                                    res.Success = false;
                                    res.Message = "发送失败！" + result.errmsg;
                                }
                            }
                            else
                            {
                                var result = GroupMessageApi.SendGroupMessageByGroupId(AccessToken, model.UserGroupIds,
                                    model.MediaId, messageType, true);
                                if (result.errcode != ReturnCode.请求成功)
                                {
                                    res.Success = false;
                                    res.Message = "发送失败！" + result.errmsg;
                                }
                            }
                            model.IsSuccess = res.Success;
                            model.Message = res.Message;
                            db.WeiChat_GroupMessageLogs.Add(model);
                            db.SaveChanges();
                            break;
                        }
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    res.Success = false;
                    res.Message = "发送失败！" + ex.Message;
                    model.IsSuccess = res.Success;
                    model.Message = res.Message;
                    db.WeiChat_GroupMessageLogs.Add(model);
                    db.SaveChanges();
                }
            }
            return Json(res);
        }

        //{

        //public ActionResult SendImages()
        //    ViewBag.GeoupId = new SelectList(db.WeiChat_UserGroups.ToList(), dataValueField: "GroupId", dataTextField: "Name");
        //    var log = new WeiChat_GroupMessageLog();
        //    return View(log);
        //}
        //[HttpPost]
        //public ActionResult SendImages(SendMessageViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = GroupMessageApi.SendGroupMessageByGroupId(WeChatConfigManager.Current.AccessToken, null, model.MediaId, GroupMessageType.image, true);
        //    if (result.errcode != ReturnCode.请求成功)
        //    {
        //        ViewBag.Messages = new List<MessageInfo>
        //        {
        //            new MessageInfo(){
        //                Message=result.errmsg,
        //                MessageType=MessageTypes.Danger
        //            }
        //        };

        //    }
        //    else
        //    {
        //        ViewBag.Messages = new List<MessageInfo>() {
        //            new MessageInfo(){
        //                Message="发送成功！",
        //                MessageType=MessageTypes.Success
        //            }
        //        };
        //    }
        //    return View(model);
        //}


        //public ActionResult SendVoices()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult SendVoices(SendMessageViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = GroupMessageApi.SendGroupMessageByGroupId(WeChatConfigManager.Current.AccessToken, null, model.MediaId, GroupMessageType.voice, true);
        //    if (result.errcode != ReturnCode.请求成功)
        //    {
        //        ViewBag.Messages = new List<MessageInfo>
        //        {
        //            new MessageInfo(){
        //                Message=result.errmsg,
        //                MessageType=MessageTypes.Danger
        //            }
        //        };

        //    }
        //    else
        //    {
        //        ViewBag.Messages = new List<MessageInfo>() {
        //            new MessageInfo(){
        //                Message="发送成功！",
        //                MessageType=MessageTypes.Success
        //            }
        //        };
        //    }
        //    return View(model);
        //}

        //public ActionResult SendNews()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult SendNews(SendMessageViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = GroupMessageApi.SendGroupMessageByGroupId(WeChatConfigManager.Current.AccessToken, null, model.MediaId, GroupMessageType.mpnews, true);
        //    if (result.errcode != ReturnCode.请求成功)
        //    {
        //        ViewBag.Messages = new List<MessageInfo>
        //        {
        //            new MessageInfo(){
        //                Message=result.errmsg,
        //                MessageType=MessageTypes.Danger
        //            }
        //        };

        //    }
        //    else
        //    {
        //        ViewBag.Messages = new List<MessageInfo>() {
        //            new MessageInfo(){
        //                Message="发送成功！",
        //                MessageType=MessageTypes.Success
        //            }
        //        };
        //    }
        //    return View(model);
        //}
    }
}