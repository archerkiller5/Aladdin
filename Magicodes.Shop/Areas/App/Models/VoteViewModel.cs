using Magicodes.WeiChat.Data.Models.Activity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Areas.App.Models
{
    public class VoteViewModel
    {
        public int Id { get; set; }
        [Display(Name = "投票人")]
        public string Voter { get; set; }
        [Display(Name = "标题")]
        public string Title { get; set; }
        public string OpenId { get; set; }
        [Display(Name = "投票时间")]
        public DateTime Votetime { get; set; }
        [Display(Name = "投票开始时间")]
        public DateTime StartTime { get; set; }
        [Display(Name = "投票截止时间")]
        public DateTime? EndTime { get; set;}
        /// <summary>
        /// 每人允许次数
        /// </summary>
        [Display(Name = "每人允许次数")]
        public int AllowNumberPerPerson { get; set; }
        /// <summary>
        ///     每天最多允许次数
        /// </summary>
        [Display(Name = "每天允许次数")]
        public int AllowNumberPerDay { get; set; }
        public List<Activity_SurveyOption> VoteOption { get; set; }
    }
}