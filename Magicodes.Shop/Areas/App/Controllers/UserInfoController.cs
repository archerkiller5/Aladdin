using Magicodes.Shop.Controllers;
using Magicodes.Shop.Models;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Data.Models.User;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.Identity;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using Magicodes.WeiChat.Unity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Magicodes.Shop.Areas.App.Controllers
{
    public class UserInfoController : BaseController
    {
        IdentityManager _identityManager = new IdentityManager();
        // GET: App/UserInfo
        public ActionResult Index()
        {
            return View();
        }
        public AppUserManager UserManager
        {
            get
            {
                return _identityManager.UserManager;            
            }

        }
       
        public ActionResult MemberlLogin(string returnU)
        {
            ViewBag.retu = returnU;
            ViewBag.Title = string.Format("{0} | 登录", WebConfigurationManager.AppSettings["CustomerInformation"]);

            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        [WeChatOAuth]
        public ActionResult MemberlLogin(User_Info model, string returnUrl)
        {
            string OpenId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
            var open = db.User_Infos.Find(OpenId);
            if (open != null) {
               // return View("~/App/Personal?tenantId=1");
                return RedirectToAction("Index", "Personal", new {TenantId=1 });
            }
            else { 
                if (ModelState.IsValid)
                {
                    string userid = model.Userid;
                    string pew = model.Pwd;
                    var user = db.User_Infos.Where(a => a.Userid == userid && a.Pwd == pew).ToList();

                    if (user.Count == 1)
                    {
                        try
                        {
                            var weichar = db.WeiChat_Users.Where(q => q.Remark == userid).ToList();
                            WeiChat_User wei = new WeiChat_User();
                            wei.OpenId = weichar[0].OpenId;
                            var weiChat_User = db.WeiChat_Users.Find(wei.OpenId);
                            //删除
                            db.WeiChat_Users.Remove(weiChat_User);
                            db.SaveChanges();
                            //新增
                            wei = weichar[0];
                            wei.OpenId = weichar[0].OpenId = OpenId;

                            // wei.Subscribe = weichar[0].Subscribe;
                            // wei.NickName = weichar[0].NickName;
                            // wei.Sex = weichar[0].Sex;
                            // wei.City = weichar[0].City;
                            // wei.Country = weichar[0].Country;
                            // wei.Province = weichar[0].Province;
                            // wei.Language = weichar[0].Language;
                            // wei.HeadImgUrl = weichar[0].HeadImgUrl;
                            // wei.SubscribeTime = weichar[0].SubscribeTime;
                            //// wei.UnionId = weichar[0].UnionId;
                            // wei.Remark = weichar[0].Remark;
                            // wei.GroupId = weichar[0].GroupId;
                            // wei.AllowTest = weichar[0].AllowTest;
                            // wei.TenantId = weichar[0].TenantId;
                            db.WeiChat_Users.Add(wei);
                            db.SaveChanges();
                            //userinfo 删除
                            var info = db.User_Infos.Where(w => w.Userid == userid).ToList();
                            model.OpenId = info[0].OpenId;
                            var user_info = db.User_Infos.Find(model.OpenId);
                            db.User_Infos.Remove(user_info);
                            db.SaveChanges();
                            //新增
                            model.OpenId = info[0].OpenId = OpenId;
                            db.User_Infos.Add(info[0]);
                            db.SaveChanges();

                            // model.Userid = info[0].Userid;
                            // model.Pwd = info[0].Pwd;
                            // model.Email = info[0].Email;
                            // model.Mobile = info[0].Mobile;
                            // model.NickName = info[0].NickName;
                            // model.TrueName = info[0].TrueName;
                            // model.IdCard = info[0].IdCard;
                            // model.Address = info[0].Address;
                            // model.WorkPlace_1 = info[0].WorkPlace_1;



                            // info.WorkPlace_1 = model.Userinfo.WorkPlace_1;
                            // info.WorkPlace_2 = model.Userinfo.WorkPlace_2;
                            // info.WorkPlace_3 = model.Userinfo.WorkPlace_3;
                            // info.Business_scope_1 = model.Userinfo.Business_scope_1;
                            // info.Business_scope_2 = model.Userinfo.Business_scope_2;
                            // info.Business_scope_3 = model.Userinfo.Business_scope_3;
                            // info.Tel_1 = model.Userinfo.Tel_1;
                            // info.Tel_2 = model.Userinfo.Tel_2;
                            // info.Tel_3 = model.Userinfo.Tel_3;
                            // weichat.OpenId = info.Userid;
                            // info.CreateTime = DateTime.Now;
                            // info.State = 0;
                            // info.Integral = 0;
                            // info.Balance = 0;
                            // info.LastLoginOn = DateTime.Now;
                            //db.User_Infos.Add(model);
                            //db.SaveChanges();
                            //await SignInAsync(model, model.RememberMe);
                            //    //登陆日志
                            //LoginSuccess(model.Userid, model.Pwd, model.TenantId);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", ex.Message);
                            //登陆日志,失败
                            // LoginFail(model.Userid, model.Pwd);
                            return View(model);
                        }
                        return RedirectToAction("Index", "Personal", new { TenantId = 1 });
                        // return View("~/App/Personal?tenantId=1");
                    }
                    else
                    {
                        //登陆日志,失败
                        LoginFail(model.Userid, model.Pwd);

                        ModelState.AddModelError("", "用户名或密码错误！");
                    }
                }
            }
            return View(model);
        }
        //private async Task SignInAsync(User_Info user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        //    var identity = await db.User_Infos.Create(user, DefaultAuthenticationTypes.ApplicationCookie);
        //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}
        public void LoginSuccess(string loginName, string UserId, int tenantId)
        {
            var loginSuccess = new Log_LoginSuccess
            {
                BrowserInfo = JsonConvert.SerializeObject(HttpContext.Request.Browser),
                ClientIpAddress = IpAddressHelper.GetIP(),
                ClientName = HttpContext.Request.UserHostName,
                CreateBy = UserId,
                CreateTime = DateTime.Now,
                LoginName = loginName,
                TenantId = tenantId
            };
            db.Log_LoginSuccess.Add(loginSuccess);
            db.SaveChanges();

        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return _identityManager.AuthenticationManager;
            }
        }
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public void LoginFail(string loginName, string passWord)
        {
            var loginFail = new Log_LoginFail
            {
                BrowserInfo = JsonConvert.SerializeObject(HttpContext.Request.Browser),
                ClientIpAddress = IpAddressHelper.GetIP(),
                ClientName = HttpContext.Request.UserHostName,
                CreateTime = DateTime.Now,
                LoginName = loginName,
                Password = passWord
            };

            db.Log_LoginFail.Add(loginFail);
            db.SaveChanges();
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }




    }
}