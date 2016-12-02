// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat.MessageHandler.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Magicodes.Shop.Helpers;
using Magicodes.WeChat.SDK.Apis.User;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.User;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.Tenant;
using NLog;
using Senparc.Weixin.Context;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Magicodes.Logger.NLog;
using Magicodes.Logger;

namespace Magicodes.Shop
{
    /// <summary>
    ///     微信消息与事件处理
    /// </summary>
    public class MessageHandler : MessageHandler<MessageContext<IRequestMessageBase, IResponseMessageBase>>
    {
        private readonly Lazy<AppDbContext> _db;

        /// <summary>
        ///     日志记录
        /// </summary>
        private readonly LoggerBase logger = new NLogLogger("WeChat.MessageHandler");

        protected AppDbContext db
        {
            get { return _db.Value; }
        }

        /// <summary>
        ///     租户Id
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        ///     所有没有被处理的消息会默认返回这里的结果
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            //所有没有被处理的消息会默认返回这里的结果
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您好，客服人员在忙，请稍后。";
            return responseMessage;
        }


        /// <summary>
        ///     订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            IResponseMessageBase responseMessage = null;
            {
                #region 获取并更新新关注用户的信息

                try
                {
                    //由于这里是基于微信服务器事件，故不能使用WeChatApisContext.Current.UserApi方式获取到UserApi接口，请new一个然后通过SetKey进行赋值
                    var userApi = new UserApi();
                    userApi.SetKey(TenantId);
                    //获取新关注用户的用户信息         
                    var userInfoResult = userApi.Get(requestMessage.FromUserName);
                    if (userInfoResult.IsSuccess())
                    {
                        var user = db.WeiChat_Users.FirstOrDefault(p => p.OpenId == userInfoResult.OpenId);
                        var userinfo = db.User_Infos.FirstOrDefault(p => p.OpenId == userInfoResult.OpenId);
                        if (userinfo == null)
                        {
                            userinfo = new User_Info
                            {
                                OpenId = userInfoResult.OpenId,
                                Balance = 0,
                                Integral = 0,
                                State = EnumUserState.Normal,
                                TenantId = TenantId,
                                CreateTime = DateTime.Now,
                                LastLoginOn = DateTime.Now,
                                LoginCount = 0
                            };
                            db.User_Infos.Add(userinfo);
                        }

                        if (user == null)
                        {
                            user = new WeiChat_User
                            {
                                City = userInfoResult.City,
                                Country = userInfoResult.Country,
                                GroupId = userInfoResult.GroupId,
                                HeadImgUrl = userInfoResult.Headimgurl,
                                Language = userInfoResult.Language,
                                NickName = userInfoResult.NickName,
                                OpenId = userInfoResult.OpenId,
                                Province = userInfoResult.Province,
                                Remark = userInfoResult.Remark,
                                Sex = userInfoResult.Sex,
                                Subscribe = true,
                                SubscribeTime = userInfoResult.SubscribeTime,
                                UnionId = userInfoResult.Unionid,
                                TenantId = TenantId
                            };
                            db.WeiChat_Users.Add(user);
                        }
                        else
                        {
                            user.City = userInfoResult.City;
                            user.Country = userInfoResult.Country;
                            user.GroupId = userInfoResult.GroupId;
                            user.HeadImgUrl = userInfoResult.Headimgurl;
                            user.Language = userInfoResult.Language;
                            user.NickName = userInfoResult.NickName;
                            //user.OpenId=userInfoResult.OpenId
                            user.Province = userInfoResult.Province;
                            user.Remark = userInfoResult.Remark;
                            user.Sex = userInfoResult.Sex;
                            user.Subscribe = true;
                            user.SubscribeTime = userInfoResult.SubscribeTime;
                            user.TenantId = TenantId;
                            user.UnionId = userInfoResult.Unionid;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        logger.Log(LoggerLevels.Error, userInfoResult.GetFriendlyMessage() + "\n\r详细错误：" + userInfoResult.DetailResult);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(LoggerLevels.Error, "MessageId:" + requestMessage.MsgId + Environment.NewLine + "具体错误信息：" + ex);
                }

                #endregion

                var keyword = string.Format("{0}[系统关注事件]", requestMessage.EventKey);

                //回复日志
                var replylog = new WeiChat_KeyWordReplyLog
                {
                    ReceiveWords = keyword,
                    CreateTime = DateTime.Now,
                    TenantId = TenantId,
                    From = requestMessage.FromUserName,
                    To = requestMessage.ToUserName,
                    EventKey = requestMessage.EventKey,
                    MsgId = requestMessage.MsgId
                };
                try
                {
                    var subscribeReplies = db.WeiChat_SubscribeReplies.FirstOrDefault();
                    if (subscribeReplies != null)
                    {
                        replylog.ContentId = subscribeReplies.ContentId;
                        replylog.KeyWord = "";
                        replylog.WeiChat_KeyWordAutoReplyId = subscribeReplies.Id;

                        responseMessage = this.ReplyResponseMessage(db, keyword, subscribeReplies.ContentId,
                            subscribeReplies.KeyWordContentType,
                            (key, contentId, type) => { replylog.IsSuccess = true; });
                    }
                }
                catch (Exception ex)
                {
                    replylog.Error = ex.ToString();
                    responseMessage = CreateResponseMessage<ResponseMessageText>();
                    ((ResponseMessageText)responseMessage).Content = HttpContext.Current.IsDebuggingEnabled
                        ? ex.ToString()
                        : "出现错误，无法处理请求，具体信息请查看回复日志！";
                }
                //记录日志
                db.WeiChat_KeyWordReplyLogs.Add(replylog);
                db.SaveChanges();
            }
            if (responseMessage == null)
            {
                responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
                ((ResponseMessageText)responseMessage).Content = "欢迎您关注Magicodes.WeiChat！";
                return responseMessage;
            }
            return responseMessage;
        }

        /// <summary>
        ///     退订
        ///     实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        ///     unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            //更新粉丝信息
            using (var db = new AppDbContext())
            {
                var user = db.WeiChat_Users.FirstOrDefault(p => p.OpenId == requestMessage.FromUserName);
                if (user != null)
                {
                    user.Subscribe = false;
                    db.SaveChanges();
                }
                //TODO:增加取消关注的日志记录
            }
            return null;
        }

        /// <summary>
        ///     扫描带参数二维码事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            //通过扫描关注
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "Key未处理：" + requestMessage.EventKey;
            if (!string.IsNullOrEmpty(requestMessage.EventKey))
            {
                var qrCode = db.WeiChat_QRCodes.FirstOrDefault(p => p.ParamsValue == requestMessage.EventKey);
                if (qrCode != null)
                    switch (qrCode.UserFor)
                    {
                        case QRCodeUseForTypes.BindManager:
                            {
                                var user = db.Users.Find(requestMessage.EventKey);
                                user.OpenId = requestMessage.FromUserName;
                                db.SaveChanges();
                                responseMessage.Content = "已成功绑定微信管理员！";
                            }
                            break;
                    }
            }
            return responseMessage;
        }

        /// <summary>
        ///     上报地理位置事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            //回复日志
            var log = new WeiChat_LocationEventLog
            {
                CreateTime = DateTime.Now,
                TenantId = TenantId,
                From = requestMessage.FromUserName,
                To = requestMessage.ToUserName,
                Latitude = requestMessage.Latitude,
                Longitude = requestMessage.Longitude,
                Precision = requestMessage.Precision
            };
            db.WeiChat_LocationEventLogs.Add(log);
            db.SaveChanges();
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            //requestMessage.Latitude
            //requestMessage.Longitude
            responseMessage.Content = "您的位置已被记录（PS：如果您不需要告知用户，请返回NULL，以免打扰用户）。";
            return responseMessage; //这里也可以返回null（需要注意写日志时候null的问题）
        }

        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
            return responseMessage;
        }

        /// <summary>
        ///     事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(
            RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件";
            return responseMessage;
        }

        /// <summary>
        ///     事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(
            RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件且弹出“消息接收中”提示框";
            return responseMessage;
        }

        /// <summary>
        ///     事件之弹出系统拍照发图(pic_sysphoto)
        ///     实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出系统拍照发图";
            return responseMessage;
        }

        /// <summary>
        ///     事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(
            RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出拍照或者相册发图";
            return responseMessage;
        }

        /// <summary>
        ///     事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出微信相册发图器";
            return responseMessage;
        }

        /// <summary>
        ///     事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationSelectRequest(
            RequestMessageEvent_Location_Select requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出地理位置选择器";
            return responseMessage;
        }

        #region 基础内容

        public MessageHandler(Stream inputStream, int tenantId, int maxRecordCount = 0)
            : base(inputStream, null, maxRecordCount)
        {
            WeixinContext.ExpireMinutes = 3;
            TenantId = tenantId;

            _db = new Lazy<AppDbContext>(() =>
            {
                var _tmpDb = new AppDbContext();
                //启用租户筛选器
                TenantManager.Current.EnableTenantFilter(_tmpDb, TenantId);
                return _tmpDb;
            });
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
                CurrentMessageContext.StorageData = 0;
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = (int)CurrentMessageContext.StorageData + 1;
        }

        #endregion

        #region 菜单事件

        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            // 预处理文字或事件类型请求。
            // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，
            // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：
            // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest
            // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage
            // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey
            return null; //返回null，则继续执行OnTextRequest或OnEventRequest
        }

        /// <summary>
        ///     菜单点击事件处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase responseMessage = null;
            //回复日志
            var replylog = new WeiChat_KeyWordReplyLog
            {
                ReceiveWords = requestMessage.EventKey,
                CreateTime = DateTime.Now,
                TenantId = TenantId,
                From = requestMessage.FromUserName,
                To = requestMessage.ToUserName,
                EventKey = requestMessage.EventKey,
                MsgId = requestMessage.MsgId
            };
            try
            {
                #region 事件关键字回复，仅支持等于

                var keyword =
                    db.WeiChat_KeyWordAutoReplies.FirstOrDefault(
                        p => p.AllowEventKey && (p.KeyWord == requestMessage.EventKey));
                if (keyword != null)
                {
                    replylog.ContentId = keyword.ContentId;
                    replylog.KeyWord = keyword.KeyWord;
                    replylog.WeiChat_KeyWordAutoReplyId = keyword.Id;
                    responseMessage = this.ReplyResponseMessage(db, keyword.KeyWord, keyword.ContentId,
                        keyword.KeyWordContentType, (key, contentId, type) => { replylog.IsSuccess = true; });
                }
                #endregion

                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                replylog.Error = ex.ToString();
                responseMessage = CreateResponseMessage<ResponseMessageText>();
                ((ResponseMessageText)responseMessage).Content = HttpContext.Current.IsDebuggingEnabled
                    ? ex.ToString()
                    : "出现错误，无法处理请求！";
            }
            //记录日志
            db.WeiChat_KeyWordReplyLogs.Add(replylog);
            db.SaveChanges();

            return responseMessage;
        }

        #endregion

        #region 消息（文本、图片、视频、音频）处理

        /// <summary>
        ///     处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            IResponseMessageBase responseMessage = null;
            //客服用于唤醒多客服
            if (requestMessage.Content == "客服")
            {
                responseMessage = CreateResponseMessage<ResponseMessageTransfer_Customer_Service>();
                return responseMessage;
            }
            //回复日志
            var replylog = new WeiChat_KeyWordReplyLog
            {
                ReceiveWords = requestMessage.Content,
                CreateTime = DateTime.Now,
                TenantId = TenantId,
                From = requestMessage.FromUserName,
                To = requestMessage.ToUserName,
                //EventKey = requestMessage.EventKey,
                MsgId = requestMessage.MsgId
            };

            {
                try
                {
                    #region 关键字回复

                    var keyword =
                        db.WeiChat_KeyWordAutoReplies.FirstOrDefault(
                            p => (p.MatchType == KeyWordMatchTypes.Equals) && (p.KeyWord == requestMessage.Content));
                    if (keyword == null)
                        keyword =
                            db.WeiChat_KeyWordAutoReplies.FirstOrDefault(
                                p =>
                                    (p.MatchType == KeyWordMatchTypes.Contains) &&
                                    requestMessage.Content.Contains(p.KeyWord));
                    if (keyword != null)
                    {
                        replylog.ContentId = keyword.ContentId;
                        replylog.KeyWord = keyword.KeyWord;
                        replylog.WeiChat_KeyWordAutoReplyId = keyword.Id;

                        responseMessage = this.ReplyResponseMessage(db, keyword.KeyWord, keyword.ContentId,
                            keyword.KeyWordContentType, (key, contentId, type) => { replylog.IsSuccess = true; });
                    }
                    #endregion

                    else
                    {
                        #region 答不上来配置

                        //答不上来
                        try
                        {
                            var notAnswerReply = db.WeiChat_NotAnswerReplies.FirstOrDefault();
                            if (notAnswerReply != null)
                            {
                                replylog.ContentId = notAnswerReply.ContentId;
                                replylog.KeyWord = "";
                                replylog.WeiChat_KeyWordAutoReplyId = notAnswerReply.Id;

                                responseMessage = this.ReplyResponseMessage(db, requestMessage.Content,
                                    notAnswerReply.ContentId, notAnswerReply.KeyWordContentType,
                                    (key, contentId, type) => { replylog.IsSuccess = true; });
                            }
                            else
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine("您好，该关键字本公众号尚不支持/(ㄒoㄒ)/~~。");
                                var keywords = db.WeiChat_KeyWordAutoReplies.Take(10).Select(p => p.KeyWord).ToArray();
                                if ((keywords != null) && (keywords.Length > 0))
                                {
                                    sb.AppendLine("不过，您可以试试以下关键字哦：");
                                    sb.AppendLine(string.Join("、", keywords));
                                }
                                responseMessage = CreateResponseMessage<ResponseMessageText>();
                                ((ResponseMessageText)responseMessage).Content = sb.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            replylog.Error = ex.ToString();
                            responseMessage = CreateResponseMessage<ResponseMessageText>();
                            ((ResponseMessageText)responseMessage).Content = HttpContext.Current.IsDebuggingEnabled
                                ? ex.ToString()
                                : "出现错误，无法处理请求！";
                        }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    replylog.Error = ex.ToString();
                    responseMessage = CreateResponseMessage<ResponseMessageText>();
                    ((ResponseMessageText)responseMessage).Content = HttpContext.Current.IsDebuggingEnabled
                        ? ex.ToString()
                        : "出现错误，无法处理请求！";
                }
                //记录日志
                db.WeiChat_KeyWordReplyLogs.Add(replylog);
                db.SaveChanges();
            }
            return responseMessage;
        }


        ///// <summary>
        ///// 处理图片请求
        ///// </summary>
        ///// <param name="requestMessage"></param>
        ///// <returns></returns>
        //public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        //{
        //    var responseMessage = CreateResponseMessage<ResponseMessageNews>();
        //    responseMessage.Articles.Add(new Article()
        //    {
        //        Title = "您刚才发送了图片信息",
        //        Description = "您发送的图片将会显示在边上",
        //        PicUrl = requestMessage.PicUrl,
        //        Url = "http://www.hp.com"
        //    });
        //    responseMessage.Articles.Add(new Article()
        //    {
        //        Title = "第二条",
        //        Description = "第二条带连接的内容",
        //        PicUrl = requestMessage.PicUrl,
        //        Url = "http://www.hp.com"
        //    });
        //    return responseMessage;
        //}

        /// <summary>
        ///     处理语音请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            //var responseMessage = CreateResponseMessage<ResponseMessageVoice>();
            //responseMessage.Voice.MediaId = "HXIy1CJD5Qt12D9XBuSx0sDA8YS_82zS3zdaJsZUYOc";
            //return responseMessage;
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = string.Empty;
            return responseMessage;
        }

        /// <summary>
        ///     处理视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnVideoRequest(RequestMessageVideo requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您发送了一条视频信息，ID：" + requestMessage.MediaId;
            return responseMessage;
        }

        /// <summary>
        ///     处理小视频请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnShortVideoRequest(RequestMessageShortVideo requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您刚才发送的是小视频";
            return responseMessage;
        }

        /// 处理位置请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = string.Format("您刚才发送了地理位置信息。Location_X：{0}，Location_Y：{1}，Scale：{2}，标签：{3}",
                requestMessage.Location_X, requestMessage.Location_Y,
                requestMessage.Scale, requestMessage.Label);
            return responseMessage;
        }

        /// <summary>
        ///     处理链接消息请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnLinkRequest(RequestMessageLink requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = string.Format(@"您发送了一条连接信息：
                Title：{0} 
                Description:{1}
                Url:{2}", requestMessage.Title, requestMessage.Description, requestMessage.Url);
            return responseMessage;
        }

        #endregion

        /// <summary>
        /// 新创建的门店审核通过事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_Poi_Check_NotifyRequest(RequestMessageEvent_Poi_Check_Notify requestMessage)
        {
            var sid = requestMessage.UniqId;
            var poi = db.Store_Info.FirstOrDefault(p => p.SID == sid);
            if (poi == null)
            {
                logger.Log(LoggerLevels.Error, "POI审核通过，但找不到SID为（" + sid + ")的POI。");
            }
            else
            {
                //TODO:添加通知
                poi.PoiId = requestMessage.PoiId;
                poi.HasApproved = true;
                db.SaveChanges();
            }
            return base.OnEvent_Poi_Check_NotifyRequest(requestMessage);
        }
    }
}