// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AuthenticationApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web.Http;
using Magicodes.Logger;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeiChat.Data.Models.User;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Magicodes.WeiChat.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Magicodes.WeiChat.Infrastructure.Services;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/Authentication")]
    public class AuthenticationApiController : TenantBaseApiController<User_Info>
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;

        [HttpPost]
        [Route("SendBindPhoneMsg")]
        public IHttpActionResult SendBindPhoneMsg([FromBody] string phoneNumber)
        {
            log.Log(LoggerLevels.Debug, "进入SendBindPhoneMsg方法");
            var res = new AjaxResponse
            {
                Message = "发送成功！",
                Success = true
            };
            try
            {
                var openid = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                var user = db.User_Infos.Find(openid);
                if (!string.IsNullOrWhiteSpace(user.Mobile))
                    return BadRequest("用户已完成了手机验证不需要反复验证!");
                //TODO:手机短信绑定
                string errMsg = "";
                var checknumber = RandomHelper.CreateNumRandomCode(6);
                ShopSmsService smsServer = new ShopSmsService();
                if (!smsServer.SendBoundPhoneMsg(phoneNumber, checknumber, out errMsg))
                {
                    throw new Exception(errMsg);
                }
                //var inparamter = new Inparameter_SendMsg();
                //inparamter.ChekOutNumber = checknumber;
                //inparamter.PhoneNumber = phoneNumber;
                //log.Log(LoggerLevels.Debug,
                //    "开始调用SendBindPhoneAsync,inparamter.ChekOutNumber:" + inparamter.ChekOutNumber +
                //    "inparamter.PhoneNumber:" + inparamter.PhoneNumber + "");
                ////ContainerManager.Current.Resolve<IShopSmsService>().SendBindPhoneAsync(inparamter);
                //var server = new AlidayuMsmService();
                //server.SendBindPhoneAsync(inparamter);
                //log.Log(LoggerLevels.Debug, "调用SendBindPhoneAsync完毕");
                
                //发送成功后需要将验证码保存到数据库用于确定时候的验证
                var model = new User_BindPhone
                {
                    CheckNumber = checknumber,
                    OpenId = openid,
                    PhoneNumber = phoneNumber,
                    TenantId = WeiChatApplicationContext.Current.WeiChatUser.TenantId,
                    CreateTime = DateTime.Now
                };
                log.Log(LoggerLevels.Debug, "User_BindPhone值:" + JsonConvert.SerializeObject(model));
                db.User_BindPhones.Add(model);
                db.SaveChanges();
                log.Log(LoggerLevels.Debug, "保存User_BindPhone成功");
            }
            catch (Exception ea)
            {
                res.Message = "发送失败！";
                res.Success = false;
                log.Log(LoggerLevels.Error, "发送用户手机[" + phoneNumber + "]绑定验证码失败!原因:" + ea.Message);
            }
            return Json(res);
        }

        [HttpPost]
        [Route("DoAuthentication")] //string[] inparamters
        public IHttpActionResult DoAuthentication([FromBody] JObject jdata)
            // string[] inparamters string yzm, string phone, 
        {
            //验证校验码是否与系统发出的校验码一致，不一致则返回消息给前台
            log.Log(LoggerLevels.Trace, "进入DoAuthentication方法");

            dynamic json = jdata;
            string yzm = json.yzm;
            string phone = json.phone;
            string name = json.name;


            string nickname =json.nickname;
            string idcard = json.idcard;
            string email =json.email;
            string workplace1 = json.workplace1;
            string businessscope1 = json.businessscope1;
            string workplace2 = json.workplace2;
            string businessscope2 = json.businessscope2;
            string workplace3 =json.workplace3;
            string businessscope3 = json.businessscope3;




            log.Log(LoggerLevels.Trace, "yzm:" + yzm + "phone:" + phone + "name:" + name);
            var res = new AjaxResponse
            {
                Message = "认证成功",
                Success = true
            };
            if (string.IsNullOrWhiteSpace(phone))
                return BadRequest("手机号不能为空!");
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("真实姓名不能为空!");
            //需要和数据库中的User_BindPhone记录进行比较,如果与数据创建时间相隔1分钟或一分钟以上则验证不通过

            var openid = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
            var user = db.User_Infos.Find(openid);

            ////屏蔽验证码环节
            //if (!string.IsNullOrWhiteSpace(user.Mobile))
            //    return BadRequest("用户已完成了手机验证不需要反复验证!");
            //var check_bind =
            //    db.User_BindPhones.Where(p => (p.OpenId == openid) && (p.CheckNumber == yzm)).FirstOrDefault();
            //if (check_bind == null) return BadRequest("没有找到验证信息，请重新获取验证码进行验证!");

            //var checkdate = long.Parse(check_bind.CreateTime.ToString("yyyyMMddHHmmss"));
            //var nowdate = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            //var timespan = nowdate - checkdate;

            //log.Log(LoggerLevels.Debug, "nowdate:" + nowdate + ", checkdate:" + checkdate + ",验证时间相隔秒数:" + timespan);
            //if (timespan >= 60)
            //    return BadRequest("验证时间超时，请重新获取验证码进行验证!");

            user.Mobile = phone;
            user.TrueName = name;
            user.NickName = nickname;
            user.IdCard = idcard;
            user.Email = email;
            user.WorkPlace_1 = workplace1;
            user.Business_scope_1 = businessscope1;
            user.WorkPlace_2 = workplace2;
            user.Business_scope_2 = businessscope2;
            user.WorkPlace_3 = workplace3;
            user.Business_scope_3 = businessscope3;


            db.SaveChanges();

            return Json(res);
        }
    }
}