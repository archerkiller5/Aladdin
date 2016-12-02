using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.Product
{
    /// <summary>
    /// 商品评论
    /// </summary>
    public class Product_Comment : WeiChat_TenantBase<Guid>
    {
        [Display(Name = "商品ID")]
        public Guid ProductId { get; set; }

        [Display(Name ="订单ID")]
        public Guid OrderID { get; set; }

        [Display(Name ="OpenId")]
        public string OpenId { get; set; }

        [Display(Name ="评价级别")]
        public CommentLevels CommentLevel { get; set; }

        [Display(Name = "评价内容")]
        public string CommentContent { get; set; }

        [Display(Name ="是否匿名发表")]
        public bool IsAnonymous { get; set; }
        /// <summary>
        /// 对指定的评论进行引用(即回复)
        /// </summary>
        [Display(Name ="引用平率id")]
        public Guid CommentId { get; set; }

    }

    /// <summary>
    /// 评价级别枚举
    /// </summary>
    public enum CommentLevels : int
    {
        [Display(Name = "完美")]
        Gorgeous = 5,
        [Display(Name = "优秀")]
        Good = 4,
        [Display(Name = "中等")]
        Regular = 3,
        [Display(Name = "中差")]
        Poor = 2,
        [Display(Name = "差")]
        Bad = 1
    }
}
