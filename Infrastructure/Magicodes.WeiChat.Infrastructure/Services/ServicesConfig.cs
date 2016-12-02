using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Sms;

namespace Magicodes.WeiChat.Infrastructure.Services
{
    public static class ServicesConfig
    {
        /// <summary>
        /// 根据TenantId获取短信服务
        /// </summary>
        public static Func<object, ISmsService> GetSmsServiceByTenantId { get; set; }
    }
}
