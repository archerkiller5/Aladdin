using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Magicodes.Notify;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeChat.SDK.Apis.User;
using Magicodes.Logger;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Infrastructure.Logging;
using Magicodes.Tasks;
using Hangfire;
using Magicodes.WeChat.SDK.Apis.CustomerService;
using Magicodes.WeiChat.Infrastructure.Tenant;

namespace Magicodes.Shop.App_Start.Tasks
{
    /// <summary>
    /// 多客服客服信息同步
    /// </summary>
    public class SyncMKFTask : TaskBase
    {
        public override string Title
        {
            get
            {
                return "多客服客服信息同步";
            }
        }

        public override void Run()
        {
            SyncMKF();
        }

        private void SyncMKF()
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

                var api = new CustomerServiceApi();
                api.SetKey(tenantId);

                var result = api.GetCustomerAccountList();
                if (result.IsSuccess())
                {
                    ReportProgress(30, "已成功获取数据：" + result.AccountList.Length);
                    using (var db = new AppDbContext())
                    {
                        TenantManager.Current.EnableTenantFilter(db, tenantId);
                        if ((result.AccountList != null) && (result.AccountList.Length != 0))
                        {
                            db.WeiChat_KFCInfos.RemoveRange(db.WeiChat_KFCInfos);
                            db.SaveChanges();

                            foreach (var item in result.AccountList)
                            {
                                var mkf = new WeiChat_KFCInfo
                                {
                                    JobNumber = item.JobNumber,
                                    NickName = item.NickName,
                                    HeadImgUrl = item.HeadUrl,
                                    Account = item.AccountName,
                                    TenantId = tenantId,
                                    CreateBy = userId,
                                    CreateTime = DateTime.Now
                                };
                                db.WeiChat_KFCInfos.Add(mkf);
                            }
                        }
                        else
                        {
                            db.WeiChat_KFCInfos.RemoveRange(db.WeiChat_KFCInfos);
                        }
                        var count = result.AccountList == null ? 0 : result.AccountList.Length;
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