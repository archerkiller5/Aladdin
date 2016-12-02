// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : TenantBaseApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Http.Controllers;
using Magicodes.WeiChat.Data.Models.Interface;
using Magicodes.WeiChat.Infrastructure.Tenant;
using Magicodes.WeiChat.Infrastructure.WebApiExtension.Filters;

namespace Magicodes.Shop.Controllers.WebApi
{
    public class TenantBaseApiController<TEntry> : WebApiControllerBase
        where TEntry : class, ITenantId, new()
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            var tenantId = TenantId;
            TenantManager.Current.EnableTenantFilter(db, tenantId);
            //TenantManager.Current.DisableTenantFilter(db);

            base.Initialize(controllerContext);
        }
    }
}