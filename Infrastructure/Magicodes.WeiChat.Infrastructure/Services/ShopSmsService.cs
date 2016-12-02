using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Sms;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Settings;

namespace Magicodes.WeiChat.Infrastructure.Services
{
    public class ShopSmsService : IDisposable
    {
        private readonly AppDbContext _dbContext = new AppDbContext();
        /// <summary>
        /// 发送手机绑定信息
        /// </summary>
        /// <param name="des">手机号码</param>
        /// <param name="code">验证码</param>
        ///  <param name="errMsg">返回消息</param>
        public bool SendBoundPhoneMsg(string des, string code,out string errMsg)
        {
            var sms = ServicesManager.Current.GetSmsService();
            var tenantId = WeiChatApplicationContext.Current.TenantId;
            var tpl =
                _dbContext.Settings_SmsTemplates.FirstOrDefault(p => p.TenantId == tenantId && p.SmsType == SmsTypes.BoundPhone);
            errMsg = "";
            if (tpl != null)
            {
                var result = sms.SendTemplateMessageAsync(new TemplateMessage()
                {
                    Destination = des,
                    SignName = tpl.SignName,
                    TemplateCode = tpl.TemplateCode,
                    Data = new Dictionary<string, string>()
                    {
                        {"code", code}
                    }
                });
                errMsg = result.Result.ErrorMessage;
                return result.Result.Success;
            }
            else
            {
                errMsg = "没有配置绑定手机的短信模板！";
                sms.Logger.Log(Logger.LoggerLevels.Error, "没有配置绑定手机的短信模板！");
                return false;
            }
        }
        /// <summary>
        /// 发送测试短信
        /// </summary>
        /// <param name="des"></param>
        public void SendTestMsg(string des)
        {
            var sms = ServicesManager.Current.GetSmsService();
            var tenantId = WeiChatApplicationContext.Current.TenantId;
            var tpl =
                _dbContext.Settings_SmsTemplates.FirstOrDefault(p => p.TenantId == tenantId &&
                p.SmsType == SmsTypes.TestSendMsg);
            if (tpl != null)
            {
                var result = sms.SendTemplateMessageAsync(
                    new TemplateMessage()
                    {
                        Destination = des,
                        SignName = tpl.SignName,
                        TemplateCode = tpl.TemplateCode,
                        Data = new Dictionary<string, string>()
                    });
            }
            else
            {
                sms.Logger.Log(Logger.LoggerLevels.Error, "没有配置绑定手机的短信模板！");
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
