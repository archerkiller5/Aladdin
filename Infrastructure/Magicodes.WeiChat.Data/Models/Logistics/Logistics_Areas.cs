// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Logistics_Areas.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.Logistics
{
    /// <summary>
    ///     物流地区模型
    /// </summary>
    public class Logistics_Area : WeiChat_ApplicationBase<string>
    {
        [MaxLength(20)]
        [Key]
        public override string Id { get; set; }

        ///// <summary>
        ///// 地区代码
        ///// </summary>
        //[Display(Name = "地区代码")]
        //public string AreaCode { get; set; }
        /// <summary>
        ///     地区名称
        /// </summary>
        [Display(Name = "地区名称")]
        public string AreaName { get; set; }

        /// <summary>
        ///     地区级别
        /// </summary>
        [Display(Name = "地区级别")]
        public Logistics_AreaLevel AreaLevel { get; set; }

        /// <summary>
        ///     父级地区代码
        /// </summary>
        [Display(Name = "父级地区")]
        public string ParentId { get; set; }

        /// <summary>
        ///     拼音码
        /// </summary>
        [Display(Name = "拼音码")]
        public string Pinyinma { get; set; }

        /// <summary>
        ///     邮政编码
        /// </summary>
        [Display(Name = "邮政编码")]
        public string PostCode { get; set; }

        /// <summary>
        ///     排序编号
        /// </summary>
        [Display(Name = "排序编号")]
        public int SortNumber { get; set; }
    }

    /// <summary>
    ///     物流地区级别枚举
    /// </summary>
    public enum Logistics_AreaLevel
    {
        /// <summary>
        ///     国家
        /// </summary>
        [Display(Name = "国家")] Country = 0,

        /// <summary>
        ///     省、直辖市
        /// </summary>
        [Display(Name = "省、直辖市")] Province = 1,

        /// <summary>
        ///     市
        /// </summary>
        [Display(Name = "市")] City = 2,

        /// <summary>
        ///     县、区
        /// </summary>
        [Display(Name = "县、区")] Zone = 3
    }
}