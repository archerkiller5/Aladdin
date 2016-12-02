// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IAdminCreate.cs
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
    public interface IAdminCreate<TCreateByKey>
    {
        /// <summary>
        ///     创建人
        /// </summary>
        TCreateByKey CreateBy { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        DateTime CreateTime { get; set; }
    }
}