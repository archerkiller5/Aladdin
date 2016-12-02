// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : CommonModel.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:16
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Magicodes.Shop.Models
{
    /// <summary>
    ///     通用业务模型基类
    /// </summary>
    [Description("通用业务模型基类")]
    [Serializable]
    public class CommonBusinessModelBase<TKey, TUserKeyType>
    {
        public CommonBusinessModelBase()
        {
            CreateTime = DateTime.Now;
            Deleted = false;
        }

        /// <summary>
        ///     主键Id
        /// </summary>
        [Key]
        [Display(Name = "主键Id")]
        public virtual TKey Id { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        ///     修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        public virtual DateTime? UpdateTime { get; set; }

        /// <summary>
        ///     是否删除
        /// </summary>
        [Display(Name = "是否删除")]
        public virtual bool Deleted { get; set; }

        /// <summary>
        ///     创建人
        /// </summary>
        [Display(Name = "创建人")]
        public virtual TUserKeyType CreateBy { get; set; }

        /// <summary>
        ///     更新人
        /// </summary>
        [Display(Name = "更新人")]
        public virtual TUserKeyType UpdateBy { get; set; }
    }

    public class MessageInfo
    {
        /// <summary>
        ///     消息类型
        /// </summary>
        public MessageTypes MessageType { get; set; }

        /// <summary>
        ///     消息
        /// </summary>
        public string Message { get; set; }
    }

    public enum MessageTypes
    {
        Success,
        Info,
        Warning,
        Danger
    }
}