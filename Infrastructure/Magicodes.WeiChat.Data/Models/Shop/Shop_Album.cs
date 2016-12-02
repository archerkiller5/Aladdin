// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Shop_Album.cs
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
    ///     店铺相册管理
    /// </summary>
    public class Shop_Album
    {
        /// <summary>
        ///     相册所属店铺
        /// </summary>
        [Required]
        [Display(Name = "所属店铺")]
        public Guid ShopId { get; set; }

        /// <summary>
        ///     店铺相册名称
        /// </summary>
        [MaxLength(200)]
        [Required]
        [Display(Name = "店铺相册名称")]
        public string AlbumName { get; set; }

        /// <summary>
        ///     店铺相册排序
        /// </summary>
        [Display(Name = "相册排序")]
        public int AlbumSort { get; set; }

        /// <summary>
        ///     相册是否显示
        /// </summary>
        [Display(Name = "是否显示")]
        public bool IsDisplay { get; set; }
    }
}