using Magicodes.Storage.Local;
using Magicodes.WeiChat.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Magicodes.Shop.App_Start
{
    public static class StorageProviderConfig
    {
        public static void ConfigureStorageProvider()
        {
            var baseDir = HostingEnvironment.MapPath("~/");
            baseDir = Path.Combine(baseDir, "Storage");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            WeiChatApplicationContext.Current.StorageProvider = new LocalStorageProvider(Path.Combine(baseDir, "Default"), "/Storage/Default");
        }
    }
}