// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SystemAdminBase.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:02
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc.Filters;
using Magicodes.Shop.Controllers;
using Magicodes.WeiChat.Data.Models.Interface;
using Microsoft.AspNet.Identity;

namespace Magicodes.Shop.Areas.SystemAdmin.Controllers
{
    public class SystemAdminBase<TModel, TKey> : BaseController
        where TModel : class, IAdminCreate<string>, IAdminUpdate<string>, ITenantId
    {
        protected void SaveModel(TModel model, bool isAdd, int? tenantId)
        {
            //判断是否为默认值
            if (isAdd)
            {
                model.CreateBy = UserId;
                model.CreateTime = DateTime.Now;
                model.TenantId = tenantId ?? UserTenantId;
                db.Set<TModel>().Add(model);
            }
            else
            {
                db.Set<TModel>().Attach(model);
                //取数据库值
                var databaseValues = db.Entry(model).GetDatabaseValues();
                model.CreateBy = databaseValues.GetValue<string>("CreateBy");
                model.CreateTime = databaseValues.GetValue<DateTime>("CreateTime");
                model.UpdateTime = DateTime.Now;
                model.UpdateBy = UserId;
                db.Entry(model).State = EntityState.Modified;
            }
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                filterContext.HttpContext.Response.Redirect("/SystemAdmin/Login");

            var userId = User.Identity.GetUserId();
            var tenantId = db.Users.Find(userId).TenantId;
            if (!db.Admin_Tenants.Any(p => (p.Id == tenantId) && p.IsSystemTenant))
                filterContext.HttpContext.Response.Redirect("/Account/Login");
            base.OnAuthentication(filterContext);
        }
    }
}