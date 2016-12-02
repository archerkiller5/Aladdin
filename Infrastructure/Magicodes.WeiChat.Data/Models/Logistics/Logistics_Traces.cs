// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Logistics_Traces.cs
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
    ///     物流追溯(接口标识数据不会再有更新后需保存到服务器数据库)
    /// </summary>
    public class Logistics_Traces : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     订单id
        /// </summary>
        [Required]
        [Display(Name = "订单ID")]
        public Guid OrderId { get; set; }

        /// <summary>
        ///     公司名称(接口返回)
        /// </summary>
        [Required]
        [Display(Name = "公司")]
        public string Company { get; set; }

        /// <summary>
        ///     公司编号(接口返回)
        /// </summary>
        [Required]
        [Display(Name = "公司编号")]
        public string Com { get; set; }

        /// <summary>
        ///     快递单号
        /// </summary>
        [Required]
        [Display(Name = "快递单号")]
        public string ExpressNo { get; set; }

        /// <summary>
        ///     物流记录时间
        /// </summary>
        [Required]
        [Display(Name = "时间")]
        public DateTime ExpressDateTime { get; set; }

        /// <summary>
        ///     物流说明信息
        /// </summary>
        [Display(Name = "说明")]
        public string Remark { get; set; }

        /// <summary>
        ///     物流抵达地区
        /// </summary>
        [Display(Name = "地区")]
        public string Zone { get; set; }
    }
}