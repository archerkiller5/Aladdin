// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_LotteryPrizeOption.cs
//          description :
//  
//          created by 李文强 at  2016/10/03 11:32
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub：https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.Activity
{
    /// <summary>
    ///     奖项
    /// </summary>
    public class Activity_LotteryPrizeOption : WeiChat_TenantBase<int>
    {
        /// <summary>
        ///     奖项名称
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "奖项名称")]
        public string Title { get; set; }

        /// <summary>
        ///     奖项
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "奖项")]
        public string Prize { get; set; }

        /// <summary>
        ///     数量
        /// </summary>
        [Display(Name = "数量")]
        public int PrizeCount { get; set; }

        /// <summary>
        ///     剩余数量
        /// </summary>
        [Display(Name = "剩余数量")]
        public int OverCount { get; set; }
    }
}