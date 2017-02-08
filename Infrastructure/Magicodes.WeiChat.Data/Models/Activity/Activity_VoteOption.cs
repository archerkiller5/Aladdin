using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.Activity
{
    /// <summary>
    /// 投票选项
    /// </summary>
   public class Activity_VoteOption

    {
        [Key]
        public int Id { get; set; }
        //记录VoteID
        public int? VoteId { get; set; }
        [Display(Name = "选项文本")]
        [MaxLength(500)]
        public string OptionText { get; set; }
        [Display(Name = "序号")]
        public int Order { get; set; }
    }
}
