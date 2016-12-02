// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Activity_EssayQuestionTopic.cs
//          description :
//  
//          created by 李文强 at  2016/10/03 11:32
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub：https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.WeiChat.Data.Models.Activity
{
    /// <summary>
    ///     问答题
    /// </summary>
    public class Activity_EssayQuestionTopic : Activity_SurveyTopicBase
    {
        public Activity_EssayQuestionTopic()
        {
            TopicType = SurveyTopicTypes.EssayQuestion;
        }
    }
}