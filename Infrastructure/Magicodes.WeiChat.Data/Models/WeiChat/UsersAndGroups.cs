// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : UsersAndGroups.cs
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
using Magicodes.WeChat.SDK.Apis.User;
using Magicodes.WeiChat.ComponentModel.DataAnnotations;
using Magicodes.WeiChat.Data.Models.Interface;
using System.Collections;
using System.Collections.Generic;

namespace Magicodes.WeiChat.Data.Models
{
    /// <summary>
    ///     微信用户信息
    /// </summary>
    public class WeiChat_User : ITenantId
    {
        /// <summary>
        ///     用户的标识，对当前公众号唯一
        /// </summary>
        [Key]
        [Display(Name = "OpenId")]
        public string OpenId { get; set; }

        /// <summary>
        ///     用户是否订阅该公众号标识
        /// </summary>
        [Display(Name = "是否订阅")]
        public bool Subscribe { get; set; }

        /// <summary>
        ///     用户的昵称
        /// </summary>
        [Display(Name = "昵称")]
        public string NickName { get; set; }

        /// <summary>
        ///     用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        [Display(Name = "性别")]
        public WeChatSexTypes Sex { get; set; }

        /// <summary>
        ///     用户所在城市
        /// </summary>
        [Display(Name = "所在城市")]
        public string City { get; set; }

        /// <summary>
        ///     用户所在国家
        /// </summary>
        [Display(Name = "所在国家")]
        public string Country { get; set; }

        /// <summary>
        ///     用户所在省份
        /// </summary>
        [Display(Name = "所在省份")]
        public string Province { get; set; }

        /// <summary>
        ///     用户的语言，简体中文为zh_CN
        /// </summary>
        [Display(Name = "语言")]
        public string Language { get; set; }

        /// <summary>
        ///     用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        [Display(Name = "头像")]
        public string HeadImgUrl { get; set; }

        /// <summary>
        ///     用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
        /// </summary>
        [Display(Name = "关注时间")]
        public DateTime SubscribeTime { get; set; }

        /// <summary>
        ///     只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。详见：获取用户个人信息（UnionID机制）
        /// </summary>
        [Display(Name = "UnionId")]
        public string UnionId { get; set; }

        /// <summary>
        ///     公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        ///     用户所在的分组ID
        /// </summary>
        [Display(Name = "分组Ids")]
        [ExportIgnore]
        public string GroupIds { get; set; }

        /// <summary>
        ///     所在用户组
        /// </summary>
        [Display(Name = "所在用户组")]
        public ICollection<WeiChat_UserGroup> UserGroups { get; set; }
        //public WeiChat_User()
        //{
        //    //公共默认分组
        //    GroupIds.Add(-1);
        //}

        /// <summary>
        ///     是否允许测试
        /// </summary>
        [Display(Name = "测试用户")]
        [ExportIgnore]
        public bool AllowTest { get; set; }

        [ExportIgnore]
        public int TenantId { get; set; }
        /// <summary>
        /// 用户属于多个组
        /// </summary>
        ///public virtual ICollection<WeiChat_UserGroup> Group { get; set; }
    }

    /// <summary>
    ///     微信用户组信息
    /// </summary>
    public class WeiChat_UserGroup : ITenantId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 分组id，由微信分配
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "分组id")]
        //多对多修改
        public int GroupId { get; set; }

        /// <summary>
        ///     分组名字，UTF8编码
        /// </summary>
        [Display(Name = "组名")]
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        /// <summary>
        ///     分组内用户数量
        /// </summary>
        [Display(Name = "用户数量")]
        public int UsersCount { get; set; }

        public int TenantId { get; set; }
        /// <summary>
        /// 组里有多个用户
        /// </summary>
        //public ICollection<WeiChat_User> WeiChatUsers { get; set; }
    }
}