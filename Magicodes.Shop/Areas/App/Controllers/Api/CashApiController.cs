// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : CashApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Web.Http;
using Magicodes.Logger;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/Cash")]
    public class CashApiController : TenantBaseApiController<Log_Withdraw>
    {
        private readonly LoggerBase Log = Loggers.Current.DefaultLogger;

        /// <summary>
        ///     申请提现
        /// </summary>
        /// <param name="tx_balance"></param>
        /// <returns></returns>
        [Route("ApplicationWithdrawals")]
        [HttpPost]
        public IHttpActionResult ApplicationWithdrawals([FromBody] JObject jdata)
        {
            dynamic json = jdata;
            Log.Log(LoggerLevels.Debug, "jdata：" + json.tx_balance);
            decimal balance = json.tx_balance;
            if (balance <= 0)
                return BadRequest("提现金额必须大于0元！");
            //去数据库获取一边该用户余额
            var openId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
            var user = db.User_Infos.Find(openId); //.WeiChat_Users.Find(openId);
            if (balance > user.Balance)
                return BadRequest("您的可提现余额不足！");

            //写入提现申请记录
            Log.Log(LoggerLevels.Debug, "开始写入提现申请记录");
            var log = new Log_Withdraw();
            log.Status = WithdrawApprovalStatus.Pending;
            log.OpenId = openId;
            log.Phone = user.Mobile;
            log.Remark = user.TrueName + " 余额提现";
            log.TrueName = user.TrueName;
            log.WithdrawAmount = balance;
            log.CreateTime = DateTime.Now;
            log.TenantId = WeiChatApplicationContext.Current.WeiChatUser.TenantId;

            db.Log_Withdraws.Add(log);
            user.Balance -= balance;
            db.SaveChanges();

            return Ok(); //Json(res);
        }
    }
}