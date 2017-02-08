// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppDbContext.Shop.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Data.Entity;
using Magicodes.WeiChat.Data.Models.Activity;
using Magicodes.WeiChat.Data.Models.Advert;
using Magicodes.WeiChat.Data.Models.Log;
using Magicodes.WeiChat.Data.Models.Logistics;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Data.Models.Product;
using Magicodes.WeiChat.Data.Models.Settings;
using Magicodes.WeiChat.Data.Models.Shop;
using Magicodes.WeiChat.Data.Models.User;
using Magicodes.WeiChat.Data.Models.WeChatStore;
using Magicodes.WeiChat.Data.Models.Card;

namespace Magicodes.WeiChat.Data
{
    public partial class AppDbContext
    {
        #region 店铺信息管理

        public DbSet<Shop_Info> Shop_Infos { get; set; }

        #endregion

        #region 订单模块

        public DbSet<Cart_Info> Cart_Infos { get; set; }

        /// <summary>
        ///     订单
        /// </summary>
        public DbSet<Order_Info> Order_Infos { get; set; }

        /// <summary>
        /// </summary>
        public DbSet<Order_Logistics> Order_Logistics { get; set; }

        /// <summary>
        /// </summary>
        public DbSet<Order_Detail> Order_Details { get; set; }

        /// <summary>
        /// </summary>
        public DbSet<Order_Refund> Order_Refunds { get; set; }

        /// <summary>
        /// </summary>
        public DbSet<Order_RefundDetail> Order_RefundDetails { get; set; }

        /// <summary>
        ///     用户收货地址数据源
        /// </summary>
        public DbSet<User_Address> User_Addresses { get; set; }

        #endregion

        #region 会员模块

        /// <summary>
        ///     会员
        /// </summary>
        public DbSet<User_Info> User_Infos { get; set; }

        /// <summary>
        ///     用户手机绑定
        /// </summary>
        public DbSet<User_BindPhone> User_BindPhones { get; set; }

        #endregion

        #region 商品信息相关

        /// <summary>
        ///     商品类目
        /// </summary>
        public DbSet<Product_Category> Product_Categorys { get; set; }

        /// <summary>
        ///     商品标签
        /// </summary>
        public DbSet<Product_Tag> Product_Tags { get; set; }

        /// <summary>
        ///     商品类别
        /// </summary>
        public DbSet<Product_Type> Product_Types { get; set; }

        /// <summary>
        ///     商品属性
        /// </summary>
        public DbSet<Product_Attribute> Product_Attributes { get; set; }

        /// <summary>
        ///     商品信息
        /// </summary>
        public DbSet<Product_Info> Product_Infos { get; set; }

        /// <summary>
        ///     商品属性关联
        /// </summary>
        public DbSet<Product_ProductAttribute> Product_ProductAttributes { get; set; }

        /// <summary>
        ///     商品标签关联
        /// </summary>
        public DbSet<Product_ProductTag> Product_ProductTags { get; set; }

        #endregion

        #region 广告管理

        /// <summary>
        ///     广告位置管理
        /// </summary>
        public DbSet<Advert_Type> Advert_Types { get; set; }

        /// <summary>
        ///     广告图片管理
        /// </summary>
        public DbSet<Advert> Adverts { get; set; }

        #endregion

        #region 物流相关

        public DbSet<Logistics_Company> Logistics_Companys { get; set; }

        public DbSet<Logistics_FreightTemplate> Logistics_FreightTemplates { get; set; }

        public DbSet<Logistics_Area> Logistics_Areas { get; set; }

        #endregion

        #region 系统设置

        public DbSet<Settings_TemplateMessage> Settings_TemplateMessages { get; set; }

        public DbSet<Settings_Point> Settings_Points { get; set; }

        public DbSet<Settings_Withdraw> Settings_Withdraws { get; set; }

        public DbSet<Settings_Product> Settings_Products { get; set; }

        public DbSet<Settings_Shopping> Settings_Shoppings { get; set; }

        public DbSet<Settings_Order> Settings_Orders { get; set; }

        public DbSet<Settings_AliMsg> Settings_AliMsgs { get; set; }

        /// <summary>
        /// 短信模板配置
        /// </summary>
        public DbSet<Settings_SmsTemplate> Settings_SmsTemplates { get; set; }

        #endregion

        #region 日志相关

        /// <summary>
        ///     审计日志
        /// </summary>
        public DbSet<Log_Audit> Log_Audits { get; set; }

        /// <summary>
        ///     登录成功日志
        /// </summary>
        public DbSet<Log_LoginSuccess> Log_LoginSuccess { get; set; }

        /// <summary>
        ///     登录失败日志
        /// </summary>
        public DbSet<Log_LoginFail> Log_LoginFail { get; set; }

        /// <summary>
        ///     积分日志
        /// </summary>
        public DbSet<Log_Point> Log_Points { get; set; }

        /// <summary>
        ///     成员访问日志
        /// </summary>
        public DbSet<Log_MemberAccess> Log_MemberAccess { get; set; }

        /// <summary>
        ///     订单支付日志
        /// </summary>
        public DbSet<Log_Order> Log_Orders { get; set; }

        /// <summary>
        ///     充值日志
        /// </summary>
        public DbSet<Log_Recharge> Log_Recharges { get; set; }

        /// <summary>
        ///     提现日志
        /// </summary>
        public DbSet<Log_Withdraw> Log_Withdraws { get; set; }

        /// <summary>
        ///     资金监控日志
        /// </summary>
        public DbSet<Log_FinancialMonitoring> Log_FinancialMonitorings { get; set; }

        /// <summary>
        ///     红包发送记录
        /// </summary>
        public DbSet<Log_RedPacketSending> Log_RedPacketSendings { get; set; }

        #endregion

        /// <summary>
        /// 抽奖
        /// </summary>
        public DbSet<Activity_Lottery> Activity_Lotteries { get; set; }
        /// <summary>
        /// 个人抽奖记录
        /// </summary>
        public DbSet<Personal_Lottery> Personal_Lotteries { get; set; }
        /// <summary>
        /// 抽奖选项
        /// </summary>
        public DbSet<Activity_LotteryPrizeOption> Activity_LotteryPrizeOptions { get; set; }
        /// <summary>
        /// 商品评论
        /// </summary>
        public DbSet<Product_Comment> Product_Comments { get; set; }

        #region 调查
        /// <summary>
        /// 调查
        /// </summary>
        public DbSet<Activity_Survey> Activity_Surveys { get; set; }
        /// <summary>
        /// 选择题
        /// </summary>
        public DbSet<Activity_SurveyChoiceTopic> Activity_SurveyChoiceTopics { get; set; }
        /// <summary>
        /// 选项
        /// </summary>
        public DbSet<Activity_SurveyOption> Activity_SurveyOptions { get; set; }
        /// <summary>
        /// 参与的调查
        /// </summary>
        public DbSet<Personal_Survey> Personal_Surveys { get; set; }
        /// <summary>
        /// 问卷答案
        /// </summary>
        public DbSet<Personal_SurveyAnswer> Personal_SurveyAnswers { get; set; }
        /// <summary>
        /// 问答题
        /// </summary>
        public DbSet<Activity_EssayQuestionTopic> Activity_EssayQuestionTopics { get; set; }
        #endregion
        #region 投票
        /// <summary>
        /// 投票
        /// </summary>
        public DbSet<Activity_Vote> Activity_Votes { get; set; }
        /// <summary>
        /// 投票选项
        /// </summary>
        public DbSet<Activity_VoteOption> Activity_VoteOptions { get; set; }
        #endregion
        #region 卡券
        /// <summary>
        /// 门店
        /// </summary>
        public DbSet<Store_Info> Store_Info { get; set; }
        /// <summary>
        /// 卡券
        /// </summary>
        public DbSet<Card_CrouponInfo> Card_CrouponInfos { get; set; }
        #endregion
        #region 签到
        /// <summary>
        /// 签到记录
        /// </summary>
        public DbSet<Sign_Record> Sign_Records { get; set; }
        /// <summary>
        /// 签到奖励
        /// </summary>
        public DbSet<Sign_Setup> Sign_Setups { get; set; }
        /// <summary>
        /// 签到奖励选项
        /// </summary>
        public DbSet<Sign_Reward> Sign_RewardOptions { get; set; }
        #endregion
        /// <summary>
        /// 用户学校
        /// </summary>
        public DbSet<User_School> User_Schools { get; set; }
        /// <summary>
        /// 用户表
        /// </summary>
        public DbSet<AppUserInfo> AppUserInfos { get; set; }
    }
}