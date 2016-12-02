// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Log_Admin.cs
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
using Magicodes.WeiChat.Data.Models.Interface;

namespace Magicodes.WeiChat.Data.Models.Log
{
    /// <summary>
    ///     后台日志基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class Log_AdminBase<TKey> : IAdminCreate<string>, ITenantId
    {
        [Key]
        public TKey Id { get; set; }

        [MaxLength(128)]
        [Display(Name = "创建人")]
        public string CreateBy { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     租户Id
        /// </summary>
        public int TenantId { get; set; }
    }

    /// <summary>
    ///     审计日志
    /// </summary>
    public class Log_Audit : Log_AdminBase<long>
    {
        [Display(Name = "审计唯一编号")]
        public string Code { get; set; }

        [Display(Name = "备注")]
        [MaxLength(500)]
        public string Remark { get; set; }

        [Display(Name = "请求路径")]
        [MaxLength(500)]
        public string RequestUrl { get; set; }

        [Display(Name = "请求数据")]
        public string FormData { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "执行时间")]
        public long ExecutionDuration { get; set; }

        [Display(Name = "客户端IP")]
        public string ClientIpAddress { get; set; }

        [Display(Name = "异常")]
        public string Exception { get; set; }

        [Display(Name = "浏览器信息")]
        public string BrowserInfo { get; set; }

        [Display(Name = "客户端电脑名称")]
        public string ClientName { get; set; }

        [Display(Name = "自定义数据")]
        public string CustomData { get; set; }

        [Display(Name = "是否处理成功")]
        public bool IsSuccess { get; set; }
    }

    /// <summary>
    ///     登录成功
    /// </summary>
    public class Log_LoginSuccess : Log_AdminBase<long>
    {
        [Display(Name = "登录名")]
        public string LoginName { get; set; }

        [Display(Name = "客户端IP")]
        public string ClientIpAddress { get; set; }

        [Display(Name = "浏览器信息")]
        public string BrowserInfo { get; set; }

        [Display(Name = "客户端电脑名称")]
        public string ClientName { get; set; }
    }

    /// <summary>
    ///     登录失败
    /// </summary>
    public class Log_LoginFail : ITenantId
    {
        [Key]
        public long Id { get; set; }
        [Display(Name ="登录名")]
        public string LoginName { get; set; }
        public string Password { get; set; }

        [Display(Name = "客户端IP")]
        public string ClientIpAddress { get; set; }

        [Display(Name = "浏览器信息")]
        public string BrowserInfo { get; set; }

        [Display(Name = "客户端电脑名称")]
        public string ClientName { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     租户Id
        /// </summary>
        public int TenantId { get; set; }
    }
}