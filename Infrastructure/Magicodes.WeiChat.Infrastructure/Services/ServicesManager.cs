using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Sms;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.Infrastructure.Services
{
    public class ServicesManager : ThreadSafeLazyBaseSingleton<ServicesManager>
    {
        /// <summary>
        ///     服务集合
        /// </summary>
        internal ConcurrentDictionary<string, object> Services = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 商城业务短信服务
        /// </summary>
        public ShopSmsService GetShopSmsService()
        {
            return new ShopSmsService();
        }

        /// <summary>
        /// 获取短信服务
        /// </summary>
        /// <param name="tenantKey"></param>
        /// <returns></returns>
        public ISmsService GetSmsService(object tenantKey = null)
        {
            var tenantId = tenantKey ?? WeiChatApplicationContext.Current.TenantId;
            var key = string.Format("ISmsService_{0}", tenantId);
            if (Services.ContainsKey(key))
            {
                return Services[key] as ISmsService;
            }
            else
            {
                if (ServicesConfig.GetSmsServiceByTenantId == null)
                    throw new ArgumentNullException("ServicesConfig.GetSmsServiceByTenantId");

                var smsService = ServicesConfig.GetSmsServiceByTenantId(tenantId);
                if (smsService == null)
                    return null;
                Services.AddOrUpdate(key, smsService, (tKey, existingVal) => { return smsService; });
                return smsService;
            }
        }
    }
}
