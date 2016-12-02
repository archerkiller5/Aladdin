using System;
using System.Linq;
using Magicodes.Logger.NLog;
using Magicodes.Sms;
using Magicodes.Sms.Alidayu;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Infrastructure.Services;

namespace Magicodes.Shop
{
    public class ServiceConfigs
    {
        public static void Register()
        {
            ServicesConfig.GetSmsServiceByTenantId = new Func<object, ISmsService>((tenantKey) =>
              {
                  using (var db = new AppDbContext())
                  {
                      var logger = new NLogLogger("Sms");
                      var tenantId = (int)tenantKey;
                      var config = db.Settings_AliMsgs.FirstOrDefault(p => p.TenantId == tenantId);
                      if (config != null)
                          return new AlidayuSmsService(logger, config.AppKey, config.Secret);
                      logger.Log(Logger.LoggerLevels.Error,"您尚未配置短信接口，将无法发送短信");
                      return null;
                  }
              });
        }
    }
}