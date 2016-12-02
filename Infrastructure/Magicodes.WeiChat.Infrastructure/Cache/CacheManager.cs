// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : CacheManager.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using CacheManager.Core;
using Magicodes.WeiChat.Unity;
using Microsoft.Practices.Unity;

namespace Magicodes.WeiChat.Infrastructure.Cache
{
    /// <summary>
    ///     缓存管理
    /// </summary>
    public class CacheManager : ThreadSafeLazyBaseSingleton<CacheManager>
    {
        /// <summary>
        ///     缓存实例名称
        /// </summary>
        private const string DefaultCacheName = "defaultCache";

        /// <summary>
        ///     Unity缓存容器
        /// </summary>
        private readonly IUnityContainer cacheContainer = new UnityContainer();

        /// <summary>
        ///     获取缓存实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ICacheManager<T> GetCacheManager<T>()
        {
            ICacheManager<T> cache = null;
            if (cacheContainer.IsRegistered<ICacheManager<T>>())
            {
                cache = cacheContainer.Resolve<ICacheManager<T>>();
            }
            else
            {
                cache = CacheFactory.FromConfiguration<T>(DefaultCacheName);
                cacheContainer.RegisterInstance(cache);
            }
            return cache;
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">key</param>
        /// <returns>值</returns>
        public T Get<T>(string key)
        {
            var cache = GetCacheManager<T>();
            return cache.Get<T>(key);
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="tenantKey">租户Key值，如果为NULL则框架会自动获取当前账户的租户Id</param>
        /// <returns>值</returns>
        public T GetByTenant<T>(string key, string tenantKey = null)
        {
            var cache = GetCacheManager<T>();
            var tid = string.IsNullOrEmpty(tenantKey)
                ? WeiChatApplicationContext.Current.TenantId.ToString()
                : tenantKey;
            return cache.Get<T>(key, tid);
        }

        /// <summary>
        ///     添加或更新缓存
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        public void AddOrUpdate<T>(string key, T value)
        {
            var cache = GetCacheManager<T>();
            cache.AddOrUpdate(key, value, o => o);
        }

        /// <summary>
        ///     添加或更新缓存
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        public void AddOrUpdate<T>(string key, T value, TimeSpan expire)
        {
            var cache = GetCacheManager<T>();
            cache.AddOrUpdate(key, value, o => o);
            cache.Expire(key, expire);
        }

        /// <summary>
        ///     根据租户缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="tenantKey">租户Key值，如果为NULL则框架会自动获取当前账户的租户Id</param>
        public void AddOrUpdateByTenant<T>(string key, T value, string tenantKey = null)
        {
            var cache = GetCacheManager<T>();
            var tid = string.IsNullOrEmpty(tenantKey)
                ? WeiChatApplicationContext.Current.TenantId.ToString()
                : tenantKey;
            cache.AddOrUpdate(key, tid, value, o => o);
        }

        /// <summary>
        ///     根据租户缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <param name="tenantKey"></param>
        public void AddOrUpdateByTenant<T>(string key, T value, TimeSpan expire, string tenantKey = null)
        {
            var cache = GetCacheManager<T>();
            var tid = string.IsNullOrEmpty(tenantKey)
                ? WeiChatApplicationContext.Current.TenantId.ToString()
                : tenantKey;
            cache.AddOrUpdate(key, tid, value, o => o);
            cache.Expire(key, expire);
        }

        /// <summary>
        ///     移除
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否移除成功</returns>
        public bool Remove<T>(string key)
        {
            var cache = GetCacheManager<T>();
            return cache.Remove(key);
        }

        /// <summary>
        ///     根据租户移除
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="tenantKey">租户Key值，如果为NULL则框架会自动获取当前账户的租户Id</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveByTenant<T>(string key, string tenantKey = null)
        {
            var cache = GetCacheManager<T>();
            var tid = string.IsNullOrEmpty(tenantKey)
                ? WeiChatApplicationContext.Current.TenantId.ToString()
                : tenantKey;
            return cache.Remove(key, tid);
        }

        /// <summary>
        ///     清理所有
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        public void Clear<T>()
        {
            var cache = GetCacheManager<T>();
            cache.Clear();
        }

        /// <summary>
        ///     清理所有
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="tenantKey">租户Key值，如果为NULL则框架会自动获取当前账户的租户Id</param>
        public void ClearByTenant<T>(string tenantKey = null)
        {
            var cache = GetCacheManager<T>();
            var tid = string.IsNullOrEmpty(tenantKey)
                ? WeiChatApplicationContext.Current.TenantId.ToString()
                : tenantKey;
            cache.ClearRegion(tid);
        }
    }
}