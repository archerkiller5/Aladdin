// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SiteStorageManager.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:23
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Magicodes.Storage;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.Infrastructure
{
    public class SiteStorageManager : ThreadSafeLazyBaseSingleton<SiteStorageManager>
    {
        /// <summary>
        ///     默认的存储提供程序
        /// </summary>
        public IStorageProvider DefaultStorageProvider { get; set; }

        /// <summary>
        ///     相册存储提供程序
        /// </summary>
        public IStorageProvider PhotoGalleryStorageProvider { get; set; }
    }
}