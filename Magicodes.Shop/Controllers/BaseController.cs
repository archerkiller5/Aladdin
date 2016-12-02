// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : BaseController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Magicodes.Logger;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Interface;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Results;
using Microsoft.AspNet.Identity;

namespace Magicodes.Shop.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private readonly Lazy<LoggerBase> _logger = new Lazy<LoggerBase>(() => Loggers.Current.ControllerLogger);
        protected AppDbContext db = new AppDbContext();
        protected Lazy<bool> hasConfigWeiChat;
        protected Lazy<string> userId;
        protected Lazy<int> userIenantId;
        protected Lazy<string> userName;

        public BaseController()
        {
            userId = new Lazy<string>(() => User.Identity.GetUserId());
            userName = new Lazy<string>(() => User.Identity.GetUserName());
            userIenantId = new Lazy<int>(() => db.Users.Find(UserId).TenantId);
            hasConfigWeiChat = new Lazy<bool>(() => db.WeiChat_Apps.Any(p => p.TenantId == TenantId));
        }

        /// <summary>
        ///     日志记录器
        /// </summary>
        public LoggerBase Logger => _logger.Value;

        /// <summary>
        ///     访问凭据
        /// </summary>
        protected string AccessToken => WeChatConfigManager.Current.GetAccessToken();

        /// <summary>
        ///     是否已配置微信信息
        /// </summary>
        public bool HasConfigWeiChat => hasConfigWeiChat.Value;

        /// <summary>
        ///     获取当前用户Id
        /// </summary>
        public string UserId
        {
            get { return userId.Value; }
        }

        /// <summary>
        ///     当前用户名
        /// </summary>
        public string UserName
        {
            get { return userName.Value; }
        }

        /// <summary>
        ///     租户Id
        /// </summary>
        public int UserTenantId
        {
            get { return userIenantId.Value; }
        }

        /// <summary>
        ///     当前用户是否为系统租户
        /// </summary>
        public bool IsSystemIenant
        {
            get { return db.Admin_Tenants.Any(p => (p.Id == UserTenantId) && p.IsSystemTenant); }
        }

        /// <summary>
        ///     应用Id
        /// </summary>
        public int AppId => TenantId;

        /// <summary>
        ///     租户Id（如果是系统租户则能获取到参数中的租户Id）
        /// </summary>
        public int TenantId => WeiChatApplicationContext.Current.GetTenantId(HttpContext);

        /// <summary>
        ///     使用JSON.NET序列化
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="contentEncoding"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        protected override JsonResult Json(object data, string contentType,
            Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                JsonRequestBehavior = behavior,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }

        protected bool SetModelWithChangeStates<TModel, TKey>(TModel model, TKey key)
            where TModel : class, IAdminCreate<string>, IAdminUpdate<string>, ITenantId, new()
        {
            var isAdd = SetModel(model, key);
            if (isAdd)
                db.Set<TModel>().Add(model);
            else
                db.Entry(model).State = EntityState.Modified;
            return isAdd;
        }


        protected bool SetModelWithSaveChanges<TModel, TKey>(TModel model, TKey key)
            where TModel : class, IAdminCreate<string>, IAdminUpdate<string>, ITenantId, new()
        {
            var isAdd = SetModelWithChangeStates(model, key);
            db.SaveChanges();
            return isAdd;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="model"></param>
        /// <param name="key"></param>
        /// <returns>是否为添加</returns>
        protected bool SetModel<TModel, TKey>(TModel model, TKey key)
            where TModel : class, IAdminCreate<string>, IAdminUpdate<string>, ITenantId, new()
        {
            //判断是否为默认值
            if (EqualityComparer<TKey>.Default.Equals(key, default(TKey)))
            {
                model.CreateBy = UserId;
                model.CreateTime = DateTime.Now;
                model.TenantId = TenantId;
                return true;
            }
            db.Set<TModel>().Attach(model);
            //取数据库值
            var databaseValues = db.Entry(model).GetDatabaseValues();
            model.CreateBy = databaseValues.GetValue<string>("CreateBy");
            model.CreateTime = databaseValues.GetValue<DateTime>("CreateTime");
            model.TenantId = databaseValues.GetValue<int>("TenantId");
            model.UpdateTime = DateTime.Now;
            model.UpdateBy = UserId;
            return false;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}