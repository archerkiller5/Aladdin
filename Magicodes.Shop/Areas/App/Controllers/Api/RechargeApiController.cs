// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : RechargeApiController.cs
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
using System.Web.Http;
using Magicodes.Logger;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Pays;
using Magicodes.WeChat.SDK.Pays.TenPayV3;
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Infrastructure;
using Newtonsoft.Json;
using Magicodes.WeiChat.Domain.Log;
using System.Web;
using Magicodes.Shop.Helpers;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/Recharge")]
    public class RechargeApiController : TenantBaseApiController<Log_Recharge>
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;

        [HttpPost]
        [Route("PayMoney")]
        public IHttpActionResult PayMoney([FromBody] Log_Recharge model)
        {
            log.Log(LoggerLevels.Debug, "进入API[PayMoney]");
            if (model.Amount <= 0)
                return BadRequest("充值金额必须大于0!");

            #region 统一下单

            var paymodel = new UnifiedorderRequest
            {
                OpenId = WeiChatApplicationContext.Current.WeiChatUser.OpenId,
                SpbillCreateIp = "8.8.8.8",
                OutTradeNo = PayUtil.GenerateOutTradeNo(),
                TotalFee = Convert.ToInt32(model.Amount * 100).ToString(),
                NonceStr = PayUtil.GetNoncestr(),
                TradeType = "JSAPI",
                Body = "购买商品",
                DeviceInfo = "WEB"
            };
            var result = WeChatApisContext.Current.TenPayV3Api.Unifiedorder(paymodel);

            var _dict = new Dictionary<string, string>
            {
                {"appId", result.AppId},
                {"timeStamp", PayUtil.GetTimestamp()},
                {"nonceStr", result.NonceStr},
                {"package", string.Format("prepay_id={0}", result.PrepayId)},
                {"signType", "MD5"}
            };
            _dict.Add("paySign", PayUtil.CreateMd5Sign(_dict, WeChatConfigManager.Current.GetPayConfig().TenPayKey));

            #endregion

            return Ok(_dict);
        }

        [HttpPost]
        [Route("DoRecharge")]
        public IHttpActionResult DoRecharge(dynamic model)
        {
            decimal amount = model.Amount; //Convert.ToDecimal(model.Amout);
            log.Log(LoggerLevels.Debug,
                "进入API[DoRecharge]amount:" + amount + ", model:" + JsonConvert.SerializeObject(model));

            if (amount <= 0)
                return BadRequest("充值金额必须大于0!");
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context

            //记录充值日志
            using (var payLogDo = new PayLogDO(TenantId, WeiChatApplicationContext.Current.WeiChatUser.OpenId, context != null ? context.GetClientIpAddress() : null, db))
            {
                payLogDo.AddRechargeLog(amount, paymentInterfaceLog: null);
            }
            return Ok();
        }
    }
}