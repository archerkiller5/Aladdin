using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Data.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Domain.Log
{
    public class PayLogDO : DomainObjectBase
    {
        private static object lockObj = new object();
        private int CurrentTenantId { get; set; }
        private string OpenId { get; set; }
        private string ClientIpAddress { get; set; }
        public PayLogDO(int tenantId, string openId, string clientIpAddress, AppDbContext context = null)
        {
            CurrentTenantId = tenantId;
            OpenId = openId;
            ClientIpAddress = clientIpAddress;
            if (context == null)
            {
                Db = new AppDbContext();
                isToDisposeDb = true;
            }
            else
                Db = context;
        }

        /// <summary>
        /// 记录订单日志
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="amount">金额</param>
        /// <param name="paymentInterfaceLog">支付接口日志</param>
        /// <param name="errorLog">错误日志</param>
        public void AddOrderLog(Guid orderId, decimal amount, string paymentInterfaceLog = null, string errorLog = null)
        {
            var orderLog = new Log_Order()
            {
                Amount = amount,
                CreateTime = DateTime.Now,
                OpenId = OpenId,
                OrderId = orderId,
                PaymentType = EnumThirdPayType.WX,
                TenantId = CurrentTenantId
            };
            Db.Log_Orders.Add(orderLog);
            var financialMonitoringsLog = new Log_FinancialMonitoring()
            {
                TenantId = CurrentTenantId,
                Amount = amount,
                ClientIpAddress = ClientIpAddress,
                CreateTime = DateTime.Now,
                IsSuccess = string.IsNullOrEmpty(errorLog),
                OpenId = OpenId,
                PaymentInterfaceLog = paymentInterfaceLog,
                ErrorLog = errorLog,
                Type = FinancialMonitoringTypes.OrderPay
            };
            Db.Log_FinancialMonitorings.Add(financialMonitoringsLog);
            Db.SaveChanges();
        }

        /// <summary>
        /// 充值日志记录
        /// </summary>
        /// <param name="amount">金额</param>
        /// <param name="paymentInterfaceLog">支付接口日志</param>
        /// <param name="errorLog">错误日志</param>
        public void AddRechargeLog(decimal amount, string paymentInterfaceLog = null, string errorLog = null)
        {
            var log = new Log_Recharge()
            {
                Amount = amount,
                CreateTime = DateTime.Now,
                OpenId = OpenId,
                TenantId = CurrentTenantId
            };
            Db.Log_Recharges.Add(log);
            var financialMonitoringsLog = new Log_FinancialMonitoring()
            {
                TenantId = CurrentTenantId,
                Amount = amount,
                ClientIpAddress = ClientIpAddress,
                CreateTime = DateTime.Now,
                IsSuccess = string.IsNullOrEmpty(errorLog),
                OpenId = OpenId,
                PaymentInterfaceLog = paymentInterfaceLog,
                ErrorLog = errorLog,
                Type = FinancialMonitoringTypes.Recharge
            };
            Db.Log_FinancialMonitorings.Add(financialMonitoringsLog);
            lock (lockObj)
            {
                var user = Db.User_Infos.Find(OpenId);
                if (user == null)
                {
                    user = new Data.Models.User.User_Info()
                    {
                        OpenId = OpenId,
                        Balance = 0,
                        Integral = 0,
                        State = EnumUserState.Normal,
                        TenantId = TenantId,
                        CreateTime = DateTime.Now,
                        LastLoginOn = DateTime.Now,
                        LoginCount = 0
                    };
                    Db.User_Infos.Add(user);
                }
                user.Balance = user.Balance + amount;
                Db.SaveChanges();
            }
        }
    }
}
