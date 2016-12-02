using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.Settings
{
    /// <summary>
    /// 短信模板
    /// </summary>
    public class Settings_SmsTemplate : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        /// 模板编号
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "模板编号")]
        public string TemplateCode { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "签名")]
        public string SignName { get; set; }

        /// <summary>
        ///     模板内容
        /// </summary>
        [Display(Name = "模板内容")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        /// <summary>
        ///     示例内容
        /// </summary>
        [Display(Name = "示例内容")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Demo { get; set; }

        [Display(Name = "短信类型")]
        public SmsTypes SmsType { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
