// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Advert_Type.cs
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

namespace Magicodes.WeiChat.Data.Models.Advert
{
    public class Advert_Type : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     位置名称
        /// </summary>
        [Required]
        [Display(Name = "位置名称")]
        public string Name { get; set; }

        /// <summary>
        ///     宽度
        /// </summary>
        [Display(Name = "宽度")]
        public float Width { get; set; }

        /// <summary>
        ///     高度
        /// </summary>
        [Display(Name = "高度")]
        public float Height { get; set; }

        /// <summary>
        ///     是否系统相册
        /// </summary>
        [Display(Name = "是否系统相册")]
        public bool IsSystem { get; set; }
    }
}