// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MaterialViewModel.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:16
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Shop.Models
{
    public class DataPageListViewModel<T> where T : class
    {
        public DataPageListViewModel(IEnumerable<T> dataRows, int currentPageIndex, int pageSize, int totalItemCount)
        {
            DataRows = dataRows;
            CurrentPageIndex = currentPageIndex;
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
        }

        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public IEnumerable<T> DataRows { get; set; }
    }

    public class MaterialViewModel
    {
        public string Id { get; set; }

        /// <summary>
        ///     文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     这篇图文消息素材的最后更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///     图文页的URL，或者，当获取的列表是图片素材列表时，该字段是图片的URL
        /// </summary>
        public string Url { get; set; }
    }

    public class AddMessagePostViewModel
    {
        public List<AddMessageViewModel> Messages { get; set; }
    }

    public class AddMessageViewModel
    {
        /// <summary>
        ///     标题
        /// </summary>
        [MaxLength(64)]
        [Display(Name = "标题")]
        [Required]
        public string Title { get; set; }

        /// <summary>
        ///     作者
        /// </summary>
        [MaxLength(8)]
        [Display(Name = "作者（选填）")]
        public string Author { get; set; }

        /// <summary>
        ///     是否在正文中显示封面
        /// </summary>
        [Display(Name = "封面图片显示在正文中")]
        public bool IsShowInText { get; set; }

        /// <summary>
        ///     摘要
        /// </summary>
        [MaxLength(120)]
        [Display(Name = "摘要（选填，该摘要只在发送图文消息为单条时显示）")]
        public string Summary { get; set; }

        /// <summary>
        ///     正文
        /// </summary>
        [Display(Name = "正文")]
        public string Text { get; set; }

        /// <summary>
        ///     原文链接（选填）
        /// </summary>
        [Display(Name = "原文链接（选填）")]
        public string OriginalLink { get; set; }

        /// <summary>
        ///     图文消息缩略图的media_id，可以在基础支持上传多媒体文件接口中获得
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "请选择封面图片")]
        [Display(Name = "封面图片")]
        public string ThumbMediaId { get; set; }
    }

    /// <summary>
    ///     图文消息模型
    /// </summary>
    public class MaterialNewsViewModel
    {
        public string Id { get; set; }
        public string ThumbMediaId { get; set; }

        /// <summary>
        ///     图文消息的标题
        /// </summary>
        public string Title { get; set; }

        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///     图文消息的描述
        /// </summary>
        public string Digest { get; set; }

        public string Url { get; set; }
    }

    public class PostNewsViewModel
    {
        /// <summary>
        ///     标题
        /// </summary>
        [MaxLength(64)]
        [Display(Name = "标题")]
        [Required]
        public string Title { get; set; }

        /// <summary>
        ///     作者
        /// </summary>
        [MaxLength(8)]
        [Display(Name = "作者（选填）")]
        public string Author { get; set; }

        /// <summary>
        ///     是否在正文中显示封面
        /// </summary>
        [Display(Name = "封面图片显示在正文中")]
        public bool IsShowInText { get; set; }

        /// <summary>
        ///     摘要
        /// </summary>
        [MaxLength(120)]
        [Display(Name = "摘要（选填，该摘要只在发送图文消息为单条时显示）")]
        public string Summary { get; set; }

        /// <summary>
        ///     正文
        /// </summary>
        [Required]
        [Display(Name = "正文")]
        public string Text { get; set; }

        /// <summary>
        ///     原文链接（选填）
        /// </summary>
        [Display(Name = "原文链接（选填）")]
        public string OriginalLink { get; set; }

        /// <summary>
        ///     图文消息缩略图的media_id，可以在基础支持上传多媒体文件接口中获得
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "请选择封面图片")]
        [Display(Name = "封面图片")]
        public string ThumbMediaId { get; set; }
    }
}