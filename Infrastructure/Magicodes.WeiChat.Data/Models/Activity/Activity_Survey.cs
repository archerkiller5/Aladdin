// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_Survey.cs
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
    ///     调查
    /// </summary>
    public class Activity_Survey : Activity_Base
    {
        public Activity_Survey()
        {
            AllowNumberPerPerson = 1;
            AllowNumberPerDay = 1;
        }

        /// <summary>
        ///     每人最多允许参与次数
        /// </summary>
        [Display(Name = "单人最多参与次数")]
        public int AllowNumberPerPerson { get; set; }

        /// <summary>
        ///     每天最多允许次数
        /// </summary>
        [Display(Name = "每天最多参与次数")]
        public int AllowNumberPerDay { get; set; }
    }
}