// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MessageViewModel.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:16
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.Shop.Models
{
    public class SendMessageViewModel
    {
        /// <summary>
        ///     群发到的分组的group_id，参加用户管理中用户分组接口，若is_to_all值为true，可不填写group_id
        /// </summary>
        [Display(Name = "用户组")]
        public string GroupId { get; set; }

        /// <summary>
        ///     用于群发的消息的media_id
        /// </summary>
        [Display(Name = "素材")]
        [Required]
        public string MediaId { get; set; }

        /// <summary>
        ///     发送给所有人
        /// </summary>
        [Display(Name = "发给所有人")]
        public bool IsToAll { get; set; }
    }
}