// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : TenantManager.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using EntityFramework.DynamicFilters;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.Infrastructure.Tenant
{
    /// <summary>
    ///     租户管理器
    /// </summary>
    public class TenantManager : ThreadSafeLazyBaseSingleton<TenantManager>
    {
        private const string tenantFilterName = "TenantEntryFilter";

        /// <summary>
        ///     启用多租户筛选器
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tenantId"></param>
        public void EnableTenantFilter(AppDbContext db, int tenantId)
        {
            db.EnableFilter(tenantFilterName);
            //设置多租户过滤
            db.SetFilterScopedParameterValue(tenantFilterName, "tenantId", tenantId);
        }

        /// <summary>
        ///     禁用多租户筛选器
        /// </summary>
        /// <param name="db"></param>
        public void DisableTenantFilter(AppDbContext db)
        {
            db.DisableFilter(tenantFilterName);
            db.ClearScopedParameters();
        }
    }
}