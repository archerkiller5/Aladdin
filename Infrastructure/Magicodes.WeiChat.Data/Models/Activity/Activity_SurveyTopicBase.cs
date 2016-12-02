// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_SurveyTopicBase.cs
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
    ///     题目
    /// </summary>
    public class Activity_SurveyTopicBase
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }

        [Display(Name = "题目")]
        [MaxLength(1000)]
        public string Title { get; set; }

        /// <summary>
        ///     序号
        /// </summary>
        [Display(Name = "序号")]
        public int Order { get; set; }

        /// <summary>
        ///     题目类型
        /// </summary>
        [Display(Name = "题目类型")]
        public SurveyTopicTypes TopicType { get; set; }
    }
}