// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_SurveyChoiceTopic.cs
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
    ///     选择题
    /// </summary>
    public class Activity_SurveyChoiceTopic : Activity_SurveyTopicBase
    {
        [Display(Name = "选项")]
        public List<Activity_SurveyOption> Options { get; set; }
    }
}