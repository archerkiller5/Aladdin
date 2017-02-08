using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.WeChatStore
{
   public class User_School
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 父级分类
        /// </summary>
        [Display(Name = "父级分类")]
        public int ParentID { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        [Display(Name = "学校名称")]
        public string School_Name { get; set; }
        /// <summary>
        /// 学校地址
        /// </summary>
        [Display(Name = "学校地址")]
        public string School_Address { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateDate { get; set; }
    }
}
