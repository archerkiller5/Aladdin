// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Settings.cs
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
using Magicodes.WeiChat.ComponentModel.Setting;

namespace Magicodes.WeiChat.Data.Models.Settings
{
    /// <summary>
    ///     设置信息分组
    /// </summary>
    public class App_SettingGroup : ISettingGroup
    {
        public App_SettingGroup()
        {
            Id = Guid.NewGuid();
            UpdateTime = DateTime.Now;
        }

        public App_SettingGroup(string name)
        {
            Name = name;
        }

        public Guid Id { get; set; }

        /// <summary>
        ///     父级Id
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        ///     租户Id
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        ///     用户Id
        /// </summary>
        [MaxLength(128)]
        public string UserId { get; set; }

        /// <summary>
        ///     更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public string CreateBy { get; set; }

        [Required]
        [MaxLength(100)]
        [Index]
        public string Name { get; set; }

        [MaxLength(200)]
        public string DisplayName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        ///     作用范围
        /// </summary>
        public SettingScopes Scopes { get; set; }

        public bool IsVisibleToClients { get; set; }

        public ISettingGroup ParentGroup { get; set; }
    }

    /// <summary>
    ///     设置信息
    /// </summary>
    public class App_SettingValue : ISettingValue
    {
        public App_SettingValue()
        {
            Id = Guid.NewGuid();
            UpdateTime = DateTime.Now;
        }

        public Guid Id { get; set; }

        /// <summary>
        ///     租户Id
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        ///     用户Id
        /// </summary>
        [MaxLength(128)]
        public string UserId { get; set; }

        /// <summary>
        ///     更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public string CreateBy { get; set; }

        /// <summary>
        ///     组Id
        /// </summary>
        public Guid GroupId { get; set; }

        [Required]
        [MaxLength(100)]
        [Index]
        public string Name { get; set; }

        [MaxLength(200)]
        public string DisplayName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        ///     作用范围
        /// </summary>
        public SettingScopes Scopes { get; set; }

        /// <summary>
        ///     自定义数据
        /// </summary>
        public string CustomData { get; set; }

        /// <summary>
        ///     值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     是否允许客户端获取并显示（预留）
        /// </summary>
        public bool IsVisibleToClients { get; set; }
    }
}