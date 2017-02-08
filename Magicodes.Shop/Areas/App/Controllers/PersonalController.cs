// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : PersonalController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
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
using Magicodes.Shop.Helpers;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.WeiChat.Data.Models.User;
using Magicodes.WeiChat.Domain;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Magicodes.WeChat.SDK.Apis.User;
using System.Web;

namespace Magicodes.Shop.Areas.App.Controllers
{
    //暂时不用
    [RouteArea("App")]
    public class PersonalController : AppBaseController
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;
        
        [HttpGet]
        // GET: App/Personal
        [WeChatOAuth]
        public  ActionResult Index()
        {
            log.Log(LoggerLevels.Trace, "进入个人中心主页");
            log.Log(LoggerLevels.Trace, "openid：" + WeiChatApplicationContext.Current.WeiChatUser.OpenId);
            log.Log(LoggerLevels.Trace, " db.Log_Points：" + db.Log_Points.ToList().Count);
            var sumPoint =
                db.Log_Points.Where(p => p.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId)
                    .ToList()
                    .Sum(p => p.Point);

            log.Log(LoggerLevels.Trace, "sumPoint:" + sumPoint);
            //传给前台一个认证标识,用于控制前台是否显示认证图标ico
            var user = db.User_Infos.Find(WeiChatApplicationContext.Current.WeiChatUser.OpenId);
            if (user != null && (!string.IsNullOrEmpty(user.Mobile)))             
                ViewBag.Authentication = true;            
            else
                ViewBag.Authentication = false; 

                return View(WeiChatUser);
        }

        [HttpGet]
        [WeChatOAuth]
        public ActionResult MyOrder()
        {
            return View();
        }

        /// <summary>
        ///     手机认证
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult Authentication()
        {
            log.Log(LoggerLevels.Trace, "进入Authentication方法");
            var lst =
                    db.User_Infos.Find(WeiChatUser.OpenId);
            List<ListItem> li = new List<ListItem>();
            foreach (int s in Enum.GetValues(typeof(WeChatSexTypes)))
            {
                li.Add(new ListItem { Value=s.ToString(), Text  = Enum.GetName(typeof(WeChatSexTypes), s) });
            }
            ViewBag.Seax = new SelectList(li,dataTextField: "text", dataValueField:"value" ,selectedValue: lst.Sex);
            return View(lst);
        }

        //public List<ListItem> ToListItem<T>()
        //{
        //    List<ListItem> li = new List<ListItem>();
        //    foreach (int s in Enum.GetValues(typeof(T)))
        //    {
        //        li.Add(new ListItem { Value = s.ToString(), Text = Enum.GetName(typeof(T), s) });
        //    }
        //    return li;
        //}


        /// <summary>
        ///     地址信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult MyAddress()
        {
            log.Log(LoggerLevels.Trace, "进入MyAddress方法");
            var lst =
                db.User_Addresses.Where(p => p.OpenId == WeiChatUser.OpenId)
                    .ToList()
                    .OrderByDescending(p => p.CreateTime)
                    .ToList();
            return View(lst);
        }

        /// <summary>
        ///     编辑地址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult EditAddress(Guid? Id)
        {
            log.Log(LoggerLevels.Trace, "进入EditAddress");
            if (Id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var address = db.User_Addresses.Find(Id);
            if (address == null)
                return HttpNotFound();
            ViewBag.Id = Id;
            return View(address);
        }

        [HttpGet]
        //[WeChatOAuth]
        public ActionResult AddressInfo()
        {
            log.Log(LoggerLevels.Trace, "进入AddressInfo");
            return View();
        }

        /// <summary>
        ///     我的积分
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult MyPoints()
        {
            log.Log(LoggerLevels.Trace, "进入MyPoints");
            log.Log(LoggerLevels.Trace, WeiChatApplicationContext.Current.WeiChatUser.OpenId);
            var Lst =
                db.Log_Points.Where(p => p.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId).ToList();
            log.Log(LoggerLevels.Trace, Lst.Count.ToString());

            ViewBag.SumPoint = Lst.Sum(p => p.Point);
            log.Log(LoggerLevels.Trace, "ViewBag.SumPoint：" + ViewBag.SumPoint);
            return View(Lst);
        }

        /// <summary>
        ///     充值
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult Recharge()
        {
            log.Log(LoggerLevels.Trace, "进入Recharge");
            return View();
        }

        /// <summary>
        ///     我的账单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult MyBill()
        {
            log.Log(LoggerLevels.Trace, "进入MyBill");
            return View();
        }

        /// <summary>
        ///     提现记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult CashRecord()
        {
            log.Log(LoggerLevels.Trace, "进入CashRecord");
            var user = db.User_Infos.Find(WeiChatUser.OpenId);
            var Lst =
                db.Log_Withdraws.Where(p => p.OpenId == WeiChatUser.OpenId)
                    .OrderByDescending(p => p.CreateTime)
                    .ToList();
            return View(Lst);
        }

        /// <summary>
        ///     提现详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult CashDetails(long? id)
        {
            log.Log(LoggerLevels.Trace, "进入CashDetails");
            var user = db.User_Infos.Find(WeiChatUser.OpenId);
            ViewBag.Balance = user.Balance;
            var Withdraw = db.Log_Withdraws.Find(id);
            return View(Withdraw);
        }

        /// <summary>
        ///     提现申请
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [WeChatOAuth]
        public ActionResult CashApply()
        {
            log.Log(LoggerLevels.Trace, "进入CashApply");
            var setting = SettingDo.Current.GetSettings<Settings_Withdraw>(db);
            ViewBag.Setting = setting;
            var openID = WeiChatUser.OpenId;
            var user = db.User_Infos.Find(openID);
            ViewBag.Balance = user != null ? user.Balance : 0;
            ViewBag.HeadImgUrl = WeiChatUser.HeadImgUrl;
            ViewBag.NickName = WeiChatUser.NickName;
            var wfx_UserWithdrawLogList = db.Log_Withdraws.Where(p => p.OpenId == openID).ToList();
            return View(wfx_UserWithdrawLogList);
        }

        public ActionResult OrderDetails()
        {
            ViewBag.OrderId = Request["id"] ?? string.Empty;
            return View();
        }

        //支付成功
        public ActionResult PaySuccess()
        {
            return SuccessTip("支付成功", "您的订单已支付成功，我们将尽快为您发货，订单商品预计很快就会到达您的手中！",
                okUrl: Url.TenantAction("MyOrder", "Personal"));
        }

        [HttpGet]
        public ActionResult Pinjia()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ApplyRefund(Guid orderID)
        {
            ViewBag.OrderId = orderID;
            return View();
        }

        // GET: App/Personal/FillPersonalInfo
        [WeChatOAuth]
        public ActionResult FillPersonalInfo()
        {
            ViewBag.OpenId = WeiChatUser.OpenId;
            return View();
        }
        [HttpPost]
        public ActionResult FillPersonalInfo(User_Info model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var openId = model.OpenId;
            var user = db.User_Infos.FirstOrDefault(p => p.OpenId == openId);
            if (user == null)
            {
                user = new User_Info()
                {
                    CreateTime = DateTime.Now,
                    Mobile = model.Mobile,
                    OpenId = openId,
                    TenantId = TenantId,
                    TrueName = model.TrueName
                };
                db.User_Infos.Add(user);
            }
            else
            {
                user.TrueName = model.TrueName;
                user.Mobile = model.Mobile;
            }
            db.SaveChanges();
            return SuccessTip(title: "温馨提示", message: "操作成功！");
        }
    }
}