// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SiteFilesManager.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.Infrastructure.Files
{
    /// <summary>
    ///     站点文件管理
    /// </summary>
    public class SiteFilesManager : ThreadSafeLazyBaseSingleton<SiteFilesManager>
    {
        /// <summary>
        ///     站点名称
        /// </summary>
        private const string SiteFilesName = "SiteFiles";

        public SiteFilesManager()
        {
            SiteBasePath = HostingEnvironment.MapPath("~/");
            Debug.Assert(SiteBasePath != null, "SiteBasePath != null");
            SiteFilesPath = Path.Combine(SiteBasePath, SiteFilesName);
            CreatePathIfNotExist(SiteFilesPath);
        }

        /// <summary>
        ///     获取站点根目录
        /// </summary>
        public string SiteBasePath { get; }

        /// <summary>
        ///     站点文件目录
        /// </summary>
        public string SiteFilesPath { get; }

        /// <summary>
        ///     如果路径不存在，则创建
        /// </summary>
        /// <param name="path"></param>
        private static void CreatePathIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        /// <summary>
        ///     创建文件，返回文件流
        /// </summary>
        /// <param name="dirName">目录名称</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public FileStream Create(string dirName, string fileName, out string url)
        {
            var dirPath = string.IsNullOrEmpty(dirName) ? SiteFilesPath : Path.Combine(SiteFilesPath, dirName);
            CreatePathIfNotExist(dirPath);
            url = string.Format("/{0}/{1}/{2}", SiteFilesName, dirName, fileName);
            //url = $"/{SiteFilesName}/{dirName}/{fileName}";
            return File.Create(Path.Combine(dirPath, fileName));
        }
    }
}