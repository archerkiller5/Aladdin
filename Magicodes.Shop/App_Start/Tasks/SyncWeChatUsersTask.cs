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
using Magicodes.WeiChat.Infrastructure.Tenant;

namespace Magicodes.Shop.App_Start.Tasks
{
    /// <summary>
    /// 粉丝同步
    /// </summary>
    public class SyncWeChatUsersTask : TaskBase
    {
        public override string Title
        {
            get
            {
                return "粉丝信息同步";
            }
        }

        public override void Run()
        {
            SyncWeChatUsers();
        }

        private void SyncWeChatUsers()
        {
            var notify = NotifyInfo;
            var notifier = Notifier;
            var strs = Parameter.ToString().Split(';');
            var tenantId = int.Parse(strs[0]);
            var logger = Logger;

            try
            {
                ReportProgress(1, "正在初始化同步信息");

                var userApi = new UserApi();
                userApi.SetKey(tenantId);
                var opendIds = new List<string>();
                //递归获取所有的OPENID
                GetOpenIds(opendIds, userApi);
                if (opendIds.Count == 0)
                    return;

                ReportProgress(5, "正在准备获取 " + opendIds.Count + " 个粉丝...");

                var distinctOpendIds = opendIds.Distinct().ToList();
                if (distinctOpendIds.Count != opendIds.Count)
                {
                    opendIds = distinctOpendIds;
                }
                var userInfoList = new List<UserBatchGetApiResult.UserInfo>();
                while (opendIds.Count > 0)
                {
                    var taskCount = opendIds.Count > 10000 ? 100 : opendIds.Count / 100 + 1;
                    var successList = new List<string>();
                    GetUserInfoList(userApi, opendIds, userInfoList, taskCount, successList);

                    var hs = new HashSet<string>(opendIds);
                    var successhs = new HashSet<string>(successList);
                    hs.RemoveWhere(p => successhs.Contains(p));
                    opendIds = hs.ToList();

                    ReportProgress(10, "已获取 " + successList.Count + " 个粉丝...");
                    logger.Log(LoggerLevels.Debug, "successList:【" + string.Join(",", successList) + "】。");
                }
                logger.Log(LoggerLevels.Debug, "获取粉丝数据完成。数量：" + userInfoList.Count);
                ReportProgress(40, "获取粉丝数据完成（" + opendIds.Count + "），正在更新数据...");

                using (var db = new AppDbContext())
                {
                    TenantManager.Current.EnableTenantFilter(db, tenantId);
                    var users = db.WeiChat_Users.ToList();

                    var userList = userInfoList.Select(userInfo => new WeiChat_User
                    {
                        City = userInfo.City,
                        Country = userInfo.Country,
                        GroupId = userInfo.GroupId,
                        HeadImgUrl = userInfo.Headimgurl,
                        Language = userInfo.Language,
                        NickName = userInfo.NickName,
                        OpenId = userInfo.OpenId,
                        Province = userInfo.Province,
                        Remark = userInfo.Remark,
                        Sex = userInfo.Sex,
                        Subscribe = userInfo.Subscribe,
                        SubscribeTime =
                            userInfo.SubscribeTime == default(DateTime)
                                ? DateTime.Parse("2011-05-10")
                                : userInfo.SubscribeTime,
                        UnionId = userInfo.Unionid,
                        TenantId = tenantId
                    }).ToList();
                    foreach (var item in userList)
                    {
                        var weChatUser = users.FirstOrDefault(p => p.OpenId == item.OpenId);
                        if (weChatUser != null)
                        {
                            weChatUser.City = item.City;
                            weChatUser.Country = item.Country;
                            weChatUser.GroupId = item.GroupId;
                            weChatUser.HeadImgUrl = item.HeadImgUrl;
                            weChatUser.Language = item.Language;
                            weChatUser.NickName = item.NickName;
                            weChatUser.OpenId = item.OpenId;
                            weChatUser.Province = item.Province;
                            weChatUser.Remark = item.Remark;
                            weChatUser.Sex = item.Sex;
                            weChatUser.Subscribe = item.Subscribe;
                            weChatUser.SubscribeTime = item.SubscribeTime;
                            weChatUser.UnionId = item.UnionId;
                        }
                        else
                            db.WeiChat_Users.Add(item);
                    }

                    ReportProgress(90, "正在写入数据,写入数量（" + userList.Count + "）...");
                    db.SaveChanges();
                    ReportProgress(100, "同步成功！同步数量（" + userList.Count + "）。");
                }

            }
            catch (Exception ex)
            {
                logger.Log(LoggerLevels.Debug, "粉丝信息同步失败。");
                logger.LogException(ex);

                ReportProgress(100, "同步失败！" + ex.Message);
                return;
            }
        }

        private void GetOpenIds(List<string> opendIds, UserApi userApi, string nextOpenId = null)
        {
            var result = userApi.GetOpenIdList(nextOpenId);
            if (result.IsSuccess() && (result.Data != null))
                opendIds.AddRange(result.Data.OpenIds);
            //最多一次只能获取10000
            if (!string.IsNullOrEmpty(result.NextOpenId) && (result.Count == 10000))
                GetOpenIds(opendIds, userApi, result.NextOpenId);
        }

        private void GetUserInfoList(UserApi userApi, List<string> opendIds,
    List<UserBatchGetApiResult.UserInfo> userInfoList, int count, List<string> successList)
        {
            var taskList = new List<System.Threading.Tasks.Task>();
            for (var i = 0; i < count; i++)
                lock (opendIds)
                {
                    var takeCount = opendIds.Count > 100 ? 100 : opendIds.Count;
                    var openIdsToGet = opendIds.Skip(i * 100).Take(takeCount).ToArray();
                    if (openIdsToGet.Count() > 0)
                    {
                        var task = new System.Threading.Tasks.Task(() =>
                        {
                            var debugStr = "";
                            //该接口最多支持获取100个粉丝的信息
                            try
                            {
                                debugStr = "准备获取以下粉丝信息：" + string.Join(",", openIdsToGet) + "。" + Environment.NewLine;

                                var batchResult = userApi.Get(openIdsToGet);
                                if (batchResult.IsSuccess())
                                {
                                    debugStr += "已成功获取粉丝信息。";
                                    lock (userInfoList)
                                    {
                                        userInfoList.AddRange(batchResult.UserInfoList);
                                    }
                                    lock (successList)
                                    {
                                        successList.AddRange(openIdsToGet);
                                    }
                                }
                                else
                                {
                                    debugStr += "粉丝信息获取失败：" + batchResult.DetailResult + "。";
                                }
                            }
                            catch (Exception ex)
                            {
                                debugStr += "粉丝信息获取异常：" + ex + "。";
                            }
                            finally
                            {
                                Logger.Log(LoggerLevels.Debug, debugStr);
                            }
                        });
                        task.Start();
                        taskList.Add(task);
                    }
                }
            if (taskList.Count > 0)
                System.Threading.Tasks.Task.WaitAll(taskList.ToArray());
            Logger.Log(LoggerLevels.Debug, "已处理完" + taskList.Count + "个任务。");
            taskList.Clear();
        }
    }
}