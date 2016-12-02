using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Infrastructure.Notify
{
    /// <summary>
    /// 系统通知
    /// </summary>
    public class NotifyInfo
    {
        /// <summary>
        /// 图标
        /// </summary>
        public string IconCls { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string Href { get; set; }
        /// <summary>
        /// 百分比
        /// </summary>
        public int? Percentage { get; set; }
    }
}
