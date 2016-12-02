// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Personal_Survey.cs
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
    ///     参与的调查
    /// </summary>
    public class Personal_Survey : WeiChat_WeChatBase<long>
    {
        /// <summary>
        ///     调查表Id
        /// </summary>
        public int SurveyId { get; set; }
    }
}