// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Cart_Info.cs
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

namespace Magicodes.WeiChat.Data.Models.Order
{
    /// <summary>
    ///     购物车
    /// </summary>
    public class Cart_Info : WeiChat_WeChatBase<Guid>
    {
        /// <summary>
        ///     买家ID
        /// </summary>
        [Display(Name = "买家ID")]
        public string UserID { get; set; }

        /// <summary>
        ///     产品ID
        /// </summary>
        [Display(Name = "产品ID")]
        public Guid ProductID { get; set; }

        /// <summary>
        ///     产品套餐ID
        /// </summary>
        [Display(Name = "产品套餐ID")]
        public Guid PackageID { get; set; }

        /// <summary>
        ///     数量
        /// </summary>
        [Display(Name = "数量")]
        public int Quantity { get; set; }

        /// <summary>
        ///     加入价格
        /// </summary>
        [Display(Name = "加入价格")]
        public decimal Price { get; set; }

        /// <summary>
        ///     是否删除
        /// </summary>
        [Display(Name = "是否删除")]
        public bool State { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(512)]
        public string Remark { get; set; }
    }
}