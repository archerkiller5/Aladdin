using Magicodes.WeChat.SDK.Apis.Card;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.WeiChat.Data.Models.Card
{
    public class Card_CrouponInfo : WeiChat_TenantBase<int>
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [MaxLength(18)]
        //[RegularExpression("^[\u4e00-\u9fa5]{0,9}$", ErrorMessage = "卡券名字数上限为9个汉字，建议涵盖卡券属性、服务及金额")]
        [Display(Name = "名称")]
        public string Title { get; set; }

        /// <summary>
        /// 卡券类型
        /// </summary>
        [Display(Name = "卡券类型")]
        [Required]
        public CardTypes CardType { get; set; }

        [Display(Name = "卡券设置")]
        [Required]
        public string Data { get; set; }
    }
}
