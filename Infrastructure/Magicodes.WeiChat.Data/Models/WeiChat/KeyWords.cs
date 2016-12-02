// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : KeyWords.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Magicodes.WeiChat.Data.Models.Interface;

namespace Magicodes.WeiChat.Data.Models.WeiChat
{

    #region 关键字处理

    /// <summary>
    ///     微信关键字
    /// </summary>
    public class WeiChat_KeyWordContentTypeBase : ITenantId, IAdminCreate<string>, IAdminUpdate<string>
    {
        public WeiChat_KeyWordContentTypeBase()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     创建者
        /// </summary>
        [Display(Name = "创建者")]
        //[NotMapped]
        [ForeignKey("CreateBy")]
        public AppUser CreateUser { get; set; }

        /// <summary>
        ///     编辑者
        /// </summary>
        [MaxLength(256)]
        [Display(Name = "最后编辑")]
        [ForeignKey("UpdateBy")]
        //[NotMapped]
        public AppUser UpdateUser { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     创建者
        /// </summary>
        [MaxLength(128)]
        public string CreateBy { get; set; }

        /// <summary>
        ///     更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        ///     更新者
        /// </summary>
        [MaxLength(128)]
        [Display(Name = "最后编辑")]
        public string UpdateBy { get; set; }

        public int TenantId { get; set; }
    }

    /// <summary>
    ///     匹配类型
    /// </summary>
    public enum KeyWordMatchTypes
    {
        /// <summary>
        ///     包含
        /// </summary>
        [Display(Name = "包含")] Contains = 0,

        /// <summary>
        ///     等于
        /// </summary>
        [Display(Name = "等于")] Equals = 1
    }

    /// <summary>
    ///     微信关键字自动答复
    /// </summary>
    public class WeiChat_KeyWordAutoReply : ITenantId, IAdminCreate<string>, IAdminUpdate<string>
    {
        public WeiChat_KeyWordAutoReply()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        /// <summary>
        ///     关键字
        /// </summary>
        [Display(Name = "关键字")]
        [MaxLength(100)]
        [Required]
        public string KeyWord { get; set; }

        /// <summary>
        ///     创建者
        /// </summary>
        [Display(Name = "创建者")]
        //[NotMapped]
        [ForeignKey("CreateBy")]
        public AppUser CreateUser { get; set; }

        /// <summary>
        ///     编辑者
        /// </summary>
        [Display(Name = "最后编辑")]
        //[NotMapped]
        [ForeignKey("UpdateBy")]
        public AppUser UpdateUser { get; set; }

        /// <summary>
        ///     匹配类型
        /// </summary>
        [Display(Name = "匹配类型")]
        public KeyWordMatchTypes MatchType { get; set; }

        /// <summary>
        ///     关键字类型
        /// </summary>
        [Display(Name = "类型")]
        public KeyWordContentTypes KeyWordContentType { get; set; }

        /// <summary>
        ///     内容Id
        /// </summary>
        [Display(Name = "内容")]
        public Guid? ContentId { get; set; }

        [Display(Name = "是否支持菜单事件关键字触发")]
        public bool AllowEventKey { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     创建者
        /// </summary>
        [MaxLength(128)]
        public string CreateBy { get; set; }

        /// <summary>
        ///     更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        ///     更新者
        /// </summary>
        [MaxLength(128)]
        public string UpdateBy { get; set; }

        public int TenantId { get; set; }
    }

    /// <summary>
    ///     关键字回复类型
    ///     参考：http://mp.weixin.qq.com/wiki/14/89b871b5466b19b3efa4ada8e577d45e.html
    /// </summary>
    public enum KeyWordContentTypes
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
        [Display(Name = "图文")] News = 5,

        /// <summary>
        ///     接入客服
        /// </summary>
        [Display(Name = "接入客服")] CustomerService = 6
    }

    /// <summary>
    ///     微信文本回复关键字
    /// </summary>
    public class WeiChat_KeyWordTextContent : WeiChat_KeyWordContentTypeBase
    {
        /// <summary>
        ///     内容
        /// </summary>
        [Display(Name = "文本内容")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }

    /// <summary>
    ///     微信图片回复关键字
    /// </summary>
    public class WeiChat_KeyWordImageContent : WeiChat_KeyWordContentTypeBase
    {
        /// <summary>
        ///     图片资源Id
        /// </summary>
        public string ImageMediaId { get; set; }
    }

    /// <summary>
    ///     微信音乐回复关键字
    /// </summary>
    public class WeiChat_KeyWordMusicContent : WeiChat_KeyWordContentTypeBase
    {
        /// <summary>
        ///     标题
        /// </summary>
        public string Title { get; set; }

        public string Description { get; set; }
        public string MusicUrl { get; set; }
        public string HQMusicUrl { get; set; }
        public string ThumbMediaId { get; set; }
    }

    /// <summary>
    ///     微信语音回复关键字
    /// </summary>
    public class WeiChat_KeyWordVoiceContent : WeiChat_KeyWordContentTypeBase
    {
        public string VoiceMediaId { get; set; }
    }

    /// <summary>
    ///     微信视频回复关键字
    /// </summary>
    public class WeiChat_KeyWordVideoContent : WeiChat_KeyWordContentTypeBase
    {
        public string MediaId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    ///     微信新闻（单图文或多图文）回复关键字
    /// </summary>
    public class WeiChat_KeyWordNewsContent : WeiChat_KeyWordContentTypeBase
    {
        public WeiChat_KeyWordNewsContent()
        {
            Articles = new List<WeiChat_KeyWordNewsArticle>();
        }

        [NotMapped]
        [Display(Name = "图文消息标题")]
        public string Title
        {
            get { return Articles.FirstOrDefault() == null ? "" : Articles.FirstOrDefault().Title; }
        }

        [Display(Name = "图片路径")]
        [NotMapped]
        public string PicUrl
        {
            get { return Articles.FirstOrDefault().PicUrl; }
        }

        /// <summary>
        ///     文章列表
        /// </summary>
        public virtual ICollection<WeiChat_KeyWordNewsArticle> Articles { get; set; }
    }

    /// <summary>
    ///     多条图文消息信息，默认第一个item为大图,注意，如果图文数超过10，则将会无响应
    /// </summary>
    public class WeiChat_KeyWordNewsArticle : WeiChat_TenantBase<long>
    {
        /// <summary>
        ///     图文消息标题
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "图文消息标题")]
        public string Title { get; set; }

        /// <summary>
        ///     图文消息描述
        /// </summary>
        [MaxLength(200)]
        [Display(Name = "图文消息描述")]
        public string Description { get; set; }

        /// <summary>
        ///     图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        /// </summary>
        [MaxLength(300)]
        [Display(Name = "图片链接")]
        public string PicUrl { get; set; }

        /// <summary>
        ///     点击图文消息跳转链接
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "点击图文消息跳转链接")]
        public string Url { get; set; }
    }

    #endregion

    /// <summary>
    ///     回复日志
    /// </summary>
    public class WeiChat_KeyWordReplyLog : ITenantId
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     接受的字符串
        /// </summary>
        [Display(Name = "用户发送内容")]
        public string ReceiveWords { get; set; }

        /// <summary>
        ///     命中的关键字
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "命中的关键字")]
        public string KeyWord { get; set; }

        /// <summary>
        ///     自动回复Id
        /// </summary>
        [Display(Name = "自动回复配置")]
        public Guid? WeiChat_KeyWordAutoReplyId { get; set; }

        /// <summary>
        ///     内容Id
        /// </summary>
        public Guid? ContentId { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "是否成功")]
        public bool IsSuccess { get; set; }

        [Display(Name = "异常日志")]
        public string Error { get; set; }

        [Display(Name = "来自")]
        [MaxLength(50)]
        public string From { get; set; }

        [Display(Name = "To")]
        [MaxLength(50)]
        public string To { get; set; }

        [Display(Name = "EventKey")]
        [MaxLength(200)]
        public string EventKey { get; set; }

        [Display(Name = "MsgId")]
        public long MsgId { get; set; }

        public int TenantId { get; set; }
    }
}