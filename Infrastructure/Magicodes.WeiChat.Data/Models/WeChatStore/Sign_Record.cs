using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicodes.WeiChat.Data.Models.WeChatStore
{
    public class Sign_Record : WeiChat_WeChatBase<int>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Display(Name ="用户ID")]
        public string Member_ID { get; set; }
        /// <summary>
        /// 最后一次登录时间
        /// </summary>
        [Display(Name = "最后一次签到时间")]
        public DateTime LatestTime { get; set; }
        /// <summary>
        /// 连续签到次数
        /// </summary>
        [Display(Name = "连续签到次数")]
        public int Sign_num { get; set; }
        [NotMapped]
        [Display(Name ="还需要签到天数")]
        public int NeedDay { get; set; }
        [NotMapped]
        [Display(Name = "将会获得的奖励")]
        public string Reword { get; set; }
    }
}
