// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : InitializerManager.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Magicodes.Logger;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Infrastructure.Logging;
using Magicodes.WeiChat.Unity;

namespace Magicodes.WeiChat.Infrastructure.Initialization
{
    /// <summary>
    ///     初始化管理器
    /// </summary>
    public class InitializerManager : ThreadSafeLazyBaseSingleton<InitializerManager>
    {
        /// <summary>
        ///     初始化器
        /// </summary>
        public List<IInitializer> Initializers = new List<IInitializer>();

        /// <summary>
        ///     日志
        /// </summary>
        public LoggerBase Logger { get; set; }

        /// <summary>
        ///     执行所有初始化
        /// </summary>
        public void StartAllInitializer()
        {
            using (var db = new AppDbContext())
            {
                foreach (
                    var currentassembly in
                    AppDomain.CurrentDomain.GetAssemblies().Where(p => p.FullName.StartsWith("Magicodes")))
                    try
                    {
                        currentassembly.GetTypes()
                            .Where(p => p.IsClass && (p.GetInterface(typeof(IInitializer).FullName) != null))
                            .Each(t =>
                            {
                                var type = (IInitializer) Activator.CreateInstance(t);
                                if (type == null)
                                {
                                    Logger.LogFormat(LoggerLevels.Error,
                                        "CreateInstance 失败！ AssemblyFullName:{0}\tFullName:{1}", t.Assembly.FullName,
                                        t.FullName);
                                }
                                else
                                {
                                    type.Logger = Logger;
                                    type.Context = db;
                                    Initializers.Add(type);
                                }
                            });
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        Logger.LogException(ex);
                    }
                //按优先级执行
                foreach (var item in Initializers.OrderByDescending(p => p.Level))
                    try
                    {
                        Logger.LogFormat(LoggerLevels.Trace, "Level:{0}\tFullName:{1}", item.Level,
                            item.GetType().FullName);
                        item.Initialize();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LoggerLevels.Error,
                            string.Format("执行程序集Initialize方法出错。Assembly:{0}，Type:{1}{2}{3}",
                                item.GetType().Assembly.FullName, item.GetType().FullName, Environment.NewLine, ex));
                    }
            }
        }
    }
}