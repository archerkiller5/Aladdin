// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SurveyTopicTypes.cs
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
    ///     题目类型
    /// </summary>
    public enum SurveyTopicTypes
    {
        [Display(Name = "单选题")] SingleChoice = 0,
        [Display(Name = "多选题")] MultipleChoice = 1,
        [Display(Name = "问答题")] EssayQuestion = 2
    }
}