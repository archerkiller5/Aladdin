// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Configuration.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.PhotoGallery;
using Magicodes.WeiChat.Data.Models.Site;
using Microsoft.AspNet.Identity;
using Magicodes.WeiChat.Data.Models.Product;
using Magicodes.WeiChat.Data.Models.Advert;
using System.Data.Entity;
using Magicodes.WeiChat.Data.Models.WeChatStore;

namespace Magicodes.WeiChat.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            ContextKey = "Magicodes.WeiChat.Models.ApplicationDbContext";
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ////关闭自动生成迁移（让程序只打我们自己生成的迁移）
            //AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppDbContext context)
        {
            var tenant = context.Admin_Tenants.FirstOrDefault(p => p.IsSystemTenant);
            if (tenant == null)
            {
                tenant = new Admin_Tenant { IsSystemTenant = true, Name = "系统租户", Remark = "系统租户，勿删！" };
                context.Admin_Tenants.Add(tenant);
                context.SaveChanges();
            }

            #region 添加用户和角色

            //全局管理员多租户Id为1
            var store = new AppUserStore(context) { TenantId = tenant.Id };
            var userManager = new UserManager<AppUser, string>(store);
            var roleManager = new RoleManager<AppRole>(new AppRoleStore(context));
            //系统管理员
            var adminRole = new AppRole
            {
                Id = "{74ABBD8D-ED32-4C3A-9B2A-EB134BFF5D91}",
                Name = "Admin",
                Description = "超级管理员，拥有最大权限"
            };
            if (!roleManager.RoleExists(adminRole.Name))
                roleManager.Create(adminRole);
            //租户管理员
            var tenantManager = new AppRole
            {
                Id = "{EB715DD5-1EEF-4131-B6B1-0DAE2FAA8861}",
                Name = "TenantManager",
                Description = "租户管理员"
            };
            if (!roleManager.RoleExists(tenantManager.Name))
                roleManager.Create(tenantManager);
            //店铺管理员
            var shopManager = new AppRole
            {
                Id = "{80BAB54E-856E-4DBE-85E1-A5F819BF9622}",
                Name = "ShopManager",
                Description = "店铺管理员"
            };
            if (!roleManager.RoleExists(shopManager.Name))
                roleManager.Create(shopManager);
            //店铺小二
            var shopWater = new AppRole
            {
                Id = "{F77E3025-026F-44DA-8565-18B1018E88F8}",
                Name = "ShopWater",
                Description = "店铺小二"
            };
            if (!roleManager.RoleExists(shopWater.Name))
                roleManager.Create(shopWater);

            var user = new AppUser
            {
                Id = "{B0FBB2AC-3174-4E5A-B772-98CF776BD4B9}",
                UserName = "admin",
                Email = "liwq@magicodes.net",
                EmailConfirmed = true,
                TenantId = tenant.Id
            };
            if (!userManager.Users.Any(p => p.Id == user.Id))
            {
                var result = userManager.Create(user, "123456abcD");
                if (result.Succeeded)
                    userManager.AddToRole(user.Id, adminRole.Name);
            }

            #endregion

            #region 初始化菜单

            if (!context.Site_Menus.Any())
            {
                var menus = new List<Site_Menu>();
                var menuWeChat = MenuDataSeedHelper.CreateRootMenu("微信功能管理", "", "", "", "fa fa-weixin");
                menus.Add(menuWeChat);

                #region 自定义菜单

                var menu1 = MenuDataSeedHelper.CreateChildMenu(menuWeChat.Id, "菜单管理", "", "", "", "fa fa-th-large");
                menus.Add(menu1);
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu1.Id, "自定义菜单", "Index", "Menus", "/Menus", ""));

                #endregion

                #region 粉丝管理

                var menu2 = MenuDataSeedHelper.CreateChildMenu(menuWeChat.Id, "粉丝管理", "", "", "", "fa fa-users");
                menus.Add(menu2);
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu2.Id, "粉丝管理", "Index", "WeiChat_User", "/WeiChat_User",
                    ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu2.Id, "用户组管理", "Index", "WeiChat_UserGroup",
                    "/WeiChat_UserGroup", ""));

                #endregion

                #region 素材管理

                var menu3 = MenuDataSeedHelper.CreateChildMenu(menuWeChat.Id, "素材管理", "", "", "", "fa fa-file-image-o");
                menus.Add(menu3);
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu3.Id, "图片管理", "Index", "Site_Resources",
                    "/Site_Resources?resourceType=0", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu3.Id, "语音管理", "Index", "Site_Resources",
                    "/Site_Resources?resourceType=1", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu3.Id, "视频管理", "Index", "Site_Resources",
                    "/Site_Resources?resourceType=2", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu3.Id, "图文消息管理", "Index", "Site_News", "/Site_News", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu3.Id, "文章管理", "Index", "Site_Article", "/Site_Article",
                    ""));

                #endregion

                #region 消息管理

                var menu4 = MenuDataSeedHelper.CreateChildMenu(menuWeChat.Id, "消息管理", "", "", "", "fa  fa-comments-o");
                menus.Add(menu4);
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu4.Id, "文本推送", "Send", "GroupMessage",
                    "/GroupMessage/Send?type=0", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu4.Id, "图片推送", "Send", "GroupMessage",
                    "/GroupMessage/Send?type=1", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu4.Id, "语音推送", "Send", "GroupMessage",
                    "/GroupMessage/Send?type=3", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu4.Id, "视频推送", "Send", "GroupMessage",
                    "/GroupMessage/Send?type=4", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu4.Id, "图文推送", "Send", "GroupMessage",
                    "/GroupMessage/Send?type=5", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu4.Id, "模板消息", "Index", "WeiChat_MessagesTemplate",
                    "/WeiChat_MessagesTemplate", ""));

                #endregion

                #region 场景二维码

                var menu5 = MenuDataSeedHelper.CreateChildMenu(menuWeChat.Id, "场景二维码", "", "", "", "fa fa-qrcode");
                menus.Add(menu5);
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu5.Id, "二维码管理", "Index", "WeiChat_QRCode",
                    "/WeiChat_QRCode", ""));

                #endregion

                #region 客服管理

                var menu6 = MenuDataSeedHelper.CreateChildMenu(menuWeChat.Id, "客服管理", "", "", "", "fa fa-users");
                menus.Add(menu6);
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu6.Id, "客服管理", "Index", "WeiChat_KFCInfo",
                    "/WeiChat_KFCInfo", ""));

                #endregion

                #region 智能回复

                var menu7 = MenuDataSeedHelper.CreateChildMenu(menuWeChat.Id, "智能回复", "", "", "", "fa fa-slack");
                menus.Add(menu7);
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu7.Id, "关键字回复", "Index", "WeiChat_KeyWordAutoReply",
                    "/WeiChat_KeyWordAutoReply", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu7.Id, "关注时回复", "Index", "WeiChat_SubscribeReply",
                    "/WeiChat_SubscribeReply", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu7.Id, "答不上来配置", "Index", "WeiChat_NotAnswerReply",
                    "/WeiChat_NotAnswerReply", ""));

                #endregion

                #region 数据与统计

                var menu8 = MenuDataSeedHelper.CreateChildMenu(menuWeChat.Id, "数据与统计", "", "", "", "fa fa-line-chart");
                menus.Add(menu8);
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu8.Id, "关键字回复统计", "Index", "WeiChat_KeyWordReplyLog",
                    "/WeiChat_KeyWordReplyLog", ""));
                menus.Add(MenuDataSeedHelper.CreateChildMenu(menu8.Id, "位置统计", "Index", "WeiChat_LocationEventLog",
                    "/WeiChat_LocationEventLog", ""));

                #endregion

                //var menu1 = MenuDataSeedHelper.CreateRootMenu(title: "菜单管理", action: "", controller: "", url: "", iconCls: "fa fa-th-large");
                //menus.Add(menu1);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu1.Id, title: "自定义菜单", action: "Index", controller: "Menus", url: "/Menus", iconCls: ""));

                //var menu2 = MenuDataSeedHelper.CreateRootMenu(title: "粉丝管理", action: "", controller: "", url: "", iconCls: "fa fa-users");
                //menus.Add(menu2);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu2.Id, title: "粉丝管理", action: "Index", controller: "WeiChat_User", url: "/WeiChat_User", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu2.Id, title: "用户组管理", action: "Index", controller: "WeiChat_UserGroup", url: "/WeiChat_UserGroup", iconCls: ""));

                //var menu3 = MenuDataSeedHelper.CreateRootMenu(title: "素材管理", action: "", controller: "", url: "", iconCls: "fa fa-file-image-o");
                //menus.Add(menu3);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu3.Id, title: "图片管理", action: "Index", controller: "Site_Resources", url: "/Site_Resources?resourceType=0", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu3.Id, title: "语音管理", action: "Index", controller: "Site_Resources", url: "/Site_Resources?resourceType=1", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu3.Id, title: "视频管理", action: "Index", controller: "Site_Resources", url: "/Site_Resources?resourceType=2", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu3.Id, title: "图文消息管理", action: "Index", controller: "Site_News", url: "/Site_News", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu3.Id, title: "文章管理", action: "Index", controller: "Site_Article", url: "/Site_Article", iconCls: ""));

                //var menu4 = MenuDataSeedHelper.CreateRootMenu(title: "消息管理", action: "", controller: "", url: "", iconCls: "fa  fa-comments-o");
                //menus.Add(menu4);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu4.Id, title: "文本推送", action: "Send", controller: "GroupMessage", url: "/GroupMessage/Send?type=0", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu4.Id, title: "图片推送", action: "Send", controller: "GroupMessage", url: "/GroupMessage/Send?type=1", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu4.Id, title: "语音推送", action: "Send", controller: "GroupMessage", url: "/GroupMessage/Send?type=3", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu4.Id, title: "视频推送", action: "Send", controller: "GroupMessage", url: "/GroupMessage/Send?type=4", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu4.Id, title: "图文推送", action: "Send", controller: "GroupMessage", url: "/GroupMessage/Send?type=5", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu4.Id, title: "模板消息", action: "Index", controller: "WeiChat_MessagesTemplate", url: "/WeiChat_MessagesTemplate", iconCls: ""));

                //var menu5 = MenuDataSeedHelper.CreateRootMenu(title: "场景二维码", action: "", controller: "", url: "", iconCls: "fa fa-qrcode");
                //menus.Add(menu5);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu5.Id, title: "二维码管理", action: "Index", controller: "WeiChat_QRCode", url: "/WeiChat_QRCode", iconCls: ""));

                //var menu6 = MenuDataSeedHelper.CreateRootMenu(title: "客服管理", action: "", controller: "", url: "", iconCls: "fa fa-users");
                //menus.Add(menu6);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu6.Id, title: "客服管理", action: "Index", controller: "WeiChat_KFCInfo", url: "/WeiChat_KFCInfo", iconCls: ""));

                //var menu7 = MenuDataSeedHelper.CreateRootMenu(title: "智能回复", action: "", controller: "", url: "", iconCls: "fa fa-slack");
                //menus.Add(menu7);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu7.Id, title: "关键字回复", action: "Index", controller: "WeiChat_KeyWordAutoReply", url: "/WeiChat_KeyWordAutoReply", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu7.Id, title: "关注时回复", action: "Index", controller: "WeiChat_SubscribeReply", url: "/WeiChat_SubscribeReply", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu7.Id, title: "答不上来配置", action: "Index", controller: "WeiChat_NotAnswerReply", url: "/WeiChat_NotAnswerReply", iconCls: ""));

                //var menu8 = MenuDataSeedHelper.CreateRootMenu(title: "数据与统计", action: "", controller: "", url: "", iconCls: "fa fa-line-chart");
                //menus.Add(menu8);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu8.Id, title: "关键字回复统计", action: "Index", controller: "WeiChat_KeyWordReplyLog", url: "/WeiChat_KeyWordReplyLog", iconCls: ""));
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu8.Id, title: "位置统计", action: "Index", controller: "WeiChat_LocationEventLog", url: "/WeiChat_LocationEventLog", iconCls: ""));

                //var menu9 = MenuDataSeedHelper.CreateRootMenu(title: "系统设置", action: "", controller: "", url: "", iconCls: "fa fa-cogs");
                //menus.Add(menu9);
                //menus.Add(MenuDataSeedHelper.CreateChildMenu(parentId: menu9.Id, title: "微信支付设置", action: "Index", controller: "WeiChat_PayConfig", url: "/WeiChat_PayConfig", iconCls: ""));


                context.Site_Menus.AddRange(menus);
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var str = "";
                    foreach (var item in ex.EntityValidationErrors)
                        foreach (var eitem in item.ValidationErrors)
                            str += eitem.PropertyName + ":" + eitem.ErrorMessage;
                    throw new Exception(str);
                }
                //初始化角色菜单
                if (!context.Role_Menus.Any(p => p.RoleId == adminRole.Id))
                {
                    foreach (var item in menus)
                        context.Role_Menus.Add(new Role_Menu
                        {
                            MenuId = item.Id,
                            RoleId = adminRole.Id
                        });
                    context.SaveChanges();
                }
            }

            #endregion

            #region 初始化默认相册
            if (context.Site_PhotoGallerys.Find(new Guid("6FAC25BF-DBE1-423D-93C8-FB31D98D8F72")) == null)
            {
                var photoGallery = new Site_PhotoGallery
                {
                    Id = new Guid("6FAC25BF-DBE1-423D-93C8-FB31D98D8F72"),
                    Title = "首页滚动图片相册",
                    CreateTime = DateTime.Now,
                    CreateBy = user.Id,
                    TenantId = 1
                };
                context.Site_PhotoGallerys.Add(photoGallery);
                context.SaveChanges();
            }
            #endregion

            #region 初始化商品标签
            //精品排行标签
            if (context.Product_Tags.Find(new Guid("527BD456-2411-4BAB-A67B-4760752D7198")) == null)
            {
                var jpphTag = new Product_Tag
                {
                    Id = new Guid("527BD456-2411-4BAB-A67B-4760752D7198"),
                    Name = "精品排行",
                    IsSystem = true,
                    CreateTime = DateTime.Now,
                    CreateBy = user.Id,
                    TenantId = 1
                };
                context.Product_Tags.Add(jpphTag);
            }
            //新品促销标签
            if (context.Product_Tags.Find(new Guid("A2587E2F-B1ED-4F24-A089-4201E40780C5")) == null)
            {
                var xpcxTag = new Product_Tag
                {
                    Id = new Guid("A2587E2F-B1ED-4F24-A089-4201E40780C5"),
                    Name = "新品促销",
                    IsSystem = true,
                    CreateTime = DateTime.Now,
                    CreateBy = user.Id,
                    TenantId = 1
                };
                context.Product_Tags.Add(xpcxTag);
            }
            //热门推荐标签
            if (context.Product_Tags.Find(new Guid("D8970F09-6C1B-429D-8E33-EC16AC90BC11")) == null)
            {
                var rmtjTag = new Product_Tag
                {
                    Id = new Guid("D8970F09-6C1B-429D-8E33-EC16AC90BC11"),
                    Name = "热门推荐",
                    IsSystem = true,
                    CreateTime = DateTime.Now,
                    CreateBy = user.Id,
                    TenantId = 1
                };
                context.Product_Tags.Add(rmtjTag);
            }
            #endregion
            #region 初始化默认广告位
            if (context.Advert_Types.Find(new Guid("6FAC25BF-DBE1-423D-93C8-FB31D98D8F72")) == null)
            {
                var advertType = new Advert_Type
                {
                    Id = new Guid("6FAC25BF-DBE1-423D-93C8-FB31D98D8F72"),
                    Name = "首页滚动图片广告位置",
                    IsSystem = true,
                    CreateTime = DateTime.Now,
                    CreateBy = user.Id,
                    TenantId = 1
                };
                context.Advert_Types.Add(advertType);
                context.SaveChanges();
            }
            #endregion
            #region 添加WeiChat_User
            if (!context.WeiChat_Users.Any())
            {
                var weichatuser = new WeiChat_User
                {
                    OpenId = "o3Q4xv9pXYCBk-pdpvgqWTbs8aLY",
                    Subscribe = true,
                    NickName = "差不多先生",
                    Sex = WeChat.SDK.Apis.User.WeChatSexTypes.Man,
                    City = "linfen",
                    Country = "中国",
                    Province = "山西",
                    Language = "zh_CN",
                    HeadImgUrl = "http://wx.qlogo.cn/mmopen/rycTXpWzibw2l9v7TNN0uFGcjQzHCic6tcOy8dLLnkPTZdP7rZxsSXXQCRJRjoXpKWhkRXnOol544kE6S5Of6uRg/0",
                    UnionId = "1",
                    Remark="9054",
                    GroupIds="102",
                    AllowTest = false,
                    TenantId = 1,
                    SubscribeTime = DateTime.Parse("2016 - 11 - 24 08:58:56.000")
                };
                context.WeiChat_Users.Add(weichatuser);
                context.SaveChanges();
        }
            #endregion
            #region 添加AppUserInfo
            if (!context.AppUserInfos.Any(p => p.OpenId == "o3Q4xv9pXYCBk - pdpvgqWTbs8aLY"))
            {
                var appUserInfo = new AppUserInfo
                {
                    //Id = new Guid(),
                    UserNo = 1,
                    OpenId = "o3Q4xv9pXYCBk - pdpvgqWTbs8aLY",
                    Member_loginname = "bb",
                    Member_password = "123",
                    Nick_name = "bb",
                    Member_ID = 5,
                    Real_Name = "我靠",
                    Empiric_Num = 1,
                    Gold_Num = 100,
                    Balance = 100,
                    ServiceShop_ID = 5,
                    Member_img = new Guid(),
                    Sex = Enumusersexs.男,
                    School_ID = 2,
                    Birth_Date = DateTime.Now,
                    Status = EnumUserStates.Normal,
                    TenantId = 0,
                    CreateTime = DateTime.Now
                };
        context.AppUserInfos.Add(appUserInfo);
                context.SaveChanges();
            }
            #endregion
            #region 创建学校跟学院
            if (!context.User_Schools.Any())
            {
                var user_School1 = new User_School
                {
                    School_Name = "中北大学",
                    ParentID = 0,
                    School_Address = "太原",
                    CreateDate = DateTime.Now
                };
    var user_School2 = new User_School
    {
        School_Name = "山西大学",
        ParentID = 0,
        School_Address = "太原",
        CreateDate = DateTime.Now
    };
    var user_School3 = new User_School
    {
        School_Name = "太原理工大学",
        ParentID = 0,
        School_Address = "太原",
        CreateDate = DateTime.Now
    };
    var user_campus1 = new User_School
    {
        School_Name = "土木工程学院",
        ParentID = user_School1.ID,
        School_Address = "太原",
        CreateDate = DateTime.Now
    };
    var user_campus2 = new User_School
    {
        School_Name = "软件工程学院",
        ParentID = user_School1.ID,
        School_Address = "太原",
        CreateDate = DateTime.Now
    };
    var user_campus3 = new User_School
    {
        School_Name = "环境工程学院",
        ParentID = user_School1.ID,
        School_Address = "太原",
        CreateDate = DateTime.Now
    };
    var user_campus4 = new User_School
    {
        School_Name = "人文学院",
        ParentID = user_School1.ID,
        School_Address = "太原",
        CreateDate = DateTime.Now
    };

    var user_campus5 = new User_School
    {
        School_Name = "软件学院",
        ParentID = user_School2.ID,
        School_Address = "太原",
        CreateDate = DateTime.Now
    };
                context.User_Schools.Add(user_campus5);
                var user_campus6 = new User_School
    {
        School_Name = "土木工程学院",
        ParentID = user_School2.ID,
        School_Address = "太原",
        CreateDate = DateTime.Now
    };
                context.User_Schools.Add(user_campus1); context.User_Schools.Add(user_campus2); context.User_Schools.Add(user_campus3); context.User_Schools.Add(user_campus4); context.User_Schools.Add(user_campus5); context.User_Schools.Add(user_campus6); context.User_Schools.Add(user_School3); context.User_Schools.Add(user_School2); context.User_Schools.Add(user_School1);
                context.SaveChanges();
            }
            #endregion
        }
    }

    internal class MenuDataSeedHelper
{
    private static int orderNo;

    public static Site_Menu CreateRootMenu(string title, string action, string controller, string url,
        string iconCls)
    {
        orderNo++;
        var id = Guid.NewGuid();
        return new Site_Menu
        {
            Id = id,
            Title = title,
            Action = action,
            Controller = controller,
            Url = url,
            IconCls = iconCls,
            Path = id.ToString("N"),
            OrderNo = orderNo,
            Tag = "Tenant"
        };
    }

    public static Site_Menu CreateChildMenu(Guid parentId, string title, string action, string controller,
        string url, string iconCls)
    {
        orderNo++;
        var id = Guid.NewGuid();
        return new Site_Menu
        {
            Id = id,
            ParentId = parentId,
            Title = title,
            Action = action,
            Controller = controller,
            Url = url,
            IconCls = iconCls,
            Tag = "Tenant",
            Path = string.Format("{0}-{1}", parentId.ToString("N"), id.ToString("N")),
            OrderNo = orderNo
        };
    }
}
}