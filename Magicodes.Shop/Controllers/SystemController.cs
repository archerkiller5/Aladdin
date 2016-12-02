// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SystemController.cs
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
using System.Threading.Tasks;
using System.Web.Mvc;
using Magicodes.Shop.Models;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SystemController : BaseController
    {
        private readonly IdentityManager _identityManager = new IdentityManager();

        public AppUserManager UserManager
        {
            get { return _identityManager.UserManager; }
        }

        // GET: AdminUser
        public ActionResult AdminUser(int pageIndex = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<AdminUserViewModel>(
                db.Users.OrderBy(p => p.UserName)
                    .Skip((pageIndex - 1)*pageSize)
                    .Take(pageSize)
                    .ToList()
                    .Select(p => new AdminUserViewModel {UserName = p.UserName, Id = p.Id, PhoneNumber = p.PhoneNumber}),
                pageIndex, pageSize, db.Users.Count());
            return View(pagedList);
        }

        [HttpGet]
        [Route("System/AdminUser/Create")]
        public ActionResult CreateAdminUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("System/AdminUser/Create")]
        public async Task<ActionResult> CreateAdminUser(CreateAdminUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Id = Guid.NewGuid().ToString(),
                    PhoneNumber = model.PhoneNumber
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                    return RedirectToAction("AdminUser");
                AddErrors(result);
            }
            return View(model);
        }

        [HttpPost]
        [Route("System/AdminUser/Remove")]
        public ActionResult Remove(RemoveAdminUserViewModel model)
        {
            var message = new MessageInfo
            {
                Message = "操作成功！",
                MessageType = MessageTypes.Success
            };
            var user = UserManager.FindById(model.Id);
            if (user != null)
            {
                if (user.UserName.ToLower() == "admin")
                {
                    message.Message = "超级管理员账号无法删除！！";
                    message.MessageType = MessageTypes.Danger;
                }
                else
                    UserManager.Delete(user);
            }
            else
            {
                message.Message = "账号不存在！";
                message.MessageType = MessageTypes.Danger;
            }
            return Json(message);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }
    }
}