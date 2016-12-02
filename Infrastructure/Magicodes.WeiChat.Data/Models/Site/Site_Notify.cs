using Magicodes.Notify;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.Site
{
    public class Site_Notify : INotifyInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [Display(Name = "链接")]
        public string Href { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [Display(Name = "图标")]
        public string IconCls { get; set; }
        /// <summary>
        /// 任务是否已完成
        /// </summary>
        [Display(Name = "任务是否已完成")]
        public bool IsTaskFinish { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "正文")]
        public string Message { get; set; }
        /// <summary>
        /// 任务进度
        /// </summary>
        [Display(Name = "任务进度")]
        public int? TaskPercentage { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "标题")]
        public string Title { get; set; }
        [MaxLength(128)]
        [Display(Name = "创建人")]
        public string CreateBy { get; set; }
        /// <summary>
        /// 接受对象（User_{UserId}，Tenant_{TenantId}）
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "接受对象")]
        [Index]
        public string Receiver { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        [NotMapped]
        [Display(Name = "是否已读")]
        public bool HasRead { get; set; }
    }
}
