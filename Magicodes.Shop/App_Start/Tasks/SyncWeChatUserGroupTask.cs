using Magicodes.Logger;
using Magicodes.Tasks;
using Magicodes.WeChat.SDK.Apis.UserGroup;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.Logging;
using Magicodes.WeiChat.Infrastructure.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Magicodes.Shop.App_Start.Tasks
{
    /// <summary>
    /// 粉丝标签信息同步
    /// </summary>
    public class SyncWeChatUserGroupTask : TaskBase
    {

        public override string Title
        {
            get
            {
                return "粉丝标签信息同步";
            }
        }

        public override void Run()
        {
            SyncWeChatUserGroup();
        }

        private void SyncWeChatUserGroup()
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

                var userGroupApi = new UserGroupApi();
                userGroupApi.SetKey(tenantId);

                var result = userGroupApi.Get();
                if (result.IsSuccess())
                {
                    ReportProgress(30, "已成功获取数据：" + result.Groups.Count);
                    using (var db = new AppDbContext())
                    {
                        TenantManager.Current.EnableTenantFilter(db, tenantId);
                        db.WeiChat_UserGroups.RemoveRange(
                db.WeiChat_UserGroups.Where(p => !db.WeiChat_Users.Any(p1 => p1.GroupIds.Any(d=>d==p.GroupId))));
                        db.SaveChanges();

                        var tenantGroups = db.WeiChat_UserGroups.ToList();
                        var groups = (from item in result.Groups
                                          //where !tenantGroups.Any(p => p.GroupIds == item.Id)
                                      where !tenantGroups.Any(p => p.GroupId==item.Id)
                                      select new WeiChat_UserGroup
                                      {
                                          //GroupIds = item.Id,
                                          Name = item.Name,
                                          UsersCount = item.Count,
                                          TenantId = tenantId
                                      }).ToList();
                        db.WeiChat_UserGroups.AddRange(groups);

                        var count = result.Groups == null ? 0 : result.Groups.Count;
                        ReportProgress(90, "正在写入数据,写入数量（" + count + "）...");
                        db.SaveChanges();
                        ReportProgress(100, "同步成功！同步数量（" + count + "）。");
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