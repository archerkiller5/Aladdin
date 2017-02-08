using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.Activity
{
    /// <summary>
    /// 投票
    /// </summary>
    public class Activity_Vote: Activity_SurveyTopicBase
    {
        /// <summary>
        /// 投票人
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 投票时间
        /// </summary>
        [Display(Name ="投票时间")]
        public DateTime VoteTime { get; set; }
        /// <summary>
        /// 选项
        /// </summary>
        [Display(Name = "选项")]
        public virtual List<Activity_VoteOption> Options { get; set; }
    }
}
