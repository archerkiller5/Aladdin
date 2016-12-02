// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_LocationEventLog.cs
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
using Magicodes.WeiChat.Data.Models.Interface;

namespace Magicodes.WeiChat.Data.Models.WeiChat
{
    public class WeiChat_LocationEventLog : ITenantId
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "来自")]
        [MaxLength(50)]
        public string From { get; set; }

        [Display(Name = "To")]
        [MaxLength(50)]
        public string To { get; set; }

        /// <summary>
        ///     地理位置纬度，事件类型为LOCATION的时存在
        /// </summary>
        [Display(Name = "纬度")]
        public double Latitude { get; set; }

        /// <summary>
        ///     地理位置经度，事件类型为LOCATION的时存在
        /// </summary>
        [Display(Name = "经度")]
        public double Longitude { get; set; }

        /// <summary>
        ///     地理位置精度，事件类型为LOCATION的时存在
        /// </summary>
        [Display(Name = "精度")]
        public double Precision { get; set; }

        public int TenantId { get; set; }
    }
}