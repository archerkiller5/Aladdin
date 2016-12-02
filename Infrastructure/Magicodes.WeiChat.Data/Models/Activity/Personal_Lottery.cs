// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Personal_Lottery.cs
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
    ///     我的抽奖记录
    /// </summary>
    public class Personal_Lottery : WeiChat_WeChatBase<long>
    {
        /// <summary>
        ///     抽奖Id
        /// </summary>
        public int LotteryId { get; set; }

        /// <summary>
        ///     是否中奖
        /// </summary>
        public bool IsWinning { get; set; }

        [MaxLength(50)]
        public string Prize { get; set; }

        public int? LotteryPrizeOptionId { get; set; }
    }
}