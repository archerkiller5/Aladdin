// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChatApplicationContext.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web;
using Magicodes.Logger;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Interface;
using Magicodes.WeiChat.Unity;
using Microsoft.AspNet.Identity;
using Magicodes.Storage;
using Magicodes.Notify;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.Tasks;
using System.Collections.Generic;

namespace Magicodes.WeiChat.Infrastructure
{
    /// <summary>
    ///     微信全局上下文对象
    /// </summary>
    public class WeiChatApplicationContext : ThreadSafeLazyBaseSingleton<WeiChatApplicationContext>
    {
        internal const string UserSessionName = "Magicodes.Weichat_User";
        internal const string TenantIdSessionName = "Magicodes.TenantId";
        internal const string WeiChatContextSessionName = "Magicodes.WeiChatContext";

        /// <summary>
        ///     OpenId的Cookie名称
        /// </summary>
        internal const string OpenIdCookieName = "Magicodes.Weichat_OpenId";
        /// <summary>
        /// 存储提供程序
        /// </summary>
        public IStorageProvider StorageProvider { get; set; }
        /// <summary>
        /// 通知管理程序
        /// </summary>
        public INotifier Notifier { get; set; }
        /// <summary>
        /// 任务管理器
        /// </summary>
        public TaskManager TaskManager { get; set; }

        /// <summary>
        ///     微信AppId
        /// </summary>
        public string AppId
        {
            get { return WeChatConfigManager.Current.GetConfig().AppId; }
        }

        /// <summary>
        ///     接口访问密钥
        /// </summary>
        public string AppSecret
        {
            get { return WeChatConfigManager.Current.GetConfig().AppSecret; }
        }

        public string UserId
        {
            get { return HttpContext.Current == null ? null : HttpContext.Current.User.Identity.GetUserId(); }
        }
        /// <summary>
        ///     租户Id（如果是系统租户则能获取到参数中的租户Id）
        /// </summary>
        public int TenantId
        {
            get
            {
                return GetTenantId(HttpContext.Current.Request.RequestContext.HttpContext);
            }
        }
        /// <summary>
        ///     租户信息
        /// </summary>
        public ITenant<int> TenantInfo
        {
            get
            {
                using (var db = new AppDbContext())
                {
                    return db.Admin_Tenants.Find(TenantId);
                }
            }
        }

        /// <summary>
        ///     获取微信用户信息
        /// </summary>
        public WeiChat_User WeiChatUser
        {
            get
            {
                var log = Loggers.Current.DefaultLogger;
                var tenantId = TenantId;
                var userSesstionName = string.Format("{0}_{1}", UserSessionName, tenantId);
                var cookieName = string.Format("{0}_{1}", OpenIdCookieName, tenantId);
                var usergroup = string.Format("groupname_{0}", tenantId);
                //从Session里获取
                if (HttpContext.Current.Session[userSesstionName] != null)
                {
                    log.Log(LoggerLevels.Trace, "Session is not Null");
                    return HttpContext.Current.Session[userSesstionName] as WeiChat_User;
                }
                //从PageItem中获取
                if (HttpContext.Current.Items[userSesstionName] != null)
                {
                    log.Log(LoggerLevels.Trace, "Items is not Null");
                    var user = HttpContext.Current.Items[userSesstionName] as WeiChat_User;
                    HttpContext.Current.Session[userSesstionName] = user;
                    return user;
                }
                //从Cookie中获取
                var openIdCookie = new HttpCookie(cookieName);
                if ((openIdCookie != null) && !string.IsNullOrEmpty(openIdCookie.Value))
                {
                    log.Log(LoggerLevels.Trace, "openIdCookie is not Null");
                    using (var db = new AppDbContext())
                    {
                        var user = db.WeiChat_Users.FirstOrDefault(p => p.OpenId == openIdCookie.Value);
                        HttpContext.Current.Session[userSesstionName] = user;
                        return user;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 获取OpenId
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public string GetOpenId(HttpContextBase context, int tenantId = 0)
        {
            if (tenantId == 0) tenantId = GetTenantId(context);
            var cookieName = string.Format("{0}_{1}", WeiChatApplicationContext.OpenIdCookieName, tenantId);
            var openIdCookie = context.Request.Cookies[cookieName];
            if (openIdCookie != null && !string.IsNullOrEmpty(openIdCookie.Value))
                return openIdCookie.Value;
            if (context.Request.RequestContext.RouteData.Values["OpenId"] != null)
                return context.Request.RequestContext.RouteData.Values["OpenId"].ToString();
            return null;
        }

        /// <summary>
        ///     获取租户Id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public int GetTenantId(HttpContextBase context)
        {
            //租户Id
            var tenantId = default(int);
            //请求参数中的租户Id
            var reqTennantId = default(int);

            #region 获取请求参数中的租户Id

            if (!string.IsNullOrWhiteSpace(context.Request.QueryString["TenantId"]))
                reqTennantId = Convert.ToInt32(context.Request.QueryString["TenantId"]);
            else
                reqTennantId = context.Request.RequestContext.RouteData.Values["TenantId"] != null
                    ? Convert.ToInt32(context.Request.RequestContext.RouteData.Values["TenantId"])
                    : default(int);

            #endregion

            //通过Session获取租户Id
            if (context.Session != null && context.Session[TenantIdSessionName] != null)
                tenantId = Convert.ToInt32(context.Session[TenantIdSessionName]);
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //代理租户Id
                var agentTennantId = default(int);
                if ((tenantId == default(int)) || ((tenantId != reqTennantId) && (reqTennantId != default(int))))
                {
                    using (var db = new AppDbContext())
                    {
                        var user = db.Users.Find(UserId);
                        //根据登录角色获取
                        tenantId = user.TenantId;
                        agentTennantId = user.AgentTennantId;

                        //如果当前租户为系统租户，则允许代理其他租户操作
                        if (db.Admin_Tenants.Any(p => (p.Id == tenantId) && p.IsSystemTenant))
                        {
                            //系统租户使用AgentTennantId
                            if ((user.AgentTennantId != default(int)) && (user.AgentTennantId != tenantId))
                                tenantId = user.AgentTennantId;

                            //如果当前租户Id与请求租户Id不一致
                            if ((tenantId != reqTennantId) && (reqTennantId != default(int)))
                            {
                                user.AgentTennantId = reqTennantId;
                                db.SaveChanges();
                                tenantId = reqTennantId;
                            }
                        }
                    }
                }
            }
            else if ((tenantId != reqTennantId) && (reqTennantId != default(int)))
            {
                tenantId = reqTennantId;
            }
            context.Session[TenantIdSessionName] = tenantId;
            return tenantId;
        }

        /// <summary>
        ///     获取用户Id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetUserId(HttpContextBase context)
        {
            return context == null ? null : context.User.Identity.GetUserId();
        }
    }
}