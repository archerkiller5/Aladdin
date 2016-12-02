// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Personal_SurveyAnswer.cs
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
    ///     问卷答案
    /// </summary>
    public class Personal_SurveyAnswer : WeiChat_WeChatBase<long>
    {
        /// <summary>
        ///     调查表Id
        /// </summary>
        public int SurveyId { get; set; }

        /// <summary>
        ///     题目Id
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        ///     题目类型
        /// </summary>
        [Display(Name = "题目类型")]
        public SurveyTopicTypes TopicType { get; set; }

        /// <summary>
        ///     选项Id
        /// </summary>
        public int OptionId { get; set; }

        /// <summary>
        ///     文本答案
        /// </summary>
        [MaxLength(2000)]
        public string Answer { get; set; }
    }
}