// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings_AliMsgController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Top.Api;
using Top.Api.Request;
using Magicodes.WeiChat.Infrastructure.Services;

namespace Magicodes.Shop.Controllers.Settings
{
    public class Settings_AliMsgController : AdminUniqueTenantBaseController<Settings_AliMsg>
    {
        [RoleMenuFilter("阿里短信接口设置", "60511405-1D94-4DB2-ACAF-62F421AF5C8D", "Admin,TenantManager,ShopManager",
             url: "/Settings_AliMsg", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        // GET:  Settings_AliMsg
        [AuditFilter("阿里短信接口设置页", "C6AB92DE-100D-4C77-A967-99854A46B59E")]
        public ActionResult Index()
        {
            return View();
        }

        [AuditFilter("阿里短信接口设置", "F952E2C8-BF06-4700-BA4F-A4985B3FDFC8")]
        [HttpPost]
        public override ActionResult Index(Settings_AliMsg model)
        {
            ViewBag.Success = false;
            ViewBag.Message = "";
            if (string.IsNullOrWhiteSpace(model.AppKey))
            {
                ViewBag.Message = "AppKey不能为空!";
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.Secret))
            {
                ViewBag.Message = "接口密钥不能为空!";
                return View(model);
            }
           
            return base.Index(model);
        }

       
       

        [AuditFilter("阿里短信接口测试", "809C6345-4FE4-4C8F-A6C0-84A5EBFD04D8")]
        [HttpGet]
        public ActionResult TestMsg()
        {
            ViewBag.Success = false;
            ViewBag.Message = "";
            return View();
        }

        [HttpPost]
        public ActionResult TestMsg(string Extend, string sms_free_sign_name, string sms_template_code, string sms_param,
            string rec_num)
        {
            ViewBag.Success = false;
            ViewBag.Message = "";
            var aliMsg =
                db.Settings_AliMsgs.FirstOrDefault(p => p.TenantId == WeiChatApplicationContext.Current.TenantId);
            if (aliMsg == null)
                throw new Exception("未对短信接口进行配置!");
            //var url = aliMsg.ServerUrl;
            //var appkey = aliMsg.AppKey;
            //var secret = aliMsg.Secret;
            //ITopClient client = new DefaultTopClient(url, appkey, secret);
            //var req = new AlibabaAliqinFcSmsNumSendRequest();
            //req.Extend = Extend; //"123456";
            //req.SmsType = "normal";
            //req.SmsFreeSignName = sms_free_sign_name; //"阿里大于";
            //req.SmsParam = "{\"code\":\"1234\"}"; //"{code:1234}"; //sms_param
            //req.RecNum = rec_num; // "13000000000";
            //req.SmsTemplateCode = sms_template_code; //"SMS_585014";
            //var rsp = client.Execute(req);
            //if (rsp.ErrCode.Trim() == "0") ViewBag.Success = true;
            //else
            //{
            //    ViewBag.Success = false;
            //    ViewBag.Message = rsp.SubErrMsg;
            //}
            //Console.WriteLine(rsp.Body);
            //调用阿里云接口            
            return View();
        }
    }
}