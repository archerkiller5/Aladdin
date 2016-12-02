// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChatController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Magicodes.Logger;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Pays.TenPayV3;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Infrastructure;
using Newtonsoft.Json;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Magicodes.WeiChat.Domain.Log;
using Magicodes.WeiChat.Data.Models.Product;

namespace Magicodes.Shop.Controllers
{
    [RoutePrefix("WeiChat")]
    public class WeiChatController : BaseController
    {
        private readonly LoggerBase logger = Loggers.Current.DefaultLogger; //LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     填写服务器配置时必须，为了安全，请生成自己的Token。注意：正式公众号的Token只允许英文或数字的组合，长度为3-32字符
        /// </summary>
        [Route("{tenantId}")]
        [AllowAnonymous]
        public ActionResult Index(int tenantId, string signature, string timestamp, string nonce, string echostr)
        {
            var token = WeChatConfigManager.Current.GetConfig(tenantId).Token;
            //get method - 仅在微信后台填写URL验证时触发
            if (CheckSignature.Check(signature, timestamp, nonce, token))
                return Content(echostr); //返回随机字符串则表示验证通过
            return Content("failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, token) + "。" +
                           "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
        }

        /// <summary>
        ///     微信支付回调地址
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [Route("PayNotify/{tenantId}")]
        [AllowAnonymous]
        public ActionResult PayNotify(int tenantId)
        {
            Action<NotifyResult> successAction = result =>
            {
                var resultLog = JsonConvert.SerializeObject(result);
                using (var context = new AppDbContext())
                {
                    var order = context.Order_Infos.FirstOrDefault(o => o.Code == result.OutTradeNo);
                    if (null != order)
                    {
                        //修改订单状态
                        order.State = EnumOrderStatus.Overhang;
                        order.ThirdPayType = EnumThirdPayType.WX;
                        order.PaymentOn = DateTime.Now;
                        order.UpdateTime = DateTime.Now;
                        //需要更新订单对应的商品的已成交数
                        var lst_details = db.Order_Details.Where(p => p.OrderID == order.Id).ToList();
                        foreach (Order_Detail detail in lst_details)
                        {
                            Product_Info product = db.Product_Infos.FirstOrDefault(
                                p => p.Id == detail.ProductID);
                            product.SellCount = product.SellCount + detail.Quantity;                            
                        }
                        context.SaveChanges();
                        //记录支付日志
                        using (var payLogDo = new PayLogDO(order.TenantId, order.OpenId, order.ClientIpAddress, context))
                        {
                            payLogDo.AddOrderLog(order.Id, order.TotalPrice, paymentInterfaceLog: resultLog);
                        }
                    }
                    else
                    {
                        logger.Log(LoggerLevels.Error, "Order information does not exist！OrderId：" + result.OutTradeNo);
                    }
                }
                //此处编写成功处理逻辑
                logger.Log(LoggerLevels.Debug, "Success: JSON:" + resultLog);
            };
            Action<NotifyResult> failAction = result =>
            {
                //此处编写失败处理逻辑
                var failLog = JsonConvert.SerializeObject(result);
                logger.Log(LoggerLevels.Error, "Fail: JSON:" + failLog);
                using (var context = new AppDbContext())
                {
                    var order = context.Order_Infos.FirstOrDefault(o => o.Code == result.OutTradeNo);
                    //记录支付日志
                    using (var payLogDo = new PayLogDO(order.TenantId, order.OpenId, order.ClientIpAddress, context))
                    {
                        payLogDo.AddOrderLog(order.Id, order.TotalPrice, paymentInterfaceLog: failLog, errorLog: failLog);
                    }
                }
            };
            return
                Content(WeChatApisContext.Current.TenPayV3Api.NotifyAndReurnResult(Request.InputStream, successAction,
                    failAction));
        }

        [HttpPost]
        [Route("{tenantId}")]
        [AllowAnonymous]
        public ActionResult Create(int tenantId, string signature, string timestamp, string nonce, string echostr)
        {
            try
            {
                var token = WeChatConfigManager.Current.GetConfig(tenantId).Token;
                //post method - 当有用户想公众账号发送消息时触发
                if (!CheckSignature.Check(signature, timestamp, nonce, token))
                {
                    logger.Log(LoggerLevels.Error, "服务器事件转发错误，请检查Token是否正确！");
                    return Content("参数错误，请检查Token是否正确！");
                }
                //设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
                var maxRecordCount = 10;

                //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
                var messageHandler = new MessageHandler(Request.InputStream, tenantId, maxRecordCount);

#if DEBUG
                //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                messageHandler.RequestDocument.Save(
                    Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Request_" +
                                   messageHandler.RequestMessage.FromUserName + ".txt"));
#endif

                try
                {
                    //执行微信处理过程
                    messageHandler.Execute();
                }
                catch (Exception ex)
                {
                    logger.Log(LoggerLevels.Error, ex.Message);
                    if (ex.InnerException != null) logger.Log(LoggerLevels.Error, ex.InnerException.Message);
                    if (messageHandler.ResponseDocument != null)
                        logger.Log(LoggerLevels.Error, messageHandler.ResponseDocument.ToString());
                }
#if DEBUG
                //测试时可开启，帮助跟踪数据
                messageHandler.ResponseDocument.Save(
                    Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Response_" +
                                   messageHandler.ResponseMessage.ToUserName + ".txt"));

#endif

                if (messageHandler.ResponseDocument == null)
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                return Content(messageHandler.ResponseDocument.ToString());
            }
            catch (Exception ex)
            {
                logger.Log(LoggerLevels.Error, ex.Message);
                if (ex.InnerException != null) logger.Log(LoggerLevels.Error, ex.InnerException.Message);
                return Content(ex.Message);
            }
        }

        /// <summary>
        ///     获取验证信息
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OAuth")]
        [AllowAnonymous]
        public ActionResult OAuth(string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
                return Content("请传递参数redirectUrl");
#if DEBUG
            //var logger = LogManager.GetCurrentClassLogger();
            //logger.Debug("OAuth redirectUrl：" + redirectUrl);
#endif
            //使用state存放目标url地址与参数
            //var url = OAuthApi.GetAuthorizeUrl(WeixinHelper.appId, ConfigHelper.Domain + "/WeiChat/OAuth/Code", redirectUrl, OAuthScope.snsapi_userinfo);
            //TODO:替换为SDK
            var url = OAuthApi.GetAuthorizeUrl(WeChatConfigManager.Current.GetConfig().AppId, redirectUrl, "magicodes.weichat",
                OAuthScope.snsapi_userinfo);
#if DEBUG
            //logger.Debug("OAuth url：" + url);
#endif
            return Redirect(url);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("OAuth/Code")]
        public ActionResult OAuthCode(string code, string state)
        {
#if DEBUG
            //var logger = LogManager.GetCurrentClassLogger();
            //logger.Debug("OAuth/Code：\n\r\tcode=" + code + "\n\r\tstate:" + state);
#endif

            if (string.IsNullOrEmpty(code))
                return Content("您拒绝了授权！");
            //通过，用code换取access_token
            //TODO:替换为SDK
            var result = OAuthApi.GetAccessToken(WeChatConfigManager.Current.GetConfig().AppId,
                WeChatConfigManager.Current.GetConfig().AppSecret, code);
            if (result.errcode != ReturnCode.请求成功)
                return Content("错误：" + result.errmsg);
            var url = Server.UrlDecode(state);
            if (string.IsNullOrWhiteSpace(url))
            {
                return Content("验证失败！请从正规途径进入！");
            }
            //已关注，可以得到详细信息
            //userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
            url += url.EndsWith("&") ? string.Empty : "&";
            url += "access_token=" + result.access_token + "&openid=" + result.openid;
            return Redirect(url);
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
            //Session["OAuthAccessTokenStartTime"] = DateTime.Now;
            //Session["OAuthAccessToken"] = result;
        }
    }
}