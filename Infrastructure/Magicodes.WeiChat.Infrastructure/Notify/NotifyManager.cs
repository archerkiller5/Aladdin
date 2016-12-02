using Magicodes.WeiChat.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Infrastructure.Notify
{
    /// <summary>
    /// 通知管理器
    /// </summary>
    public class NotifyManager : ThreadSafeLazyBaseSingleton<NotifyManager>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notify"></param>
        /// <param name="group">通知组，为null则表示所有，可以为UserId、RoleName、租户Id</param>
        public void NotifyTo(NotifyInfo notify, string group = null)
        {
            
        }

        //public List<NotifyInfo> GetLastNofityList(string group = null)
        //{

        //}
    }
}
