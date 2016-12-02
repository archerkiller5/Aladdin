// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ConfigHelper.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Magicodes.Shop.Helpers.UEEditor
{
    /// <summary>
    ///     UEEditor配置辅助类
    /// </summary>
    public static class ConfigHelper
    {
        private static readonly bool noCache = true;
        private static JObject _Items;

        public static JObject Items
        {
            get
            {
                if (noCache || (_Items == null))
                    _Items = BuildItems();
                return _Items;
            }
        }

        private static JObject BuildItems()
        {
            var json = File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/ueeditor/config.json"));
            return JObject.Parse(json);
        }


        public static T GetValue<T>(string key)
        {
            return Items[key].Value<T>();
        }

        public static string[] GetStringList(string key)
        {
            return Items[key].Select(x => x.Value<string>()).ToArray();
        }

        public static string GetString(string key)
        {
            return GetValue<string>(key);
        }

        public static int GetInt(string key)
        {
            return GetValue<int>(key);
        }
    }
}