// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SyncHelper.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:32
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magicodes.Logger;
using Magicodes.WeChat.SDK.Apis.CustomerService;
using Magicodes.WeChat.SDK.Apis.TemplateMessage;
using Magicodes.WeChat.SDK.Apis.User;
using Magicodes.WeChat.SDK.Apis.UserGroup;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.BatchOperation;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.Logging;
using Magicodes.WeiChat.Infrastructure.Tenant;

namespace Magicodes.WeiChat.WeChatHelper.Task
{
    public class SyncHelper
    {
        private readonly AppDbContext _db;
        private readonly int _tenantId;
        //Logger logger = LogManager.GetCurrentClassLogger();
        private readonly LoggerBase logger = Loggers.Current.DefaultLogger;

        public SyncHelper(int tenantId)
        {
            _db = new AppDbContext();
            TenantManager.Current.EnableTenantFilter(_db, tenantId);
            _tenantId = tenantId;
        }

        /// <summary>
        ///     触发同步
        /// </summary>
        /// <param name="syncType">同步类型</param>
        /// <param name="isUserSync">是否手动触发，为true必定触发全量同步，false会根据时间选择性触发</param>
        /// <param name="createBy">创建人</param>
        /// <returns>是否同步成功</returns>
        public async Task<bool> Sync(WeiChat_SyncTypes syncType, bool isUserSync = false, string createBy = null)
        {
            if (!isUserSync)
            {
                //默认间距6小时以上才进行同步
                var lastTime = DateTime.Now.AddHours(6);
                if (_db.WeiChat_SyncLogs.Any(p => (p.Type == syncType) && (p.CreateTime < lastTime)))
                    return await System.Threading.Tasks.Task.FromResult(false);
            }
            //TODO:封装通用的同步方法
            switch (syncType)
            {
                case WeiChat_SyncTypes.Sync_WeiChat_User:
                    await SyncWeChatUsers(isUserSync, createBy);
                    break;
                case WeiChat_SyncTypes.Sync_MKF:
                    await SyncWeChatMKF(isUserSync, createBy);
                    break;
                case WeiChat_SyncTypes.Sync_Images:
                    break;
                case WeiChat_SyncTypes.Sync_WeiChat_UserGroup:
                    await SyncWeChatGroups(isUserSync, createBy);
                    break;
                case WeiChat_SyncTypes.Sync_MessagesTemplates:
                    await SyncTemplateMessages(isUserSync, createBy);
                    break;
                default:
                    break;
            }
            return await System.Threading.Tasks.Task.FromResult(true);
        }

        /// <summary>
        ///     同步多客服账号
        /// </summary>
        private async Task<bool> SyncWeChatMKF(bool isUserSync = false, string createBy = null)
        {
            var api = new CustomerServiceApi();
            api.SetKey(_tenantId);

            var result = api.GetCustomerAccountList();
            if (result.IsSuccess())
                if ((result.AccountList != null) && (result.AccountList.Length != 0))
                {
                    _db.WeiChat_KFCInfos.RemoveRange(_db.WeiChat_KFCInfos);
                    _db.SaveChanges();

                    foreach (var item in result.AccountList)
                    {
                        var mkf = _db.WeiChat_KFCInfos.FirstOrDefault(p => p.Account == item.AccountName);
                        if (mkf == null)
                        {
                            mkf = new WeiChat_KFCInfo
                            {
                                JobNumber = item.JobNumber,
                                NickName = item.NickName,
                                HeadImgUrl = item.HeadUrl,
                                Account = item.AccountName,
                                TenantId = _tenantId,
                                CreateBy = createBy,
                                CreateTime = DateTime.Now
                            };
                            _db.WeiChat_KFCInfos.Add(mkf);
                        }
                        else
                        {
                            mkf.JobNumber = item.JobNumber;
                            mkf.HeadImgUrl = item.HeadUrl;
                            mkf.NickName = item.NickName;
                            mkf.UpdateBy = createBy;
                            mkf.UpdateTime = DateTime.Now;
                        }
                    }
                }
                else
                {
                    _db.WeiChat_KFCInfos.RemoveRange(_db.WeiChat_KFCInfos);
                }
            var log = new WeiChat_SyncLog
            {
                Type = WeiChat_SyncTypes.Sync_MKF,
                IsUserSync = isUserSync,
                TenantId = _tenantId,
                CreateBy = createBy,
                Message =
                    string.Format("客服账号同步{1}！同步数量：{0}。", result.AccountList == null ? 0 : result.AccountList.Length,
                        result.IsSuccess() ? "成功" : "失败(" + result.DetailResult + ")")
            };
            _db.WeiChat_SyncLogs.Add(log);
            await _db.SaveChangesAsync();
            return await System.Threading.Tasks.Task.FromResult(true);
        }

        /// <summary>
        ///     同步粉丝组
        /// </summary>
        private async Task<bool> SyncWeChatGroups(bool isUserSync = false, string createBy = null)
        {
            var userGroupApi = new UserGroupApi();
            userGroupApi.SetKey(_tenantId);

            var getResult = userGroupApi.Get();
            if (!getResult.IsSuccess()) return await System.Threading.Tasks.Task.FromResult(false);

            _db.WeiChat_UserGroups.RemoveRange(
                _db.WeiChat_UserGroups.Where(p => !_db.WeiChat_Users.Any(p1 => p1.GroupId == p.GroupId)));
            await _db.SaveChangesAsync();

            var tenantGroups = _db.WeiChat_UserGroups.ToList();
            var groups = (from item in getResult.Groups
                where !tenantGroups.Any(p => p.GroupId == item.Id)
                select new WeiChat_UserGroup
                {
                    GroupId = item.Id,
                    Name = item.Name,
                    UsersCount = item.Count,
                    TenantId = _tenantId
                }).ToList();
            _db.WeiChat_UserGroups.AddRange(groups);
            var log = new WeiChat_SyncLog
            {
                Type = WeiChat_SyncTypes.Sync_WeiChat_UserGroup,
                IsUserSync = isUserSync,
                TenantId = _tenantId,
                CreateBy = createBy,
                Message = string.Format("同步成功！同步组数：{0}。", groups.Count)
            };
            _db.WeiChat_SyncLogs.Add(log);
            await _db.SaveChangesAsync();
            return await System.Threading.Tasks.Task.FromResult(true);
        }

        /// <summary>
        ///     同步微信用户
        /// </summary>
        private async Task<bool> SyncWeChatUsers(bool isUserSync = false, string createBy = null)
        {
            try
            {
                _db.Database.ExecuteSqlCommand("Delete from MWC.WeiChat_User where TenantId={0}", _tenantId);
                logger.Log(LoggerLevels.Debug, "已清除租户【" + _tenantId + "】的粉丝信息。");
                //TODO:暂时不支持批量移除
                ////批量移除
                //_db.BathRemoveBy(_db.WeiChat_Users, p => p.TenantId == _tenantId);
                var userApi = new UserApi();
                userApi.SetKey(_tenantId);
                var opendIds = new List<string>();
                //递归获取所有的OPENID
                GetOpenIds(opendIds, userApi);
                if (opendIds.Count == 0)
                    return await System.Threading.Tasks.Task.FromResult(false);
                logger.Log(LoggerLevels.Debug, "待同步粉丝数：" + opendIds.Count);
                var distinctOpendIds = opendIds.Distinct().ToList();
                if (distinctOpendIds.Count != opendIds.Count)
                {
                    logger.Log(LoggerLevels.Debug, "粉丝数存在重复项。去重后粉丝数：" + distinctOpendIds.Count);
                    opendIds = distinctOpendIds;
                }
                var userInfoList = new List<UserBatchGetApiResult.UserInfo>();
                while (opendIds.Count > 0)
                {
                    var taskCount = opendIds.Count > 10000 ? 100 : opendIds.Count/100 + 1;
                    var successList = new List<string>();
                    GetUserInfoList(userApi, opendIds, userInfoList, taskCount, successList);

                    //opendIds.RemoveAll(p => successList.Any(p1 => p1 == p));

                    var hs = new HashSet<string>(opendIds);
                    var successhs = new HashSet<string>(successList);
                    hs.RemoveWhere(p => successhs.Contains(p));
                    opendIds = hs.ToList();

                    logger.Log(LoggerLevels.Debug, "successList:【" + string.Join(",", successList) + "】。");
                }
                logger.Log(LoggerLevels.Debug, "获取粉丝数据完成。数量：" + userInfoList.Count);

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
                    TenantId = _tenantId
                }).ToList();
                logger.Log(LoggerLevels.Debug, "待插入的粉丝数:【" + string.Join(",", userList.Count) + "】。");
                //TODO:插入之前判断是否在处理期间有用户关注，如果有，则进行处理
                //批量插入
                _db.BathInsert(_db.WeiChat_Users, userList);
                logger.Log(LoggerLevels.Debug, "批量插入已完成。");

                var log = new WeiChat_SyncLog
                {
                    Type = WeiChat_SyncTypes.Sync_WeiChat_User,
                    IsUserSync = isUserSync,
                    TenantId = _tenantId,
                    CreateBy = createBy,
                    Success = true,
                    Message = string.Format("同步成功！同步数量：{0}。", userList.Count)
                };
                _db.WeiChat_SyncLogs.Add(log);
                await _db.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                var validationException = ex;
                sb.AppendLine("实体验证错误。错误数：" + validationException.EntityValidationErrors.Count() + " ");
                foreach (var validationResult in validationException.EntityValidationErrors)
                    foreach (var item in validationResult.ValidationErrors)
                        sb.AppendFormat("（{0}:{1}）", item.PropertyName, item.ErrorMessage);
                logger.Log(LoggerLevels.Error, sb.ToString());
                var log = new WeiChat_SyncLog
                {
                    Type = WeiChat_SyncTypes.Sync_WeiChat_User,
                    IsUserSync = isUserSync,
                    TenantId = _tenantId,
                    CreateBy = createBy,
                    Message = string.Format("同步失败！具体异常：{0}。", sb),
                    Success = false
                };
                _db.WeiChat_SyncLogs.Add(log);
                //为了兼容VS2013编译器语法
                //await _db.SaveChangesAsync();
                //return await System.Threading.Tasks.Task.FromResult(false);
                _db.SaveChanges();
                return false;
            }
            catch (Exception ex)
            {
                logger.Log(LoggerLevels.Debug, "粉丝信息同步失败。");
                logger.LogException(ex);
                var log = new WeiChat_SyncLog
                {
                    Type = WeiChat_SyncTypes.Sync_WeiChat_User,
                    IsUserSync = isUserSync,
                    TenantId = _tenantId,
                    CreateBy = createBy,
                    Message = string.Format("同步失败！具体异常：{0}。", ex),
                    Success = false
                };
                _db.WeiChat_SyncLogs.Add(log);
                //为了兼容VS2013编译器语法
                //await _db.SaveChangesAsync();
                //return await System.Threading.Tasks.Task.FromResult(false);
                _db.SaveChanges();
                return false;
            }
            return await System.Threading.Tasks.Task.FromResult(true);
        }

        private void GetUserInfoList(UserApi userApi, List<string> opendIds,
            List<UserBatchGetApiResult.UserInfo> userInfoList, int count, List<string> successList)
        {
            var taskList = new List<System.Threading.Tasks.Task>();
            logger.Log(LoggerLevels.Debug, "同步粉丝任务数为" + count + "。");
            for (var i = 0; i < count; i++)
                lock (opendIds)
                {
                    var takeCount = opendIds.Count > 100 ? 100 : opendIds.Count;
                    var openIdsToGet = opendIds.Skip(i*100).Take(takeCount).ToArray();
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
                                logger.Log(LoggerLevels.Debug, debugStr);
                            }
                        });
                        task.Start();
                        taskList.Add(task);
                    }
                }
            if (taskList.Count > 0)
                System.Threading.Tasks.Task.WaitAll(taskList.ToArray());
            logger.Log(LoggerLevels.Debug, "已处理完" + taskList.Count + "个任务。");
            taskList.Clear();
        }


        private async Task<bool> SyncTemplateMessages(bool isUserSync = false, string createBy = null)
        {
            var templateMessageApi = new TemplateMessageApi();
            templateMessageApi.SetKey(_tenantId);

            var result = templateMessageApi.Get();
            if (result.IsSuccess())
                if ((result.Templates != null) && (result.Templates.Length != 0))
                    foreach (var item in result.Templates)
                    {
                        var mst = _db.WeiChat_MessagesTemplates.FirstOrDefault(p => p.TemplateNo == item.TemplateId);
                        if (mst == null)
                        {
                            mst = new WeiChat_MessagesTemplate
                            {
                                Demo = item.Example,
                                OneIndustry = item.PrimaryIndustry,
                                TemplateNo = item.TemplateId,
                                Content = item.Content,
                                TenantId = _tenantId,
                                Title = item.Title,
                                TwoIndustry = item.DeputyIndustry,
                                CreateBy = createBy,
                                CreateTime = DateTime.Now
                            };
                            _db.WeiChat_MessagesTemplates.Add(mst);
                        }
                        else
                        {
                            mst.Demo = item.Example;
                            mst.OneIndustry = item.PrimaryIndustry;
                            mst.Content = item.Content;
                            mst.TenantId = _tenantId;
                            mst.Title = item.Title;
                            mst.TwoIndustry = item.DeputyIndustry;
                            mst.UpdateBy = createBy;
                            mst.UpdateTime = DateTime.Now;
                        }
                    }
                else
                    _db.WeiChat_MessagesTemplates.RemoveRange(_db.WeiChat_MessagesTemplates);
            var log = new WeiChat_SyncLog
            {
                Type = WeiChat_SyncTypes.Sync_MessagesTemplates,
                IsUserSync = isUserSync,
                TenantId = _tenantId,
                CreateBy = createBy,
                Message =
                    string.Format("模板消息同步{1}！同步数量：{0}。", result.Templates == null ? 0 : result.Templates.Length,
                        result.IsSuccess() ? "成功" : "失败(" + result.DetailResult + ")")
            };
            _db.WeiChat_SyncLogs.Add(log);
            await _db.SaveChangesAsync();
            return await System.Threading.Tasks.Task.FromResult(true);
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
    }
}