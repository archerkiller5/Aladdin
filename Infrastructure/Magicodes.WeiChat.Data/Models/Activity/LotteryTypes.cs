// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : LotteryTypes.cs
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
    ///     抽奖类型
    /// </summary>
    public enum LotteryTypes
    {
        /// <summary>
        ///     大转盘
        /// </summary>
        [Display(Name = "大转盘")] BigWheel = 0,

        /// <summary>
        ///     砸金蛋
        /// </summary>
        [Display(Name = "砸金蛋")] HitGoldenEggs = 1,

        /// <summary>
        ///     刮刮乐
        /// </summary>
        [Display(Name = "刮刮乐")] Scratch = 2
    }
}