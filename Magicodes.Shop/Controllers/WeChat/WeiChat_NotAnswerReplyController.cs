// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_NotAnswerReplyController.cs
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
using Magicodes.WeiChat.Data.Models.WeiChat;

namespace Magicodes.Shop.Controllers.WeChat
{
    public class WeiChat_NotAnswerReplyController : TenantBaseController<WeiChat_NotAnswerReply>
    {
        public ActionResult Index()
        {
            var model = db.WeiChat_NotAnswerReplies.FirstOrDefault(p => p.TenantId == TenantId) ??
                        new WeiChat_NotAnswerReply
                        {
                            TenantId = TenantId
                        };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(WeiChat_NotAnswerReply model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var subscribe = db.WeiChat_NotAnswerReplies.Find(model.Id);
            var isAdd = subscribe == null;
            Guid? oldContentId = null;
            if (!isAdd)
                oldContentId = subscribe.ContentId;
            if ((model.ContentId == null) && (model.KeyWordContentType != KeyWordContentTypes.CustomerService))
            {
                ModelState.AddModelError("", "请选择或者填写内容！");
                return View(model);
            }
            switch (model.KeyWordContentType)
            {
                case KeyWordContentTypes.Text:
                    break;
                case KeyWordContentTypes.Image:
                {
                    var mediaId = Request.Form["MediaId"];
                    if (string.IsNullOrEmpty(mediaId))
                    {
                        ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                        return View(model);
                    }
                    if (!isAdd)
                    {
                        var removeItem = db.WeiChat_KeyWordImageContents.Find(oldContentId);
                        if (removeItem != null)
                            db.WeiChat_KeyWordImageContents.Remove(removeItem);
                    }
                    var image = new WeiChat_KeyWordImageContent
                    {
                        ImageMediaId = mediaId
                    };
                    SetModelWithChangeStates(image, default(Guid));
                    model.ContentId = image.Id;
                }
                    break;
                case KeyWordContentTypes.Voice:
                {
                    var mediaId = Request.Form["mediaId"];
                    if (string.IsNullOrEmpty(mediaId))
                    {
                        ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                        return View(model);
                    }
                    if (!isAdd)
                    {
                        var removeItem = db.WeiChat_KeyWordVoiceContents.Find(oldContentId);
                        if (removeItem != null)
                            db.WeiChat_KeyWordVoiceContents.Remove(removeItem);
                    }
                    var voice = new WeiChat_KeyWordVoiceContent
                    {
                        VoiceMediaId = mediaId
                    };
                    SetModelWithChangeStates(voice, default(Guid));
                    model.ContentId = voice.Id;
                    break;
                }
                case KeyWordContentTypes.Video:
                {
                    var mediaId = Request.Form["mediaId"];
                    if (string.IsNullOrEmpty(mediaId))
                    {
                        ModelState.AddModelError("", "您没有选择资源，请选择资源！");
                        return View(model);
                    }
                    if (!isAdd)
                    {
                        var removeItem = db.WeiChat_KeyWordVideoContents.Find(oldContentId);
                        if (removeItem != null)
                            db.WeiChat_KeyWordVideoContents.Remove(removeItem);
                    }
                    var video = new WeiChat_KeyWordVideoContent
                    {
                        MediaId = mediaId
                    };
                    SetModelWithChangeStates(video, default(Guid));
                    model.ContentId = video.Id;
                    break;
                }
                case KeyWordContentTypes.News:
                    break;
                default:
                    break;
            }
            if (subscribe != null)
            {
                subscribe.ContentId = model.ContentId;
                subscribe.KeyWordContentType = model.KeyWordContentType;
                subscribe.UpdateBy = UserId;
                subscribe.UpdateTime = DateTime.Now;
                db.SaveChanges();
            }
            else
            {
                SetModelWithSaveChanges(model, isAdd ? default(Guid) : model.Id);
            }
            return View(model);
        }
    }
}