// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Setting_AliMsg.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;

namespace Magicodes.WeiChat.Data.Models.Settings
{
    /// <summary>
    ///     阿里短信服务配置
    /// </summary>
    public class Settings_AliMsg : WeiChat_AdminUniqueTenantBase<int>
    {
        /// <summary>
        ///     接口所需AppKey
        /// </summary>
        [Display(Name = "接口所需AppKey")]
        public string AppKey { get; set; }

        /// <summary>
        ///     接口密钥
        /// </summary>
        [Display(Name = "接口密钥")]
        public string Secret { get; set; }
    }
}