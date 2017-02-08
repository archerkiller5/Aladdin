using Magicodes.WeiChat.Data.Models.Interface;
using System;
using System.ComponentModel.DataAnnotations;
namespace Magicodes.WeiChat.Data.Models.WeChatStore
{
   public class AppUserInfo : WeiChat_WeChatBase<Guid>,ITenantId
    {
        public int UserNo { get; set; }
        /// <summary>
        /// OpendID
        /// </summary>
        public override string OpenId { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [Display(Name = "用户名")]
        public string Member_loginname { get; set; }
        /// <summary>
        /// 会员密码
        /// </summary>
        [Display(Name = "密码")]
        public string Member_password { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [Display(Name = "昵称")]
        public string Nick_name { get; set; }
        /// <summary>
        /// 会员等级
        /// </summary>
        [Display(Name = "会员等级")]
        public int Member_ID { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        [Display(Name = "真实姓名")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "请输入正确的真实姓名!")]
        public string Real_Name { get; set; }
        /// <summary>
        /// 经验值
        /// </summary>
        [Display(Name = "经验值")]
        public int Empiric_Num { get; set; }
        /// <summary>
        /// 金币值
        /// </summary>
        [Display(Name = "金币值")]
        public int Gold_Num { get; set; }
        /// <summary>
        /// 会员余额
        /// </summary>
        [Display(Name = "余额")]
        public float Balance { get; set; }
        /// <summary>
        /// 服务店铺ID
        /// </summary>
        [Display(Name = "服务店铺ID")]
        public int ServiceShop_ID { get; set; }
        /// <summary>
        /// 会员头像
        /// </summary>
        [Display(Name = "会员头像")]
        public Guid Member_img { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public Enumusersexs Sex { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        [Display(Name = "学校名称")]
        public int School_ID { get; set; }
        /// <summary>
        /// 校区ID
        /// </summary>
        [Display(Name = "校区名称")]
        public string Campus_ID { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        [RegularExpression(@"((/^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$/)|(/^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{4}$/))",ErrorMessage ="您输入的身份证号格式不正确")]
        public string IDNumber { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "电话号码")]
        [RegularExpression(
             @"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)",
             ErrorMessage = "手机号格式不正确")]
        public string Telephone { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        [Display(Name = "出生日期")]
        [RegularExpression(@"/^\s*$|^\d{4}\-\d{1,2}\-\d{1,2}\s{1}(\d{1,2}:){2}\d{1,2}$/",ErrorMessage = "请选择正确的日期!!")]
        public DateTime Birth_Date { get; set; }
        /// <summary>
        /// 会员标签
        /// </summary>
        [Display(Name = "会员标签")]
        public string Member_Label { get; set; }
        /// <summary>
        /// 账号状态
        /// </summary>
        [Display(Name = "账号状态")]
        public EnumUserStates Status { get; set; }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        [Display(Name = "上次登录时间")]
        public string LastLogin_Time { get; set; }
        [Display(Name ="有效地址")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "地址必须明确范围,请重新输入!")]
        public string Adress { get; set; }
    }
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum EnumUserStates : byte
    {
        /// <summary>
        ///     注销 (0)
        /// </summary>
        [Display(Name = "注销")]
        Cancel = 0,
        /// <summary>
        ///     正常 (1)
        /// </summary>
        [Display(Name = "正常")]
        Normal = 1
    }
    public enum Enumusersexs : byte
    {
       [Display (Name ="男")]
       男=0,
        [Display(Name = "女")]
        女 = 1,

    }
}
