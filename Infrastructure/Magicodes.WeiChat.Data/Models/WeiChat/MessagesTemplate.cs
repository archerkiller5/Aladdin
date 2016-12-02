// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MessagesTemplate.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Magicodes.WeiChat.Data.Models.Interface;

namespace Magicodes.WeiChat.Data.Models.WeiChat
{
    /// <summary>
    ///     模板消息模板
    /// </summary>
    public class WeiChat_MessagesTemplate : WeiChat_TenantBase<int>
    {
        /// <summary>
        ///     模板编号
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "模板编号")]
        public string TemplateNo { get; set; }

        [MaxLength(50)]
        [Display(Name = "模板库中模板的编号")]
        public string ShortNo { get; set; }

        /// <summary>
        ///     标题
        /// </summary>
        [Display(Name = "标题")]
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        ///     一级行业
        /// </summary>
        [Display(Name = "一级行业")]
        [MaxLength(50)]
        public string OneIndustry { get; set; }

        /// <summary>
        ///     二级行业
        /// </summary>
        [Display(Name = "二级行业")]
        [MaxLength(50)]
        public string TwoIndustry { get; set; }

        /// <summary>
        ///     模板内容
        /// </summary>
        [Display(Name = "模板内容")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        /// <summary>
        ///     示例内容
        /// </summary>
        [Display(Name = "示例内容")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Demo { get; set; }

        /// <summary>
        ///     是否允许测试
        /// </summary>
        public bool AllowTest { get; set; }
    }

    /// <summary>
    ///     模板消息发送记录
    /// </summary>
    public class WeiChat_MessagesTemplateSendLog : ITenantId, IAdminCreate<string>
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        ///     批次号
        /// </summary>
        [Display(Name = "批次号")]
        public Guid BatchNumber { get; set; }

        /// <summary>
        ///     模板消息Id
        /// </summary>
        [Required]
        public int MessagesTemplateId { get; set; }

        /// <summary>
        ///     模板消息编号
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string MessagesTemplateNo { get; set; }

        /// <summary>
        ///     模板消息
        /// </summary>
        [Display(Name = "模板消息")]
        [ForeignKey("MessagesTemplateId")]
        public WeiChat_MessagesTemplate MessagesTemplate { get; set; }

        /// <summary>
        ///     消息内容
        /// </summary>
        [Display(Name = "消息内容")]
        public string Content { get; set; }

        /// <summary>
        ///     接收人
        /// </summary>
        [MaxLength(50)]
        public string ReceiverId { get; set; }

        /// <summary>
        ///     接收人
        /// </summary>
        [MaxLength(256)]
        [Display(Name = "接收人")]
        [NotMapped]
        public WeiChat_User Receiver { get; set; }

        /// <summary>
        ///     顶部颜色
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "顶部颜色")]
        public string TopColor { get; set; }

        /// <summary>
        ///     链接
        /// </summary>
        [MaxLength(225)]
        [Display(Name = "链接")]
        public string Url { get; set; }

        /// <summary>
        ///     发送结果
        /// </summary>
        [MaxLength(500)]
        [Display(Name = "发送结果")]
        public string Result { get; set; }

        /// <summary>
        ///     是否发送成功
        /// </summary>
        [Display(Name = "是否发送成功")]
        public bool IsSuccess { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Display(Name = "发送时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     创建者
        /// </summary>
        [MaxLength(128)]
        public string CreateBy { get; set; }

        public int TenantId { get; set; }
    }
}