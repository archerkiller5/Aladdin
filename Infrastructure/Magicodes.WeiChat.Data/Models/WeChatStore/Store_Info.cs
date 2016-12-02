using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.WeChatStore
{
    /// <summary>
    /// 门店
    /// </summary>
    public class Store_Info : WeiChat_TenantBase<int>
    {
        /// <summary>
        /// 门店名称
        /// </summary>
        [MaxLength(15)]
        [Required]
        [Display(Name = "门店名称")]
        public string Name { get; set; }
        /// <summary>
        /// 分店名
        /// </summary>
        [MaxLength(10)]
        [Display(Name = "分店名")]
        public string BranchName { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        [MaxLength(10)]
        [Required]
        [Display(Name = "省")]
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [MaxLength(30)]
        [Required]
        [Display(Name = "市")]
        public string City { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        [Display(Name = "区")]
        [MaxLength(10)]
        [Required]
        public string District { get; set; }
        /// <summary>
        /// 详细街道地址
        /// </summary>
        [MaxLength(80)]
        [Required]
        [Display(Name = "详细街道地址")]
        public string Address { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        [MaxLength(53)]
        [Required]
        [Display(Name = "电话号码")]
        [DataType(DataType.PhoneNumber)]
        public string Telephone { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        [MaxLength(50)]
        [Required]
        [Display(Name = "类别")]
        public string Categorys { get; set; }
        /// <summary>
        /// 门店所在地理位置的经度
        /// </summary>
        [Required]
        public double Longitude { get; set; }
        /// <summary>
        /// 门店所在地理位置的纬度（经纬度均为火星坐标，最好选用腾讯地图标记的坐标）
        /// </summary>
        [Required]
        public double Latitude { get; set; }
        /// <summary>
        /// 推荐
        /// </summary>
        [MaxLength(200)]
        [Display(Name = "推荐")]
        [DataType(DataType.MultilineText)]
        public string Recommend { get; set; }
        /// <summary>
        /// 特色服务
        /// </summary>
        [MaxLength(200)]
        [Display(Name = "特色服务")]
        [DataType(DataType.MultilineText)]
        public string Special { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [MaxLength(300)]
        [Display(Name = "简介")]
        [DataType(DataType.MultilineText)]
        public string Introduction { get; set; }
        /// <summary>
        /// 营业时间
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "营业时间")]
        public string OpenTime { get; set; }
        /// <summary>
        /// 人均价格
        /// </summary>
        [Range(0, 100000)]
        [Display(Name = "人均价格")]
        public int AvgPrice { get; set; }
        /// <summary>
        /// 店铺图片
        /// </summary>
        public string PhotoList { get; set; }
        /// <summary>
        /// 是否已经审核通过
        /// </summary>
        [Display(Name = "是否已审核")]
        public bool HasApproved { get; set; }
        /// <summary>
        /// 业务Id
        /// </summary>
        [MaxLength(50)]
        public string SID { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        [MaxLength(50)]
        public string PoiId { get; set; }
    }
}
