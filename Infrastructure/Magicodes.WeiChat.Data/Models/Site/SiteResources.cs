// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SiteResources.cs
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

namespace Magicodes.WeiChat.Data.Models.Site
{
    public class Site_ResourceBase : WeiChat_TenantBase<Guid>
    {
        public Site_ResourceBase()
        {
            Id = Guid.NewGuid();
        }
    }

    public class Site_FileBase : Site_ResourceBase
    {
        public Site_FileBase()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        ///     类型Id
        /// </summary>
        [Display(Name = "标签")]
        public Guid ResourcesTypeId { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        [Required]
        [MaxLength(225)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [MaxLength(500)]
        [Display(Name = "站内地址")]
        public virtual string SiteUrl { get; set; }

        [MaxLength(500)]
        [Display(Name = "地址")]
        public virtual string Url { get; set; }
    }

    /// <summary>
    ///     站点资源类型
    /// </summary>
    public class Site_ResourceType : Site_ResourceBase
    {
        /// <summary>
        ///     标题（名称）
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "名称")]
        [Required]
        public string Title { get; set; }

        /// <summary>
        ///     站点资源类型
        /// </summary>
        [Display(Name = "类型")]
        public SiteResourceTypes ResourceType { get; set; }

        /// <summary>
        ///     是否为系统资源
        /// </summary>
        public bool IsSystemResource { get; set; }

        /// <summary>
        ///     封面
        /// </summary>
        [NotMapped]
        public string Cover { get; set; }
    }

    /// <summary>
    ///     站点图片
    /// </summary>
    public class Site_Image : Site_FileBase
    {
        /// <summary>
        ///     微信媒体Id
        /// </summary>
        [MaxLength(50)]
        public string MediaId { get; set; }

        /// <summary>
        ///     是否封面
        /// </summary>
        public bool IsFrontCover { get; set; }
    }

    /// <summary>
    ///     语音
    /// </summary>
    public class Site_Voice : Site_FileBase
    {
        /// <summary>
        ///     微信媒体Id
        /// </summary>
        [MaxLength(50)]
        public string MediaId { get; set; }
    }

    /// <summary>
    ///     视频
    /// </summary>
    public class Site_Video : Site_FileBase
    {
        /// <summary>
        ///     微信媒体Id
        /// </summary>
        [MaxLength(50)]
        public string MediaId { get; set; }
    }

    /// <summary>
    ///     站点资源类型
    /// </summary>
    public enum SiteResourceTypes
    {
        /// <summary>
        ///     相册
        /// </summary>
        [Display(Name = "相册")] Gallery = 0,

        /// <summary>
        ///     语音
        /// </summary>
        [Display(Name = "语音")] Voice = 1,
        [Display(Name = "视频")] Video = 2,
        //[Display(Name = "缩略图")]
        //Thumb = 3,
        [Display(Name = "文章")] Article = 4,
        [Display(Name = "多图文")] News = 5
    }
    
    /// <summary>
    ///     文章
    /// </summary>
    public class Site_Article : Site_FileBase
    {
        /// <summary>
        ///     内容
        /// </summary>
        [Display(Name = "内容")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "作者")]
        public string Author { get; set; }

        [Display(Name = "摘要")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        [MaxLength(500)]
        [Display(Name = "封面")]
        public string FrontCoverImageUrl { get; set; }

        [MaxLength(500)]
        [Display(Name = "链接")]
        public override string Url { get; set; }

        [MaxLength(500)]
        [Display(Name = "站内链接")]
        public override string SiteUrl { get; set; }

        [MaxLength(500)]
        [Display(Name = "原文链接")]
        public string OriginalUrl { get; set; }

        [MaxLength(500)]
        [Display(Name = "图片集合")]
        public List<Site_Image> site_images { get; set; }

    }


    /// <summary>
    ///     文章
    /// </summary>
    public class Site_Articlea : Site_FileBase
    {
        /// <summary>
        ///     内容
        /// </summary>
        [Display(Name = "内容")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "作者")]
        public string Author { get; set; }

        [Display(Name = "摘要")]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string Summary { get; set; }

        [MaxLength(500)]
        [Display(Name = "封面")]
        public string FrontCoverImageUrl { get; set; }



        [MaxLength(500)]
        [Display(Name = "链接")]
        public override string Url { get; set; }

        [MaxLength(500)]
        [Display(Name = "站内链接")]
        public override string SiteUrl { get; set; }

        [MaxLength(500)]
        [Display(Name = "原文链接")]
        public string OriginalUrl { get; set; }
    }
    /// <summary>
    ///     多图文
    /// </summary>
    public class Site_News : Site_FileBase
    {
        public Site_News()
        {
            Articles = new List<Site_NewsArticle>();
        }

        /// <summary>
        ///     文章列表
        /// </summary>
        public virtual ICollection<Site_NewsArticle> Articles { get; set; }

        /// <summary>
        ///     微信媒体Id
        /// </summary>
        [MaxLength(50)]
        public string MediaId { get; set; }

        [MaxLength(500)]
        [Display(Name = "封面")]
        public string FrontCoverImageUrl { get; set; }
    }

    /// <summary>
    ///     多图文文章
    /// </summary>
    public class Site_NewsArticle : WeiChat_TenantBase<long>
    {
        [Display(Name = "文章")]
        public Guid SiteArticleId { get; set; }

        [Display(Name = "文章")]
        [ForeignKey("SiteArticleId")]
        public Site_Article Article { get; set; }

        [Display(Name = "封面图片显示在正文中")]
        public bool IsShowInText { get; set; }

        [Display(Name = "封面")]
        [ForeignKey("SiteImageId")]
        public Site_Image FrontCoverImage { get; set; }

        [Display(Name = "封面")]
        public Guid SiteImageId { get; set; }
    }

}