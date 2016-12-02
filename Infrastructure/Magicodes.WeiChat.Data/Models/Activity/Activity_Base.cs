// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_Base.cs
//          description :
//  
//          created by 李文强 at  2016/10/03 11:32
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub：https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.Activity
{
    /// <summary>
    ///     活动基础类
    /// </summary>
    public class Activity_Base : WeiChat_TenantBase<int>, IValidatableObject
    {
        [Display(Name = "活动名称")]
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        /// <summary>
        ///     开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime? EndTime { get; set; }

        [Display(Name = "活动描述")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "是否启用")]
        public bool IsEnable { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime.Date < DateTime.Now.Date)
                yield return new ValidationResult("开始时间 必须大于等于今天", new[] {"StartTime"});

            if ((EndTime != null) && (EndTime.Value.Date < StartTime.Date))
                yield return new ValidationResult("结束时间 必须大于等于 开始时间", new[] {"EndTime"});
        }
    }
}