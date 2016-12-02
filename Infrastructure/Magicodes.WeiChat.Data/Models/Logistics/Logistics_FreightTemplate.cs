// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Logistics_FreightTemplate.cs
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

namespace Magicodes.WeiChat.Data.Models.Logistics
{
    /// <summary>
    ///     运费模板
    /// </summary>
    public class Logistics_FreightTemplate : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     模板名称
        /// </summary>
        [StringLength(200, MinimumLength = 2, ErrorMessage = "运费模板名称长度有误!")]
        [Display(Name = "模板名称")]
        public string Name { get; set; }

        /// <summary>
        ///     物流模板价格
        /// </summary>
        [Range(0, 1000, ErrorMessage = "价格最高不能超过1000元最低不能低于0元!")]
        [Display(Name = "价格")]
        public decimal Price { get; set; }

        /// <summary>
        ///     模板备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        ///     默认运费模板
        /// </summary>
        [Display(Name = "默认运费模板")]
        public bool IsDefault { get; set; }
    }
}