using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Unity
{
    /// <summary>
    /// 线程安全延迟单例创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ThreadSafeLazyBaseSingleton<T>
        where T : new()
    {
        private static readonly Lazy<T> Lazy = new Lazy<T>(() => new T());
        /// <summary>
        /// 当前实例单例对象
        /// </summary>
        public static T Current
        {
            get
            {
                return Lazy.Value;
            }
        }
    }
}
