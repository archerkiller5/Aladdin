// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_Photo.cs
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
using Magicodes.WeiChat.Data.Models.Interface;

namespace Magicodes.WeiChat.Data.Models.PhotoGallery
{
    /// <summary>
    ///     图片
    /// </summary>
    public class Site_Photo : WeiChat_TenantBase<Guid>, IDeleted
    {
        public Site_Photo()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        ///     相册ID
        /// </summary>
        [Required]
        [Display(Name = "相册ID")]
        public Guid GalleryId { get; set; }

        /// <summary>
        ///     图片
        /// </summary>
        [Required]
        [MaxLength(225)]
        [Display(Name = "图片")]
        public string FileName { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "图片地址")]
        public string Url { get; set; }

        public bool IsDeleted { get; set; }
    }
}