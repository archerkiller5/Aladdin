// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : TaskManager.cs
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
using System.Linq;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Apis.TemplateMessage;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.WeiChat;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.WeChatHelper.Task
{
    /// <summary>
    ///     任务管理
    /// </summary>
    public class TaskManager : ThreadSafeLazyBaseSingleton<TaskManager>
    {
        public void StartAllTasks()
        {
            RegisterFuncs();
        }

        public void RegisterFuncs()
        {
            //注册Key。不再需要各个控制注册
            WeChatFrameworkFuncsManager.Current.Register(WeChatFrameworkFuncTypes.GetKey,
                model => WeiChatApplicationContext.Current.TenantId);

            //注册获取配置函数：根据Key获取微信配置（加载一次后续将缓存）
            WeChatFrameworkFuncsManager.Current.Register(WeChatFrameworkFuncTypes.Config_GetWeChatConfigByKey,
                model =>
                {
                    var arg = model as WeChatApiCallbackFuncArgInfo;
                    using (var _db = new AppDbContext())
                    {
                        var appConfig = _db.WeiChat_Apps.Find(arg.Data);
                        if (appConfig == null)
                            throw new Exception("您尚未配置公众号，请配置公众号信息！");
                        return appConfig;
                    }
                });
            //注册获取微信支付配置函数：根据Key获取微信支付配置（加载一次后续将缓存）
            WeChatFrameworkFuncsManager.Current.Register(WeChatFrameworkFuncTypes.Config_GetWeChatPayConfigByKey,
                model =>
                {
                    var arg = model as WeChatPayCallbackFuncArgInfo;
                    using (var _db = new AppDbContext())
                    {
                        var key = arg.Api == null ? null : arg.Api.GetKey();
                        if (key == null)
                            key = arg.Data;

                        var tenantId = key != null ? (int) key : WeiChatApplicationContext.Current.TenantId;
                        var appConfig = _db.WeiChat_Pays.FirstOrDefault(p => p.TenantId == tenantId);
                        if (appConfig == null)
                            throw new Exception("您尚未配置微信支付，请配置微信支付信息！");
                        return appConfig;
                    }
                });

            //模板消息发送:记录发送日志
            WeChatFrameworkFuncsManager.Current.Register(WeChatFrameworkFuncTypes.APIFunc_TemplateMessageApi_Create,
                model =>
                {
                    var arg = model as WeChatApiCallbackFuncArgInfo;
                    var messagesTemplateLogFuncModel = arg.Data as List<MessagesTemplateLogFuncModel>;
                    if ((messagesTemplateLogFuncModel == null) || (messagesTemplateLogFuncModel.Count == 0))
                        return true;
                    var key = arg.Api == null ? null : arg.Api.GetKey();
                    var tenantId = key != null ? (int) key : WeiChatApplicationContext.Current.TenantId;
                    var userId = WeiChatApplicationContext.Current.UserId;
                    var messageTplNo = messagesTemplateLogFuncModel.First().MessagesTemplateNo;
                    using (var _db = new AppDbContext())
                    {
                        var messageTemplate =
                            _db.WeiChat_MessagesTemplates.FirstOrDefault(
                                p => (p.TenantId == tenantId) && (p.TemplateNo == messageTplNo));
                        if (messageTemplate == null)
                            throw new Exception(
                                string.Format(
                                    "模板库中不存在此模板消息，请先维护。模板Id：{0}。模板库地址：https://mp.weixin.qq.com/advanced/tmplmsg?action=tmpl_store&t=tmplmsg/store&token=1241910100&lang=zh_CN",
                                    messageTplNo));
                        var logs = messagesTemplateLogFuncModel.Select(p => new WeiChat_MessagesTemplateSendLog
                        {
                            CreateTime = p.CreateTime,
                            BatchNumber = p.BatchNumber,
                            Content = p.Content,
                            MessagesTemplateNo = p.MessagesTemplateNo,
                            MessagesTemplateId = messageTemplate.Id,
                            ReceiverId = p.ReceiverId,
                            TopColor = p.TopColor,
                            Url = p.Url,
                            TenantId = tenantId,
                            CreateBy = userId,
                            Result = p.Result
                        }).ToList();
                        _db.WeiChat_MessagesTemplateSendLogs.AddRange(logs);
                        _db.SaveChanges();
                    }
                    return true;
                });
        }
    }
}