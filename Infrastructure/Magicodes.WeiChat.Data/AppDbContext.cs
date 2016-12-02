// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppDbContext.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using EntityFramework.DynamicFilters;
using Magicodes.Data.Multitenant;
using Magicodes.WeiChat.Data.Migrations;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Data.Models.Interface;
using Magicodes.WeiChat.Data.Models.PhotoGallery;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Data.Models.WeiChat;
using System.Web.Configuration;

namespace Magicodes.WeiChat.Data
{
    public partial class AppDbContext :
        MultitenantIdentityDbContext<AppUser, AppRole, string, int, AppUserLogin, AppUserRole, AppUserClaim>
    {
        static AppDbContext()
        {
            //初始化时自动更新迁移到最新版本
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Configuration>());
        }

        /// <summary>
        ///     初始化DbContext
        /// </summary>
        public AppDbContext()
            : base("DefaultConnection")
        {
        }

        /// <summary>
        ///     初始化DbContext
        /// </summary>
        /// <param name="strConnection"></param>
        public AppDbContext(string strConnection)
            : base(strConnection)
        {
        }

        /// <summary>
        ///     租户信息
        /// </summary>
        public DbSet<Admin_Tenant> Admin_Tenants { get; set; }

        /// <summary>
        ///     微信App
        /// </summary>
        public DbSet<WeiChat_App> WeiChat_Apps { get; set; }

        /// <summary>
        ///     微信支付设置
        /// </summary>
        public DbSet<WeiChat_Pay> WeiChat_Pays { get; set; }

        /// <summary>
        ///     微信用户
        /// </summary>
        public DbSet<WeiChat_User> WeiChat_Users { get; set; }

        /// <summary>
        ///     微信用户用户组信息
        /// </summary>
        public DbSet<WeiChat_UserGroup> WeiChat_UserGroups { get; set; }

        /// <summary>
        ///     多客服信息
        /// </summary>
        public DbSet<WeiChat_KFCInfo> WeiChat_KFCInfos { get; set; }

        /// <summary>
        ///     微信同步日志
        /// </summary>
        public DbSet<WeiChat_SyncLog> WeiChat_SyncLogs { get; set; }

        /// <summary>
        ///     自定义菜单数据缓存
        /// </summary>
        public DbSet<WeiChat_Menu> WeiChat_Menus { get; set; }

        /// <summary>
        ///     微信二维码
        /// </summary>
        public DbSet<WeiChat_QRCode> WeiChat_QRCodes { get; set; }

        /// <summary>
        ///     关注时回复
        /// </summary>
        public DbSet<WeiChat_SubscribeReply> WeiChat_SubscribeReplies { get; set; }

        /// <summary>
        ///     答不上来配置
        /// </summary>
        public DbSet<WeiChat_NotAnswerReply> WeiChat_NotAnswerReplies { get; set; }

        /// <summary>
        ///     消息发送记录
        /// </summary>
        public DbSet<WeiChat_GroupMessageLog> WeiChat_GroupMessageLogs { get; set; }

        /// <summary>
        ///     位置事件日志记录
        /// </summary>
        public DbSet<WeiChat_LocationEventLog> WeiChat_LocationEventLogs { get; set; }

        /// <summary>
        ///     角色菜单
        /// </summary>
        public DbSet<Role_Menu> Role_Menus { get; set; }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //会自动创建
            base.OnModelCreating(modelBuilder);
            var dbType = WebConfigurationManager.AppSettings["dbType"];
            if (dbType != "MySql")
            {
                modelBuilder.Entity<AppUser>()
               .Property(e => e.TenantId)
               .IsRequired()
               .HasColumnAnnotation(
                   "Index",
                   new IndexAnnotation(new IndexAttribute("UserNameIndex", order: 0)
                   {
                       IsUnique = true
                   }));
            }

            //MySQL不支持数据库架构
            var tablePrefix = dbType != "MySql" ? "MWC." : "";
            modelBuilder.Types().Configure(entity => entity.ToTable(tablePrefix + entity.ClrType.Name));
            //将默认的表名通通改了,用户角色表统一加上Account前缀
            modelBuilder.Entity<AppUser>().ToTable(tablePrefix + "Admin_Users")
                //MySQL索引长度限制
                .Property(p => p.UserName).HasMaxLength(255);

            modelBuilder.Entity<AppRole>().ToTable(tablePrefix + "Admin_Roles")
                //MySQL索引长度限制
                .Property(p => p.Name).HasMaxLength(255);

            modelBuilder.Entity<AppUserClaim>().ToTable(tablePrefix + "Admint_UserClaims");
            modelBuilder.Entity<AppUserLogin>().ToTable(tablePrefix + "Admin_UserLogin");
            modelBuilder.Entity<AppUserRole>().ToTable(tablePrefix + "Admin_UserRoles");

            //筛选多租户
            modelBuilder.Filter("TenantEntryFilter", (ITenantId app, int tenantId) => app.TenantId == tenantId, 0);

            //删除Filter
            //modelBuilder.Filter("IsDeletedFilter", (IDeleted d) => d.IsDeleted, false);
            //默认禁用多租户
            modelBuilder.DisableFilterGlobally("TenantEntryFilter");
            //modelBuilder.DisableFilterGlobally("IsDeletedFilter");
        }

        #region 关键字

        /// <summary>
        ///     微信关键字自动答复
        /// </summary>
        public DbSet<WeiChat_KeyWordAutoReply> WeiChat_KeyWordAutoReplies { get; set; }

        /// <summary>
        ///     微信文本回复关键字
        /// </summary>
        public DbSet<WeiChat_KeyWordTextContent> WeiChat_KeyWordTextContents { get; set; }

        /// <summary>
        ///     微信图片回复关键字
        /// </summary>
        public DbSet<WeiChat_KeyWordImageContent> WeiChat_KeyWordImageContents { get; set; }

        /// <summary>
        ///     微信音乐回复关键字
        /// </summary>
        public DbSet<WeiChat_KeyWordMusicContent> WeiChat_KeyWordMusicContents { get; set; }

        /// <summary>
        ///     微信语音回复关键字
        /// </summary>
        public DbSet<WeiChat_KeyWordVoiceContent> WeiChat_KeyWordVoiceContents { get; set; }

        /// <summary>
        ///     微信视频回复关键字
        /// </summary>
        public DbSet<WeiChat_KeyWordVideoContent> WeiChat_KeyWordVideoContents { get; set; }

        /// <summary>
        ///     微信新闻（单图文或多图文）回复关键字
        /// </summary>
        public DbSet<WeiChat_KeyWordNewsContent> WeiChat_KeyWordNewsContents { get; set; }

        /// <summary>
        ///     多条图文消息信息
        /// </summary>
        public DbSet<WeiChat_KeyWordNewsArticle> WeiChat_KeyWordNewsArticles { get; set; }

        /// <summary>
        ///     回复日志
        /// </summary>
        public DbSet<WeiChat_KeyWordReplyLog> WeiChat_KeyWordReplyLogs { get; set; }

        #endregion

        #region 模板消息

        /// <summary>
        ///     模板消息模板
        /// </summary>
        public DbSet<WeiChat_MessagesTemplate> WeiChat_MessagesTemplates { get; set; }

        /// <summary>
        ///     模板消息日志
        /// </summary>
        public DbSet<WeiChat_MessagesTemplateSendLog> WeiChat_MessagesTemplateSendLogs { get; set; }

        #endregion

        #region 站点相关

        /// <summary>
        ///     站点资源类型
        /// </summary>
        public DbSet<Site_ResourceType> Site_ResourceTypes { get; set; }

        /// <summary>
        ///     站点图片
        /// </summary>
        public DbSet<Site_Image> Site_Images { get; set; }

        /// <summary>
        ///     语音
        /// </summary>
        public DbSet<Site_Voice> Site_Voices { get; set; }

        /// <summary>
        ///     视频
        /// </summary>
        public DbSet<Site_Video> Site_Videos { get; set; }

        /// <summary>
        ///     文章
        /// </summary>
        public DbSet<Site_Article> Site_Articles { get; set; }


        /// <summary>
        ///     文章
        /// </summary>
        public DbSet<Site_Articlea> Site_Articlesa { get; set; }

        /// <summary>
        ///     多图文
        /// </summary>
        public DbSet<Site_News> Site_News { get; set; }

        /// <summary>
        ///     多图文文章
        /// </summary>
        public DbSet<Site_NewsArticle> Site_NewsArticles { get; set; }

        /// <summary>
        ///     站点菜单
        /// </summary>
        public DbSet<Site_Menu> Site_Menus { get; set; }

        /// <summary>
        /// 站点通知
        /// </summary>
        public DbSet<Site_Notify> Site_Notifies { get; set; }

        /// <summary>
        /// 站点已读通知
        /// </summary>
        public DbSet<Site_ReadNotify> Site_ReadNotifies { get; set; }

        #endregion

        #region 设置

        public DbSet<App_SettingGroup> App_SettingGroups { get; set; }
        public DbSet<App_SettingValue> App_SettingValues { get; set; }

        #endregion

        #region 相册相关

        public DbSet<Site_PhotoGallery> Site_PhotoGallerys { get; set; }
        /// <summary>
        /// 相册图片
        /// </summary>
        public DbSet<Site_Photo> Site_Photos { get; set; }

        #endregion
    }
}