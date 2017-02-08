using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.WeChatStore
{
   public class Sign_Setup
    {
        [Key]
       public int Id { get; set; }
        /// <summary>
        /// 签到天数对比
        /// </summary>
        [Display (Name ="签到天数对比")]
        public int Frequency { get; set; }
        //奖励类型跟奖励值一一对应
        [Display(Name = "奖励")]
        public List<Sign_Reward> Sign_Rewards { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime? CreateDate { get; set; }
    }

    public class Sign_Reward
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "奖励类型")]
        public Reward_type Reward_type { get; set; }

        [Display(Name = "奖励值")]
        public int Reward_num { get; set; }
    }

    public enum Reward_type
    {
        [Display(Name = "经验")]
        经验 = 1,
        [Display(Name = "金币")]
        金币 = 2,
        [Display(Name ="代金券")]
        代金券 = 3
    }
}
