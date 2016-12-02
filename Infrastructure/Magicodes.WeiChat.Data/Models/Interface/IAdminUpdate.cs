// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IAdminUpdate.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;

namespace Magicodes.WeiChat.Data.Models.Interface
{
    public interface IAdminUpdate<TUpdateByKey>
    {
        /// <summary>
        ///     创建人
        /// </summary>
        TUpdateByKey UpdateBy { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        DateTime? UpdateTime { get; set; }
    }
}