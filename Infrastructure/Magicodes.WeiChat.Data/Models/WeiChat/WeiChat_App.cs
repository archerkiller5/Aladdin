// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_App.cs
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
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data.Models.Interface;

namespace Magicodes.WeiChat.Data.Models
{
    /// <summary>
    ///     微信公众号配置
    /// </summary>
    public class WeiChat_App : IWeChatConfig, IAdminCreate<string>, IAdminUpdate<string>, ITenantId
    {
        /// <summary>
        ///     AppId
        /// </summary>
        [MaxLength(50)]
        [Required]
        [Display(Name = "AppId")]
        public string AppId { get; set; }

        /// <summary>
        ///     AppSecret
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Display(Name = "AppSecret")]
        [DataType(DataType.Password)]
        public string AppSecret { get; set; }

        /// <summary>
        ///     微信号
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "微信号")]
        public string WeiXinAccount { get; set; }

        /// <summary>
        ///     版权信息
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "版权信息")]
        public string CopyrightInformation { get; set; }

        /// <summary>
        ///     客户信息
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "客户信息")]
        public string CustomerInformation { get; set; }

        /// <summary>
        ///     填写服务器配置时必须，为了安全，请生成自己的Token。注意：正式公众号的Token只允许英文或数字的组合，长度为3-32字符
        /// </summary>
        [MaxLength(200)]
        public string Token { get; set; }

        /// <summary>
        ///     公众号类型
        /// </summary>
        [Display(Name = "公众号类型")]
        public WeChatAppTypes WeChatAppType { get; set; }


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
        ///     租户Id（使用租户Id作为Key）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TenantId { get; set; }
    }

    /// <summary>
    ///     微信支付配置
    /// </summary>
    public class WeiChat_Pay : WeiChat_AdminUniqueTenantBase<int>, IWeChatPayConfig
    {
        /// <summary>
        ///     与微信商户平台商户MchID一致
        /// </summary>
        [Display(Name = "证书密钥")]
        public string CertPassword { get; set; }

        [Display(Name = "商户Mch_ID")]
        public string MchId { get; set; }

        [Display(Name = "证书相对路径")]
        public string PayCertPath { get; set; }

        [Display(Name = "支付密钥")]
        public string TenPayKey { get; set; }

        [Display(Name = "回调地址")]
        public string Notify { get; set; }
    }

    /// <summary>
    ///     公众号类型
    /// </summary>
    public enum WeChatAppTypes
    {
        [Display(Name = "认证服务号")] Service = 0,
        [Display(Name = "认证订阅号")] Subscription = 1,
        [Display(Name = "企业号")] Enterprise = 2,
        [Display(Name = "测试号")] Test = 3
    }


    /// <summary>
    ///     同步日志
    /// </summary>
    public class WeiChat_SyncLog : ITenantId, IAdminCreate<string>
    {
        public WeiChat_SyncLog()
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        ///     主键Id
        /// </summary>
        [Key]
        [Display(Name = "主键Id")]
        public int Id { get; set; }

        public WeiChat_SyncTypes Type { get; set; }

        /// <summary>
        ///     是否用户手动同步
        /// </summary>
        public bool IsUserSync { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        [Display(Name = "是否成功")]
        public bool Success { get; set; }

        [MaxLength(128)]
        [Display(Name = "创建人")]
        public string CreateBy { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        public int TenantId { get; set; }
    }

    /// <summary>
    ///     多客服信息
    /// </summary>
    public class WeiChat_KFCInfo : WeiChat_TenantBase<int>
    {
        /// <summary>
        ///     客服账号
        /// </summary>
        [Display(Name = "客服账号")]
        public string Account { get; set; }

        /// <summary>
        ///     客服昵称
        /// </summary>
        [Display(Name = "客服昵称")]
        public string NickName { get; set; }

        /// <summary>
        ///     客服工号
        /// </summary>
        [Display(Name = "客服工号")]
        public string JobNumber { get; set; }

        /// <summary>
        ///     客服头像
        /// </summary>
        [Display(Name = "客服头像")]
        public string HeadImgUrl { get; set; }
    }

    public enum WeiChat_SyncTypes
    {
        /// <summary>
        ///     微信用户
        /// </summary>
        SyncWeChatUsersTask = 0,

        /// <summary>
        ///     多客服信息
        /// </summary>
        SyncMKFTask = 1,

        /// <summary>
        ///     图片资源
        /// </summary>
        SyncWeChatImagesTask = 2,

        /// <summary>
        ///     同步用户组
        /// </summary>
        SyncWeChatUserGroupTask = 3,

        /// <summary>
        ///     同步模板消息
        /// </summary>
        SyncMessagesTemplatesTask = 4
    }

    public class WeiChat_Material
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}