// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : LoginController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:02
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Magicodes.Shop.Models;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Magicodes.Shop.Areas.SystemAdmin.Controllers
{
    [RouteArea("SystemAdmin")]
    [RoutePrefix("Login")]
    [Route("{action}")]
    public class LoginController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        public LoginController()
        {
            UserManager = new UserManager<AppUser, string>(new AppUserStore(new AppDbContext())
            {
                TenantId = db.Admin_Tenants.First(p => p.IsSystemTenant).Id
            });
        }

        public UserManager<AppUser, string> UserManager { get; }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Login");
        }

        [AllowAnonymous]
        [Route("")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Title = string.Format("{0} | 系统登录", WebConfigurationManager.AppSettings["CustomerInformation"]);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("")]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    try
                    {
                        await SignInAsync(user, model.RememberMe);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        return View(model);
                    }
                    return RedirectToAction("Index", "AdminHome");
                }
                ModelState.AddModelError("", "用户名或密码错误！");
            }
            return View(model);
        }

        private async Task SignInAsync(AppUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
        }
    }
}