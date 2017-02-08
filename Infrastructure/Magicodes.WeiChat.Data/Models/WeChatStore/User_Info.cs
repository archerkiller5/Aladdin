// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : User_Info.cs
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

namespace Magicodes.WeiChat.Data.Models.User
{
    /// <summary>
    ///     会员信息
    /// </summary>
    public class User_Info : WeiChat_WeChatWithNoKeyBase
    {
        /// <summary>
        ///     OPENID
        /// </summary>
        [Key]
        public override string OpenId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserNo { get; set; }

        /// <summary>
        ///     电子邮箱
        /// </summary>
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        ///     手机号
        /// </summary>
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        /// <summary>
        ///     状态(0:注销;1:正常)
        /// </summary>
        [Display(Name = "状态")]
        public EnumUserState State { get; set; }

        /// <summary>
        ///     积分
        /// </summary>
        [Display(Name = "积分")]
        public int Integral { get; set; }

        /// <summary>
        ///     余额
        /// </summary>
        [Display(Name = "余额")]
        public decimal Balance { get; set; }

        /// <summary>
        ///     最后登录时间
        /// </summary>
        [Display(Name = "最后登录时间")]
        public DateTime LastLoginOn { get; set; }

        /// <summary>
        ///     登陆次数
        /// </summary>
        [Display(Name = "登陆次数")]
        public int LoginCount { get; set; }

        [Display(Name = "账号")]
        public string  Userid { get; set; }

        [Display(Name = "密码")]
        public string Pwd { get; set; }

        [Display(Name = "昵称")]
        public string NickName { get; set; }

        /// <summary>
        ///     真实姓名 在手机认证时候回填
        /// </summary>
        [Display(Name = "真实姓名")]
        public string TrueName { get; set; }
 

        [Display(Name = "身份证号")]
        public string IdCard { get; set; }

        [Display(Name = "住址")]
        public string Address { get; set; }

        /// <summary>
        ///     用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        [Display(Name = "性别")]
        public WeChat.SDK.Apis.User.WeChatSexTypes Sex { get; set; }
        
        [Display(Name = "工作单位")]
        public string WorkPlace_1 { get; set; }

        [Display(Name = "经营范围")]
        public string Business_scope_1 { get; set; }

        [Display(Name = "联系电话")]
        public string Tel_1 { get; set; }

        [Display(Name = "工作单位")]
        public string WorkPlace_2 { get; set; }

        [Display(Name = "经营范围")]
        public string Business_scope_2 { get; set; }

        [Display(Name = "联系电话")]
        public string Tel_2 { get; set; }

        [Display(Name = "工作单位")]
        public string WorkPlace_3 { get; set; }

        [Display(Name = "经营范围")]
        public string Business_scope_3 { get; set; }

        [Display(Name = "联系电话")]
        public string Tel_3 { get; set; }

        [Display(Name = "公开字段")]
        public string Open_Field { get; set; }

    }

    #region 用户状态

    /// <summary>
    ///     状态 : byte
    ///     <remarks>(0:注销;1:正常)</remarks>
    /// </summary>
    public enum EnumUserState : byte
    {
        /// <summary>
        ///     注销 (0)
        /// </summary>
        [Display(Name = "注销")] Cancel = 0,

        /// <summary>
        ///     正常 (1)
        /// </summary>
        [Display(Name = "正常")] Normal = 1
    }

    #endregion
}