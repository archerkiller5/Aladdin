// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_Lottery.cs
//          description :
//  
//          created by 李文强 at  2016/10/03 11:32
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub：https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.Activity
{
    /// <summary>
    ///     抽奖
    /// </summary>
    public class Activity_Lottery : Activity_Base
    {
        public Activity_Lottery()
        {
            PrizeOptions = new List<Activity_LotteryPrizeOption>();
        }

        /// <summary>
        ///     抽奖类型
        /// </summary>
        [Display(Name = "抽奖类型")]
        public LotteryTypes LotteryType { get; set; }

        /// <summary>
        ///     中奖提示
        /// </summary>
        [Display(Name = "中奖提示")]
        [MaxLength(200)]
        [DataType(DataType.MultilineText)]
        public string WinningTips { get; set; }

        /// <summary>
        ///     预计参与人数
        /// </summary>
        [Display(Name = "预计参与人数")]
        public int ExpectedNumber { get; set; }

        /// <summary>
        ///     每人最多允许抽奖次数
        /// </summary>
        [Display(Name = "单人抽奖数")]
        public int AllowNumberPerPerson { get; set; }

        /// <summary>
        ///     每天最多允许抽奖次数
        /// </summary>
        [Display(Name = "每天抽奖数")]
        public int AllowNumberPerDay { get; set; }

        /// <summary>
        ///     奖项设置
        /// </summary>
        public virtual List<Activity_LotteryPrizeOption> PrizeOptions { get; set; }
    }
}