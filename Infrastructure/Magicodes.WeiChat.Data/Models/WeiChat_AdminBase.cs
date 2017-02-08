// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_AdminBase.cs
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
using System.ComponentModel.DataAnnotations.Schema;
using Magicodes.WeiChat.Data.Models.Interface;
using System.Collections.Generic;

namespace Magicodes.WeiChat.Data.Models
{
    /// <summary>
    ///     全局系统基础类（非租户相关）
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class WeiChat_ApplicationBase<TKey> : IAdminCreate<string>, IAdminUpdate<string>
    {
        [Key]
        public virtual TKey Id { get; set; }

        [MaxLength(128)]
        [Display(Name = "创建人")]
        public string CreateBy { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        [MaxLength(128)]
        [Display(Name = "最后编辑")]
        public string UpdateBy { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }
    }

    public abstract class WeiChat_AdminUniqueTenantBase<TKey> : IAdminCreate<string>, IAdminUpdate<string>, ITenantId
    {
        [Key]
        public TKey Id { get; set; }

        [MaxLength(128)]
        [Display(Name = "创建人")]
        public string CreateBy { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        [MaxLength(128)]
        [Display(Name = "最后编辑")]
        public string UpdateBy { get; set; }

        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        ///     租户Id
        /// </summary>
        [Index(IsUnique = true)]
        public int TenantId { get; set; }
    }

    /// <summary>
    ///     App基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class WeiChat_TenantBase<TKey> : ITenantId, IAdminCreate<string>, IAdminUpdate<string>
    {
        [Key]
        public virtual TKey Id { get; set; }

        /// <summary>
        ///     创建者
        /// </summary>
        [Display(Name = "创建者")]
        //[NotMapped]
        [ForeignKey("CreateBy")]
        public AppUser CreateUser { get; set; }


        /// <summary>
        ///     浏览用户组
        /// </summary>
        [Display(Name = "浏览用户组")]
        //[NotMapped]
        public string UserGroups { get; set; }

        /// <summary>
        ///     编辑者
        /// </summary>
        [MaxLength(256)]
        [Display(Name = "最后编辑")]
        [ForeignKey("UpdateBy")]
        //[NotMapped]
        public AppUser UpdateUser { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     创建者
        /// </summary>
        [MaxLength(128)]
        public string CreateBy { get; set; }

        /// <summary>
        ///     更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        ///     更新者
        /// </summary>
        [MaxLength(128)]
        public string UpdateBy { get; set; }

        public int TenantId { get; set; }
    }
}