// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ConfigManager.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Concurrent;

//======================================================================
//
//        Copyright (C) 2014-2016 Magicodes.WeiChat    
//        All rights reserved
//
//        filename :ConfigManager
//        description :
//
//        created by 雪雁 at  2015/7/10 15:45:04
//        http://www.magicodes.net
//
//======================================================================

namespace Magicodes.WeiChat.Infrastructure.Config
{
    /// <summary>
    ///     配置管理器
    /// </summary>
    public class ConfigManager
    {
        private static readonly object _locker = new object(); //锁对象
        protected ConcurrentDictionary<Type, ConfigBase> Configs = new ConcurrentDictionary<Type, ConfigBase>();

        public T Load<T>()
        {
            //return SerializeHelper.Load<T>(path);
            throw new NotSupportedException();
        }

        public void Save<T>(T configType) where T : ConfigBase
        {
            throw new NotSupportedException();
        }

        public T Get<T>() where T : ConfigBase
        {
            lock (_locker)
            {
                var type = typeof(T);
                if (Configs.ContainsKey(type))
                {
                    return Configs[type] as T;
                }
                var config = Load<T>();
                if (config != null)
                {
                    Configs.AddOrUpdate(type, config, (tKey, existingVal) => { return config; });
                    return config;
                }
            }
            return default(T);
        }
    }
}