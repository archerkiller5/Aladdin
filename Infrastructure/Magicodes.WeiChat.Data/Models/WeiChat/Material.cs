// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Material.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Magicodes.WeiChat.Data.Models.WeiChat
{
    /// <summary>
    ///     图文消息素材
    /// </summary>
    public class Material_News
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        ///     文件名称
        /// </summary>
        public string Title
        {
            get { return Articles.First().title; }
        }

        /// <summary>
        ///     图文页的URL，或者，当获取的列表是图片素材列表时，该字段是图片的URL
        /// </summary>
        public string Url
        {
            get { return Articles.First().url; }
        }

        public List<Material_Article> Articles { get; set; }
    }

    public class Material_Article
    {
        /// <summary>
        ///     图文消息缩略图的media_id，可以在基础支持上传多媒体文件接口中获得
        /// </summary>
        public string thumb_media_id { get; set; }

        /// <summary>
        ///     图文消息的作者
        /// </summary>
        public string author { get; set; }

        /// <summary>
        ///     图文消息的标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        ///     在图文消息页面点击“阅读原文”后的页面
        /// </summary>
        public string content_source_url { get; set; }

        public string url { get; set; }

        /// <summary>
        ///     图文消息页面的内容，支持HTML标签
        /// </summary>
        public string content { get; set; }

        /// <summary>
        ///     图文消息的描述
        /// </summary>
        public string digest { get; set; }

        /// <summary>
        ///     是否显示封面，1为显示，0为不显示
        /// </summary>
        public string show_cover_pic { get; set; }
    }
}