// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Advert.cs
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

namespace Magicodes.WeiChat.Data.Models.Advert
{
    public class Advert : WeiChat_TenantBase<Guid>
    {
        [Display(Name = "广告位置")]
        public Guid TypeId { get; set; }

        /// <summary>
        ///     广告名称
        /// </summary>
        [Required]
        [Display(Name = "广告名称")]
        public string Name { get; set; }

        /// <summary>
        ///     是否开启
        /// </summary>
        [Display(Name = "是否开启")]
        public bool Enable { get; set; }

        /// <summary>
        ///     广告开始时间
        /// </summary>
        [Display(Name = "广告开始时间")]
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     广告截止时间
        /// </summary>
        [Display(Name = "广告截止时间")]
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     广告图片地址
        /// </summary>
        [Display(Name = "广告图片路径")]
        public string Src { get; set; }

        [Display(Name = "链接地址")]
        public string Url { get; set; }
    }
}