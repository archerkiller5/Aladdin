// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeChatMessageHelper.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Data.Entity;
using System.Linq;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;

namespace Magicodes.Shop.Helpers
{
    public static class WeChatMessageHelper
    {
        /// <summary>
        ///     根据关键字或关注时回复自动返回相应类型的消息
        /// </summary>
        /// <param name="messageHandler"></param>
        /// <param name="db"></param>
        /// <param name="keyWord"></param>
        /// <param name="contentId"></param>
        /// <param name="type"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public static IResponseMessageBase ReplyResponseMessage(
            this MessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>> messageHandler,
            AppDbContext db, string keyWord, Guid? contentId, KeyWordContentTypes type,
            Action<string, Guid?, KeyWordContentTypes> success)
        {
            IResponseMessageBase responseMessage = null;
            switch (type)
            {
                case KeyWordContentTypes.Text:
                {
                    var text = db.WeiChat_KeyWordTextContents.FirstOrDefault(p => p.Id == contentId);
                    if (text == null)
                        throw new Exception("文本消息未设置或设置不正确，请在后台重新设置！");
                    responseMessage = messageHandler.CreateResponseMessage<ResponseMessageText>();
                    ((ResponseMessageText) responseMessage).Content = text.Text;
                    success.Invoke(keyWord, contentId, type);
                }
                    break;
                case KeyWordContentTypes.News:
                {
                    var news =
                        db.WeiChat_KeyWordNewsContents.Include(p => p.Articles).FirstOrDefault(p => p.Id == contentId);
                    if (news == null)
                        throw new Exception("文本消息未设置或设置不正确，请在后台重新设置！");
                    responseMessage = messageHandler.CreateResponseMessage<ResponseMessageNews>();
                    var articles =
                        news.Articles.Select(
                            p =>
                                new Article
                                {
                                    Description = p.Description,
                                    PicUrl = p.PicUrl,
                                    Title = p.Title,
                                    Url = p.Url
                                }).ToList();
                    ((ResponseMessageNews) responseMessage).Articles = articles;
                    success.Invoke(keyWord, contentId, type);
                }
                    break;
                case KeyWordContentTypes.Image:
                {
                    var image = db.WeiChat_KeyWordImageContents.FirstOrDefault(p => p.Id == contentId);
                    if (image == null)
                        throw new Exception("文本消息未设置或设置不正确，请在后台重新设置！");
                    responseMessage = messageHandler.CreateResponseMessage<ResponseMessageImage>();
                    ((ResponseMessageImage) responseMessage).Image = new Image
                    {
                        MediaId = image.ImageMediaId
                    };
                    success.Invoke(keyWord, contentId, type);
                }
                    break;
                case KeyWordContentTypes.Video:
                {
                    var video = db.WeiChat_KeyWordVideoContents.FirstOrDefault(p => p.Id == contentId);
                    if (video == null)
                        throw new Exception("文本消息未设置或设置不正确，请在后台重新设置！");
                    responseMessage = messageHandler.CreateResponseMessage<ResponseMessageVideo>();
                    ((ResponseMessageVideo) responseMessage).Video = new Video
                    {
                        MediaId = video.MediaId,
                        //TODO：存储描述和标题
                        Description = "测试",
                        Title = "测试"
                    };
                    success.Invoke(keyWord, contentId, type);
                }
                    break;
                case KeyWordContentTypes.Voice:
                {
                    var voice = db.WeiChat_KeyWordVoiceContents.FirstOrDefault(p => p.Id == contentId);
                    if (voice == null)
                        throw new Exception("文本消息未设置或设置不正确，请在后台重新设置！");
                    responseMessage = messageHandler.CreateResponseMessage<ResponseMessageVoice>();
                    ((ResponseMessageVoice) responseMessage).Voice = new Voice
                    {
                        MediaId = voice.VoiceMediaId
                    };
                    success.Invoke(keyWord, contentId, type);
                }
                    break;
                case KeyWordContentTypes.CustomerService:
                {
                    responseMessage = messageHandler.CreateResponseMessage<ResponseMessageTransfer_Customer_Service>();
                    success.Invoke(keyWord, contentId, type);
                    break;
                }
                default:
                    break;
            }
            return responseMessage;
        }
    }
}