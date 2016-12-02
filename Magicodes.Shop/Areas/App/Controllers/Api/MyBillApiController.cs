// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MyBillApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Magicodes.Logger;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Infrastructure;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/MyBillApi")]
    public class MyBillApiController : TenantBaseApiController<Log_Recharge>
    {
        private LoggerBase log = Loggers.Current.DefaultLogger;

        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get(int tenantId, int orderstate = 0)
        {
            var openid = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
            //构造viewmodel给前台显示
            var listObject = new List<object>();
            //获取充值记录
            var lst_recharge = db.Log_Recharges.Where(p => p.OpenId == openid).ToList();
            foreach (var recharge in lst_recharge)
            {
                var re = new
                {
                    Money = recharge.Amount,
                    CreateTime = recharge.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Business = "充值",
                    Operation = "加"
                };
                listObject.Add(re);
            }
            //获取提现记录
            var Lst_withdraw = db.Log_Withdraws.Where(p => p.OpenId == openid).ToList();
            foreach (var withdraw in Lst_withdraw)
            {
                var wd = new
                {
                    Money = withdraw.WithdrawAmount,
                    CreateTime = withdraw.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Business = "提现",
                    Operation = "减"
                };
                listObject.Add(wd);
            }
            //订单记录
            var Lst_order = db.Order_Infos.Where(p => (p.OpenId == openid) &&
                                                      (p.State != EnumOrderStatus.Closed) &&
                                                      (p.State != EnumOrderStatus.Obligation) &&
                                                      (p.State != EnumOrderStatus.ReturnedGoods) &&
                                                      (p.State != EnumOrderStatus.UnpaidDelete)).ToList();
            foreach (var order in Lst_order)
            {
                var od = new
                {
                    Money = order.TotalPrice + order.Shipping,
                    CreateTime = order.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Business = "消费",
                    Operation = "减"
                };
                listObject.Add(od);
            }
            return Ok(listObject);
        }
    }
}