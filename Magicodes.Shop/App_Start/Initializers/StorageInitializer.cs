// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StorageInitializer.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.IO;
using System.Web.Hosting;
using Magicodes.Logger;
using Magicodes.Storage.Local;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.Initialization;

namespace Magicodes.Shop.Initializers
{
    /// <summary>
    ///     存储管理器初始化
    /// </summary>
    public class StorageInitializer : IInitializer
    {
        /// <summary>
        ///     日志记录器
        /// </summary>
        public LoggerBase Logger { get; set; }

        /// <summary>
        ///     数据库上下文对象，系统会自动注入
        /// </summary>
        public AppDbContext Context { get; set; }

        /// <summary>
        ///     初始化优先级
        /// </summary>
        public InitializerLevels Level
        {
            get { return InitializerLevels.High; }
        }

        public void Initialize()
        {
            var baseDir = HostingEnvironment.MapPath("~/");
            baseDir = Path.Combine(baseDir, "Storage");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);
            //设置默认存储提供程序
            SiteStorageManager.Current.DefaultStorageProvider = new LocalStorageProvider(baseDir, "/Storage");
            //设置相册存储提供程序
            SiteStorageManager.Current.PhotoGalleryStorageProvider =
                new LocalStorageProvider(Path.Combine(baseDir, "Gallery"), "/Storage/Gallery");
        }
    }
}