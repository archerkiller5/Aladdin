using Magicodes.Logger.NLog;
using Magicodes.Notify;
using Magicodes.Notify.SignalR;
using Magicodes.Notify.SignalR.Builder;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Magicodes.Shop.App_Start
{
     
    public static class Magicodes_Notify_Config
    {
        
        public static void Builder()
        {
            NotifyBuilder<Site_Notify>
                .Create()
                .WithOnReconnected((context, groups) =>
                {
                    var logger = new NLogLogger("Notify");
                    logger.Log(Logger.LoggerLevels.Debug, "OnReconnected...");
                })
                .WithOnConnected((context, groups) =>
                {
                    var logger = new NLogLogger("Notify");
                    logger.Log(Logger.LoggerLevels.Debug, "OnConnected...");
                    if (context.User.Identity.IsAuthenticated)
                    {
                        var userId = context.User.Identity.GetUserId();
                        using (AppDbContext db = new AppDbContext())
                        {
                            var userKey = "User_" + userId;
                            groups.Add(context.ConnectionId, userKey);
                            var user = db.Users.Find(userId);
                            //根据登录角色获取
                            var tenantId = user.TenantId;
                            var agentTennantId = user.AgentTennantId;
                            //如果当前租户为系统租户，则允许代理其他租户操作
                            if (db.Admin_Tenants.Any(p => (p.Id == tenantId) && p.IsSystemTenant))
                            {
                                groups.Add(context.ConnectionId, "Admin");
                                //系统租户使用AgentTennantId
                                if ((user.AgentTennantId != default(int)) && (user.AgentTennantId != tenantId))
                                    tenantId = user.AgentTennantId;
                            }
                            var tenantKey = "Tenant_" + tenantId;
                            groups.Add(context.ConnectionId, tenantKey);

                            //连接时通知
                            var notifier = WeiChatApplicationContext.Current.Notifier;
                            var queryable = db.Site_Notifies.Where(p => (p.Receiver == userKey || p.Receiver == tenantKey) && !db.Site_ReadNotifies.Any(p1 => p1.NotifyId == p.Id && p1.CreateBy == userId));
                            var lastNotifies = queryable.OrderByDescending(p => p.UpdateTime).Take(8).ToList<INotifyInfo>();
                            notifier.NotifyTo(lastNotifies, userKey);
                        }
                    }
                })
                .WithOnClientNotify((notifyStr, groupName, context, groups) =>
                {
                    if (context.User.Identity.IsAuthenticated)
                    {
                        var userId = context.User.Identity.GetUserId();
                        var notifyInfo = JsonConvert.DeserializeObject<Site_Notify>(notifyStr);
                        using (var db = new AppDbContext())
                        {
                            if (notifyInfo.Id == default(int))
                            {
                                notifyInfo.CreateTime = DateTime.Now;
                                notifyInfo.CreateBy = userId;
                                notifyInfo.UpdateTime = DateTime.Now;
                                notifyInfo.Receiver = groupName;
                                db.Site_Notifies.Add(notifyInfo);
                            }
                            else
                            {
                                notifyInfo.UpdateTime = DateTime.Now;
                                notifyInfo.Href = notifyInfo.Href;
                                notifyInfo.IconCls = notifyInfo.IconCls;
                                notifyInfo.IsTaskFinish = notifyInfo.IsTaskFinish;
                                notifyInfo.Message = notifyInfo.Message;
                                notifyInfo.Receiver = groupName;
                                notifyInfo.TaskPercentage = notifyInfo.TaskPercentage;
                                notifyInfo.Title = notifyInfo.Title;
                                db.Entry(notifyInfo).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            return notifyInfo;
                        }
                    }


                    return null;
                })
                .Build();

            WeiChatApplicationContext.Current.Notifier = new SignalRNotifier<Site_Notify>();


        }
    }
}