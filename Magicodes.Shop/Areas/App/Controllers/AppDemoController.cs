// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppDemoController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Magicodes.Shop.Helpers;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Pays;
using Magicodes.WeChat.SDK.Pays.TenPayV3;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using Newtonsoft.Json;
using NLog;

namespace Magicodes.Shop.Areas.App.Controllers
{
    [RouteArea("App")]
    //注意继承自：AppBaseController
    public class AppDemoController : AppBaseController
    {
        //产品宣传
        public ActionResult ProductPromotion()
        {
            return View();
        }

        // GET: App/AppDemo/WeChatOAuthTest
        //博客说明：http://www.cnblogs.com/codelove/p/5355514.html
        [WeChatOAuth]
        public ActionResult WeChatOAuthTest()
        {
            return View(WeiChatUser);
        }

        public ActionResult RoadMap()
        {
            return View();
        }

        //个人中心
        [WeChatOAuth]
        public ActionResult PersonalCenter()
        {
            return View(WeiChatUser);
        }

        //绑定手机
        [WeChatOAuth]
        public ActionResult PhoneBound()
        {
            return View(WeiChatUser);
        }

        [WeChatOAuth]
        public ActionResult TestPay()
        {
            #region 统一下单

            LogManager.GetCurrentClassLogger().Debug("开始下单");
            var model = new UnifiedorderRequest { OpenId = WeiChatApplicationContext.Current.WeiChatUser.OpenId };
            LogManager.GetCurrentClassLogger().Debug(JsonConvert.SerializeObject(model.OpenId));
            model.SpbillCreateIp = "8.8.8.8";
            model.OutTradeNo = PayUtil.GenerateOutTradeNo();
            model.TotalFee = "1";
            model.NonceStr = PayUtil.GetNoncestr();
            model.TradeType = "JSAPI";
            model.Body = "购买商品";
            model.DeviceInfo = "WEB";
            var result = WeChatApisContext.Current.TenPayV3Api.Unifiedorder(model);

            var _dict = new Dictionary<string, string>
            {
                {"appId", result.AppId},
                {"timeStamp", PayUtil.GetTimestamp()},
                {"nonceStr", result.NonceStr},
                {"package", "prepay_id=" + result.PrepayId},
                {"signType", "MD5"}
            };
            _dict.Add("paySign", PayUtil.CreateMd5Sign(_dict, WeChatConfigManager.Current.GetPayConfig().TenPayKey));

            #endregion

            ViewBag.PayPam = JsonConvert.SerializeObject(_dict);
            return View();
        }

        public ActionResult ExceptionTest()
        {
            throw new Exception("异常测试！");
        }

        public ActionResult SuccessMessage()
        {
            return SuccessTip("温馨提示", "您的认证申请提交成功，管理员会在1~2个工作日内完成审批，请耐心等待！",
                okUrl: Url.TenantAction("WeChatOAuthTest", "AppDemo"));
        }

        public ActionResult WarnMessage()
        {
            return WarnTip("温馨提示", "您提交的资料存在问题，请确认！", okUrl: Url.TenantAction("WeChatOAuthTest", "AppDemo"));
        }

        /// <summary>
        ///     购物车
        /// </summary>
        /// <returns></returns>
        [WeChatOAuth]
        public ActionResult ShoppingCart()
        {
            return View();
        }

        #region 微相册

        //相册
        public ActionResult PhotoGallery()
        {
            return View();
        }

        //照片
        [Route("AppDemo/PhotoGallery/{typeId}/Photos", Name = "PhotoGallery_Photos_Route")]
        public ActionResult PhotoGallery_Photos(string typeId)
        {
            return View();
        }

        [Route("AppDemo/PhotoGallery/{typeId}/Upload", Name = "PhotoGallery_UploadPhotos_Route")]
        public ActionResult PhotoGallery_UploadPhotos(string typeId)
        {
            return View();
        }

        #endregion
    }
}