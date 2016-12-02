// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Logistics_Company.cs
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
    ///     物流公司
    /// </summary>
    public class Logistics_Company : WeiChat_TenantBase<Guid>
    {
        /// <summary>
        ///     公司名称
        /// </summary>
        //[StringLength(maximumLength:200,MinimumLength =2,ErrorMessage ="物流公司名称长度有误!")]
        [Required(ErrorMessage = "{0}不能为空")]
        [MaxLength(200)]
        [MinLength(2)]
        [Display(Name = "公司名称")]
        public string Name { get; set; }

        /// <summary>
        ///     公司备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        ///     快递公司编号(物流接口)
        /// </summary>
        [Display(Name = "快递公司编号(物流接口)")]
        public string ApiCom { get; set; }

        /// <summary>
        ///     是否已删除
        ///     true 是
        ///     flas 否
        /// </summary>
        [Display(Name = "是否删除")]
        public bool IsDelete { get; set; }
    }
}