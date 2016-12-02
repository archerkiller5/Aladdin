// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Menu.cs
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

namespace Magicodes.WeiChat.Data.Models.Site
{
    /// <summary>
    ///     站点菜单
    /// </summary>
    public class Site_Menu
    {
        public Site_Menu()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }

        [Display(Name = "父级菜单")]
        public Guid? ParentId { get; set; }

        [Display(Name = "菜单名称")]
        [MaxLength(50)]
        [Required]
        public string Title { get; set; }

        [MaxLength(500)]
        [Display(Name = "菜单地址")]
        public string Url { get; set; }

        [MaxLength(50)]
        [Display(Name = "Controller")]
        public string Controller { get; set; }

        [MaxLength(50)]
        [Display(Name = "Action")]
        public string Action { get; set; }

        [Display(Name = "图标")]
        [MaxLength(50)]
        public string IconCls { get; set; }

        [Display(Name = "路径")]
        [MaxLength(500)]
        public string Path { get; set; }

        [Display(Name = "排序号")]
        public int OrderNo { get; set; }

        /// <summary>
        ///     是否由代码创建
        /// </summary>
        public bool IsCreateByCode { get; set; }

        /// <summary>
        ///     平台
        /// </summary>
        [MaxLength(20)]
        public string Tag { get; set; }
    }

    public enum MenuPlatform
    {
        /// <summary>
        ///     租户平台
        /// </summary>
        Tenant = 0,

        /// <summary>
        ///     系统平台
        /// </summary>
        System = 1
    }

    /// <summary>
    ///     角色菜单
    /// </summary>
    public class Role_Menu
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string RoleId { get; set; }

        public Guid MenuId { get; set; }
    }
}