using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Domain
{ 
    /// <summary>
    /// 设置辅助类
    /// </summary>
    public class SettingDo : ThreadSafeLazyBaseSingleton<SettingDo>
    {
        /// <summary>
        /// 获取设置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSettings<T>(AppDbContext db = null) where T : Data.Models.WeiChat_AdminUniqueTenantBase<int>, new()
        {
            if (db != null)
            {
                return db.Set<T>().FirstOrDefault() ?? new T();
            }
            else
            {
                using (db = new AppDbContext())
                {
                    return db.Set<T>().FirstOrDefault() ?? new T();
                }
            }
        }
    }
}
