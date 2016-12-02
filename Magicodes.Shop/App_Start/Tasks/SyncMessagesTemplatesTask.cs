using Magicodes.Logger;
using Magicodes.Tasks;
using Magicodes.WeChat.SDK.Apis.TemplateMessage;
using Magicodes.WeChat.SDK.Apis.UserGroup;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure.Logging;
using Magicodes.WeiChat.Infrastructure.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magicodes.Shop.App_Start.Tasks
{
    /// <summary>
    /// 消息模板信息同步
    /// </summary>
    public class SyncMessagesTemplatesTask : TaskBase
    {

        public override string Title
        {
            get
            {
                return "消息模板信息同步";
            }
        }

        public override void Run()
        {
            SyncMessagesTemplates();
        }

        private void SyncMessagesTemplates()
        {
            var notify = NotifyInfo;
            var notifier = Notifier;
            var strs = Parameter.ToString().Split(';');
            var tenantId = int.Parse(strs[0]);
            var userId = strs[1];
            var logger = Logger;

            try
            {
                ReportProgress(1, "正在初始化同步信息");

                var templateMessageApi = new TemplateMessageApi();
                templateMessageApi.SetKey(tenantId);

                var result = templateMessageApi.Get();
               
                if (result.IsSuccess())
                {
                    if ((result.Templates != null) && (result.Templates.Length != 0))
                    {
                        ReportProgress(30, "已成功获取数据：" + result.Templates.Length);
                        using (var db = new AppDbContext())
                        {
                            TenantManager.Current.EnableTenantFilter(db, tenantId);
                            db.WeiChat_MessagesTemplateSendLogs.RemoveRange(db.WeiChat_MessagesTemplateSendLogs);
                            db.SaveChanges();
                            ReportProgress(40, "已清除所有的发送日志！");
                            db.WeiChat_MessagesTemplates.RemoveRange(db.WeiChat_MessagesTemplates);
                            db.SaveChanges();
                            ReportProgress(50, "已清除所有的消息模板！");

                            var tps = (from item in result.Templates
                                       select new WeiChat_MessagesTemplate
                                       {
                                           Demo = item.Example,
                                           OneIndustry = item.PrimaryIndustry,
                                           TemplateNo = item.TemplateId,
                                           Content = item.Content,
                                           TenantId = tenantId,
                                           Title = item.Title,
                                           TwoIndustry = item.DeputyIndustry,
                                           CreateBy = userId,
                                           CreateTime = DateTime.Now
                                       }).ToList();
                            db.WeiChat_MessagesTemplates.AddRange(tps);

                            var count = result.Templates == null ? 0 : result.Templates.Length;
                            ReportProgress(90, "正在写入数据,写入数量（" + count + "）...");
                            db.SaveChanges();
                            ReportProgress(100, "同步成功！同步数量（" + count + "）。");
                        }
                    }
                    else
                    {
                        ReportProgress(100, "同步成功！没有需要同步的数据。");
                    }
                }
                else
                {
                    ReportProgress(100, "同步失败！" + result.GetFriendlyMessage());
                }

            }
            catch (Exception ex)
            {
                logger.Log(LoggerLevels.Debug, Title + " 失败！");
                logger.LogException(ex);

                ReportProgress(100, "同步失败！" + ex.Message);
                return;
            }
        }
    }
}