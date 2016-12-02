using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Unity
{
    /// <summary>
    /// 安全返回辅助类
    /// </summary>
    public static class SafeReturnHelper
    {
        /// <summary>
        /// 安全返回
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">执行函数</param>
        /// <returns>返回对象</returns>
        public static T SafeReturn<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
