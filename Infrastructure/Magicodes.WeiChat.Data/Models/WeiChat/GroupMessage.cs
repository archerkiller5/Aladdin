// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : GroupMessage.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.WeiChat
{
    /// <summary>
    ///     消息日志
    /// </summary>
    public class WeiChat_GroupMessageLog : WeiChat_TenantBase<long>
    {
        public string UserGroupId { get; set; }

        public SexTypes SexType { get; set; }

        [Required]
        [MaxLength(2000)]
        public string MediaId { get; set; }

        [Required]
        [Display(Name = "类型")]
        public SendMessageTypes MessageType { get; set; }

        /// <summary>
        ///     是否发送成功
        /// </summary>
        [Display(Name = "是否发送成功")]
        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }

    /// <summary>
    ///     性别类型
    /// </summary>
    public enum SexTypes
    {
        [Display(Name = "所有")] All = 0,
        [Display(Name = "未知")] UnKnown = 1,
        [Display(Name = "男")] Man = 2,
        [Display(Name = "女")] Woman = 3
    }

    /// <summary>
    ///     发送类型
    /// </summary>
    public enum SendMessageTypes
    {
        /// <summary>
        ///     文本
        /// </summary>
        [Display(Name = "文本")] Text = 0,

        /// <summary>
        ///     图片
        /// </summary>
        [Display(Name = "图片")] Image = 1,
        ///// <summary>
        ///// 音乐
        ///// </summary>
        //[Display(Name = "音乐")]
        //Music = 2,
        /// <summary>
        ///     语音
        /// </summary>
        [Display(Name = "语音")] Voice = 3,

        /// <summary>
        ///     视频
        /// </summary>
        [Display(Name = "视频")] Video = 4,

        /// <summary>
        ///     图文
        /// </summary>
        [Display(Name = "图文")] News = 5
    }
}