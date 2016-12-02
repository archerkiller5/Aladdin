// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_PhotoGallery.cs
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

namespace Magicodes.WeiChat.Data.Models.PhotoGallery
{
    public class Site_PhotoGallery : WeiChat_TenantBase<Guid>
    {
        public Site_PhotoGallery()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        ///     相册名称
        /// </summary>
        [MaxLength(200)]
        [Display(Name = "相册名称")]
        public string Title { get; set; }
    }
}