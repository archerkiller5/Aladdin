// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_SurveyOption.cs
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
    ///     选项
    /// </summary>
    public class Activity_SurveyOption
    {
        public int Id { get; set; }

        [Display(Name = "选项文本")]
        [MaxLength(500)]
        public string OptionText { get; set; }
    }
}