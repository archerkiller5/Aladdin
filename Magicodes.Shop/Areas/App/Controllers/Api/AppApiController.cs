// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Interface;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.WebApiExtension.Filters;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    public class AppApiController : ApiController
    {
        protected AppDbContext db = new AppDbContext();

        public WeiChat_User WeChatUser
        {
            get { return WeiChatApplicationContext.Current.WeiChatUser; }
        }

        public string OpenId
        {
            get { return WeChatUser.OpenId; }
        }

        /// <summary>
        ///     租户Id（如果是系统租户则能获取到参数中的租户Id）
        /// </summary>
        public int TenantId
        {
            get { return WeiChatApplicationContext.Current.TenantId; }
        }


        protected bool SetModel<TModel, Tkey>(TModel model, Tkey key)
            where TModel : WeiChat_WeChatBase<Tkey>, new()
        {
            //判断是否为默认值
            if (EqualityComparer<Tkey>.Default.Equals(key, default(Tkey)))
            {
                model.OpenId = OpenId;
                model.CreateTime = DateTime.Now;
                model.TenantId = TenantId;
                return true;
            }
            db.Set<TModel>().Attach(model);
            //取数据库值
            var databaseValues = db.Entry(model).GetDatabaseValues();
            model.OpenId = databaseValues.GetValue<string>("OpenId");
            model.CreateTime = databaseValues.GetValue<DateTime>("CreateTime");
            model.TenantId = databaseValues.GetValue<int>("TenantId");
            return false;
        }

        protected bool SetModelWithSaveChanges<TModel, Tkey>(TModel model, Tkey key)
            where TModel : WeiChat_WeChatBase<Tkey>, ITenantId, new()
        {
            var isAdd = SetModelWithChangeStates(model, key);
            db.SaveChanges();
            return isAdd;
        }

        protected bool SetModelWithChangeStates<TModel, Tkey>(TModel model, Tkey key)
            where TModel : WeiChat_WeChatBase<Tkey>, ITenantId, new()
        {
            var isAdd = SetModel(model, key);
            if (isAdd)
                db.Set<TModel>().Add(model);
            else
                db.Entry(model).State = EntityState.Modified;
            return isAdd;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}