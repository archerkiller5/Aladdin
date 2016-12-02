// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IdentityManager.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Linq;
using System.Web;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Magicodes.WeiChat.Infrastructure.Identity
{
    public class IdentityManager
    {
        private readonly AppDbContext _db = new AppDbContext();

        public RoleManager<AppRole> RoleManager { get; } = new RoleManager<AppRole>(
            new AppRoleStore(new AppDbContext()));

        private HttpContextBase Context
        {
            get { return new HttpContextWrapper(HttpContext.Current); }
        }

        /// <summary>
        ///     获取用户管理器
        /// </summary>
        public AppUserManager UserManager
        {
            get { return Context.GetOwinContext().GetUserManager<AppUserManager>(); }
        }


        public IAuthenticationManager AuthenticationManager
        {
            get { return Context.GetOwinContext().Authentication; }
        }

        public bool RoleExists(string name)
        {
            return RoleManager.RoleExists(name);
        }


        public bool CreateUser(AppUser user, string password)
        {
            var idResult = UserManager.Create(user, password);
            return idResult.Succeeded;
        }


        public bool AddUserToRole(string userId, string roleName)
        {
            var idResult = UserManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }


        public void ClearUserRoles(string userId)
        {
            var user = UserManager.FindById(userId);
            var roles = _db.Roles.Where(p => user.Roles.Any(p1 => p1.RoleId == p.Id)).Select(p => p.Name).ToArray();
            UserManager.RemoveFromRoles(userId, roles);
        }


        public void RemoveFromRole(string userId, string roleName)
        {
            UserManager.RemoveFromRole(userId, roleName);
        }


        public void DeleteRole(string roleId)
        {
            var roleUsers = _db.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId));
            var role = _db.Roles.Find(roleId);

            foreach (var user in roleUsers)
                RemoveFromRole(user.Id, role.Name);
            _db.Roles.Remove(role);
            _db.SaveChanges();
        }
    }
}