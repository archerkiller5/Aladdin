// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Account.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.ComponentModel.DataAnnotations;
using Magicodes.Data.Multitenant;
using Magicodes.WeiChat.Data.Models.Interface;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Magicodes.WeiChat.Data.Models
{
    /// <summary>
    ///     租户信息
    /// </summary>
    public class Admin_Tenant : ITenant<int>
    {
        /// <summary>
        ///     是否为系统租户（仅支持一个）
        /// </summary>
        public bool IsSystemTenant { get; set; }

        /// <summary>
        ///     多租户Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     租户名称
        /// </summary>
        [Display(Name = "名称")]
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Display(Name = "备注")]
        [DataType(DataType.MultilineText)]
        [MaxLength(500)]
        public string Remark { get; set; }
    }

    //如果需要使用Int类型主键，请参考以下链接：
    //http://stackoverflow.com/questions/19553424/how-to-change-type-of-id-in-microsoft-aspnet-identity-entityframework-identityus
    //http://www.codeproject.com/Articles/777733/ASP-NET-Identity-Change-Primary-Key
    /// <summary>
    ///     用户
    /// </summary>
    public class AppUser : MultitenantIdentityUser<string, int, AppUserLogin, AppUserRole, AppUserClaim>,
        IUser<string>
    {
        /// <summary>
        ///     用户的标识，对当前公众号唯一
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "微信OPENID")]
        public string OpenId { get; set; }

        [Display(Name = "邮箱")]
        public override string Email
        {
            get { return base.Email; }
            set { base.Email = value; }
        }

        [Display(Name = "手机号码")]
        public override string PhoneNumber
        {
            get { return base.PhoneNumber; }
            set { base.PhoneNumber = value; }
        }

        [Display(Name = "手机确认")]
        public override bool PhoneNumberConfirmed
        {
            get { return base.PhoneNumberConfirmed; }
            set { base.PhoneNumberConfirmed = value; }
        }

        [Display(Name = "邮箱确认")]
        public override bool EmailConfirmed
        {
            get { return base.EmailConfirmed; }
            set { base.EmailConfirmed = value; }
        }

        [Display(Name = "访问失败次数")]
        public override int AccessFailedCount
        {
            get { return base.AccessFailedCount; }
            set { base.AccessFailedCount = value; }
        }

        [Display(Name = "两次认证")]
        public override bool TwoFactorEnabled
        {
            get { return base.TwoFactorEnabled; }
            set { base.TwoFactorEnabled = value; }
        }

        [Display(Name = "锁定")]
        public override bool LockoutEnabled
        {
            get { return base.LockoutEnabled; }
            set { base.LockoutEnabled = value; }
        }

        [Display(Name = "锁定截止时间")]
        public override DateTime? LockoutEndDateUtc
        {
            get { return base.LockoutEndDateUtc; }
            set { base.LockoutEndDateUtc = value; }
        }

        /// <summary>
        ///     代理的租户Id（仅对管理员有效）
        /// </summary>
        public int AgentTennantId { get; set; }

        [Display(Name = "用户名")]
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value; }
        }
    }

    public class AppRole : IdentityRole<string, AppUserRole>
    {
        public AppRole()
        {
        }

        public AppRole(string name, string description)
        {
            Description = description;
        }

        /// <summary>
        ///     角色描述
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "描述")]
        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }
    }

    public class AppUserLogin : MultitenantIdentityUserLogin<string, int>
    {
    }

    public class AppUserRole : IdentityUserRole
    {
    }

    public class AppUserClaim : IdentityUserClaim
    {
    }

    public class AppUserStore :
        MultitenantUserStore<AppUser, AppRole, string, int, AppUserLogin, AppUserRole, AppUserClaim>,
        IUserStore<AppUser>
    {
        public AppUserStore()
            : base(new AppDbContext())
        {
        }

        public AppUserStore(AppDbContext context)
            : base(context)
        {
        }
    }

    public class AppRoleStore : RoleStore<AppRole, string, AppUserRole>,
        IRoleStore<AppRole>
    {
        public AppRoleStore()
            : base(new AppDbContext())
        {
        }

        public AppRoleStore(AppDbContext context)
            : base(context)
        {
        }
    }
}