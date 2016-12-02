// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WebApiControllerBase.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Http.Controllers;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Interface;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.WebApiExtension.Filters;
using Microsoft.AspNet.Identity;

namespace Magicodes.Shop.Controllers.WebApi
{
    public class WebApiControllerBase : ApiController
    {
        protected AppDbContext db = new AppDbContext();
        protected Lazy<string> userId;
        protected Lazy<string> userName;

        public WebApiControllerBase()
        {
            userId = new Lazy<string>(() => User.Identity.GetUserId());
            userName = new Lazy<string>(() => User.Identity.GetUserName());
        }

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
        ///     访问凭据
        /// </summary>
        protected string AccessToken
        {
            get { return WeChatConfigManager.Current.GetAccessToken(); }
        }

        /// <summary>
        ///     租户Id（如果是系统租户则能获取到参数中的租户Id）
        /// </summary>
        public int TenantId
        {
            get { return WeiChatApplicationContext.Current.TenantId; }
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }

        protected bool SetModel<TModel, Tkey>(TModel model, Tkey key)
            where TModel : class, IAdminCreate<string>, IAdminUpdate<string>, ITenantId, new()
        {
            //判断是否为默认值
            if (EqualityComparer<Tkey>.Default.Equals(key, default(Tkey)))
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

        protected bool SetModelWithSaveChanges<TModel, Tkey>(TModel model, Tkey key)
            where TModel : class, IAdminCreate<string>, IAdminUpdate<string>, ITenantId, new()
        {
            var isAdd = SetModelWithChangeStates(model, key);
            db.SaveChanges();
            return isAdd;
        }

        protected bool SetModelWithChangeStates<TModel, Tkey>(TModel model, Tkey key)
            where TModel : class, IAdminCreate<string>, IAdminUpdate<string>, ITenantId, new()
        {
            var isAdd = SetModel(model, key);
            if (isAdd)
                db.Set<TModel>().Add(model);
            else
                db.Entry(model).State = EntityState.Modified;
            return isAdd;
        }
    }
}