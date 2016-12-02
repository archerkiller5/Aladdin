// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_QRCode.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.WeiChat
{
    /// <summary>
    ///     二维码类型
    /// </summary>
    public enum QRCodeUseForTypes
    {
        /// <summary>
        ///     其他
        /// </summary>
        [Display(Name = "其他")] Orthers = 0,

        /// <summary>
        ///     绑定微信管理员
        /// </summary>
        [Display(Name = "绑定管理员")] BindManager = 1
    }

    /// <summary>
    ///     微信二维码
    /// </summary>
    public class WeiChat_QRCode : WeiChat_TenantBase<int>
    {
        /// <summary>
        ///     参数值
        /// </summary>
        [Display(Name = "参数值")]
        [Required]
        public string ParamsValue { get; set; }

        /// <summary>
        ///     过期时间（秒）
        /// </summary>
        [Display(Name = "过期时间（秒）")]
        [Required]
        public int ExpireSeconds { get; set; }

        /// <summary>
        ///     有效时间
        /// </summary>
        [Display(Name = "有效时间")]
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(50)]
        public string Remark { get; set; }

        /// <summary>
        ///     获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。
        /// </summary>
        [MaxLength(100)]
        public string Ticket { get; set; }

        /// <summary>
        ///     用于
        /// </summary>
        [Display(Name = "用于")]
        public QRCodeUseForTypes UserFor { get; set; }
    }
}