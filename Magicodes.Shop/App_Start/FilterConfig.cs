using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Magicodes.Logger.NLog;
using Magicodes.Mvc.AccessFilter;
using Magicodes.Mvc.AuditFilter;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure;
using System.Data.Entity;

namespace Magicodes.Shop
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //将访问筛选器添加到全局筛选器中
            filters.Add(new AccessFilter());
        }

        public static void RegisterMagicodesFilter()
        {
            RoleMenuFilterBuilder
                //创建实例
                .Create()
                //设置包含的程序集（仅会在此程序集扫描）
                .WithContainAssemblyName("Magicodes.Shop")
                //基于此基类查找
                .WithControllerType(typeof(ControllerBase))
                //添加日志记录器
                .WithLogger(new NLogLogger("RoleMenuFilter"))
                //添加初始化方法
                .WithMenuInitialization((list) =>
                {
                    List<AppRole> roles;
                    int orderIndex = 1;
                    #region 移除已有的代码菜单
                    using (var db = new AppDbContext())
                    {
                        roles = db.Roles.ToList();
                        if (db.Site_Menus.Any())
                        {
                            var toRemove = db.Site_Menus.Where(p => p.IsCreateByCode).ToList();
                            //db.Role_Menus.RemoveRange(
                            //    db.Role_Menus.Where(p => db.Site_Menus.Any(p1 => (p1.Id == p.MenuId) && p1.IsCreateByCode)));
                            db.Site_Menus.RemoveRange(toRemove);
                            db.SaveChanges();
                            orderIndex = db.Site_Menus.Max(p => p.OrderNo) + 1;
                        }
                    }
                    #endregion
                    var siteMenus = new List<Site_Menu>();
                    var roleMenus = new List<Role_Menu>();

                    foreach (var roleMenuFilter in list)
                    {
                        #region 菜单数据
                        var siteMenu = new Site_Menu()
                        {
                            Action = roleMenuFilter.Action,
                            Controller = roleMenuFilter.Controller,
                            Title = roleMenuFilter.Title,
                            IconCls = roleMenuFilter.IconCls,
                            IsCreateByCode = true,
                            OrderNo = roleMenuFilter.OrderNo == null || roleMenuFilter.OrderNo == default(int) ? orderIndex : roleMenuFilter.Order,
                            Id = roleMenuFilter.Id,
                            ParentId = roleMenuFilter.ParentId,
                            Tag = string.IsNullOrEmpty(roleMenuFilter.Tag) ? "Tenant" : roleMenuFilter.Tag,
                            Url = roleMenuFilter.Url,
                            Path = roleMenuFilter.ParentId == null
                                                ? roleMenuFilter.Id.ToString("N")
                                                : string.Format("{0:N}-{1:N}", roleMenuFilter.ParentId.Value, roleMenuFilter.Id),
                        };
                        #endregion
                        siteMenus.Add(siteMenu);
                        #region 角色菜单绑定数据
                        foreach (var roleName in roleMenuFilter.RoleNames.Split(','))
                        {
                            var role = roles.FirstOrDefault(p => p.Name == roleName);
                            if (role == null) continue;
                            var roleMenu = new Role_Menu
                            {
                                MenuId = siteMenu.Id,
                                RoleId = role.Id
                            };
                            roleMenus.Add(roleMenu);
                        }
                        #endregion
                        orderIndex++;
                    }
                    using (var db = new AppDbContext())
                    {
                        db.Site_Menus.AddRange(siteMenus);

                        var currentRoleMenus = db.Role_Menus.ToList();
                        foreach (var item in roleMenus)
                        {
                            if (!currentRoleMenus.Any(p => p.RoleId == item.RoleId && p.MenuId == item.MenuId))
                            {
                                db.Role_Menus.Add(item);
                            }
                        }
                        db.SaveChanges();
                    }
                })
                //添加角色权限控制
                .WithRoleControl()
                //构造并启动
                .Build();

            //注册审计筛选器
            AuditFilterBuilder
                //创建Builder对象
                .Create()
                //设置审计数据处理函数
                .UsingAuditDataAction((filter, httpcontext) =>
                {
                    var logAudit = new Log_Audit()
                    {
                        BrowserInfo = filter.BrowserInfo,
                        ClientIpAddress = filter.ClientIpAddress,
                        ClientName = filter.ClientName,
                        Code = filter.Code,
                        CreateBy = WeiChatApplicationContext.Current.GetUserId(httpcontext),
                        CreateTime = DateTime.Now,
                        CustomData = (httpcontext.Items["CustomData"] ?? string.Empty).ToString(),
                        Exception = (filter.Exception == null ? null : filter.Exception.ToString()),
                        ExecutionDuration = filter.ExecutionDuration,
                        FormData = filter.ActionData,
                        IsSuccess = filter.Exception == null,
                        Remark = filter.Remark,
                        RequestUrl = filter.RequestUrl,
                        Title = filter.Title,
                        TenantId = WeiChatApplicationContext.Current.GetTenantId(httpcontext)
                    };
                    using (var db = new AppDbContext())
                    {
                        db.Log_Audits.Add(logAudit);
                        db.SaveChanges();
                    }
                })
                //构造执行（必须）
                .Build();

            //注册访问筛选器
            AccessFilterBuilder
                .Create()
                .WithExcludeUrlPrefixs("/Account")
                .UsingAccessDataAction((filter, httpcontext) =>
                {
                    var tenantId = WeiChatApplicationContext.Current.GetTenantId(httpcontext);
                    if (tenantId == default(int))
                    {
                        return;
                    }
                    var log = new Log_MemberAccess()
                    {
                        BrowserInfo = filter.BrowserInfo,
                        ClientIpAddress = filter.ClientIpAddress,
                        //CreateBy = WeiChatApplicationContext.Current.GetUserId(httpcontext),
                        CreateTime = DateTime.Now,
                        ExecutionDuration = filter.ExecutionDuration,
                        //FormData = filter.ActionData,
                        RequestUrl = filter.RequestUrl,
                        TenantId = tenantId,
                        OpenId = WeiChatApplicationContext.Current.GetOpenId(context: httpcontext, tenantId: tenantId)
                    };
                    using (var db = new AppDbContext())
                    {
                        db.Log_MemberAccess.Add(log);
                        db.SaveChanges();
                    }
                })
                //权限验证
                .OnAuthorization((filter, context) =>
                {
                    var httpContextBase = context.HttpContext;
                    var request = httpContextBase.Request;
                    var action = context.ActionDescriptor.ActionName;
                    var controller = context.ActionDescriptor.ControllerDescriptor.ControllerName;
                    var url = request.Url.AbsolutePath.ToString().ToLower();
                    

                    if (url.StartsWith("/api/")
                    || url.StartsWith("/app/")
                    || url.StartsWith("/account/login")
                    || controller == "Modules"
                    || controller == "Unity"
                    || controller == "WeiChat"
                    || controller == "WeiChat_KeyWordTextContent"
                    || url.StartsWith("/bmspay/wxpaynotify")
                    || url.StartsWith("/bmspay/alipaynotify")
                    || url.StartsWith("/account/register")
                    || url.StartsWith("/account/logoff")
                    || url.StartsWith("/product_attribute/createattribute")
                    || url.StartsWith("/product_style/createattribute")
                    || url.StartsWith("/systemadmin/login")
                    || url.StartsWith("/account/validatecode")
                    || url.StartsWith("/account/nopermission")
                    || url.StartsWith("/ueditor")
                    || url.StartsWith("/site_article/indexlist")
                    || url.StartsWith("/site_article/detailcontent")
                    )
                    {
                        return;
                    }
                    var userId = WeiChatApplicationContext.Current.GetUserId(httpContextBase);
                    if (string.IsNullOrEmpty(userId) && !url.StartsWith("/account/login"))
                    {
                        context.Result = new RedirectResult("/Account/Login");
                        return;
                    }
                    //var tenantId = WeiChatApplicationContext.Current.GetTenantId(httpContextBase);
                    
                    var siteMenus = httpContextBase.Session["Menus"] as List<Site_Menu>;
                    if (siteMenus == null)
                    {
                        using (var db = new AppDbContext())
                        {
                            var appUser = db.Users.Include(p => p.Roles).FirstOrDefault(p => p.Id == userId);
                            if (appUser != null)
                            {
                                var roles = appUser.Roles.Select(p => p.RoleId).ToArray();
                                var menus =
                                    db.Site_Menus.Where(
                                            p => db.Role_Menus.Any(p1 => (p1.MenuId == p.Id) && roles.Any(p2 => p2 == p1.RoleId)))
                                        .ToList().Distinct().ToList();
                                siteMenus = menus;
                                httpContextBase.Session["Menus"] = siteMenus;
                            }
                            else
                            {
                                context.Result = new RedirectResult("/Account/Login");
                                return;
                            }
                        }
                    }
                    //现在只判断控制器级别权限
                    if (!siteMenus.Any(p => p.Controller.Equals(controller, StringComparison.CurrentCultureIgnoreCase)))
                    {
                         //throw new Exception("您没权限访问该页面！");
                    }
                })
                .Build();
        }
    }
}