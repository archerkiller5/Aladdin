// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Shop_Photo.cs
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

namespace Magicodes.WeiChat.Data.Models.Shop
{
    /// <summary>
    ///     店铺相片管理
    /// </summary>
    public class Shop_Photo
    {
        /// <summary>
        ///     所属相册
        /// </summary>
        public Guid AlbumId { get; set; }

        /// <summary>
        ///     相片名称
        /// </summary>
        [MaxLength(100)]
        [Required]
        [Display(Name = "相片名称")]
        public string PhotoName { get; set; }

        /// <summary>
        ///     相片路径
        /// </summary>
        public string PhotoUrl { get; set; }
    }
}