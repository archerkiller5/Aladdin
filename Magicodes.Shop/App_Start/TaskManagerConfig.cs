using Hangfire;
using Magicodes.Logger.NLog;
using Magicodes.Tasks.Builder;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magicodes.Shop.App_Start
{
    public class TaskManagerConfig
    {
        /// <summary>
        /// 配置任务管理器
        /// </summary>
        public static void ConfigTaskManager()
        {
            var taskManager =
                TaskBuilder.Create()
                //设置日志记录
                .WithLoggers(new NLogLogger("TaskManager"), new NLogLogger("Task"))
                //设置通知器
                .WithNotifier(WeiChatApplicationContext.Current.Notifier)
                .WithTaskCompleteAction((task) =>
                {
                    var notifyInfo = task.NotifyInfo as Site_Notify;
                    using (var db = new AppDbContext())
                    {
                        db.Site_Notifies.Add(notifyInfo);
                        db.SaveChanges();
                    }
                })
                .Build();

            WeiChatApplicationContext.Current.TaskManager = taskManager;

        }
    }
}