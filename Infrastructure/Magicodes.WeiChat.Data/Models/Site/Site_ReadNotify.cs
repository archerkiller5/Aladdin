using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.Site
{
    /// <summary>
    /// 已读通知
    /// </summary>
    public class Site_ReadNotify
    {
        public long Id { get; set; }
        /// <summary>
        /// 通知Id
        /// </summary>
        public int NotifyId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [MaxLength(128)]
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
