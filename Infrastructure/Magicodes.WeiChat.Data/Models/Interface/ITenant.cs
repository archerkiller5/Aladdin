// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ITenant.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:06
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.WeiChat.Data.Models.Interface
{
    public interface ITenant<Tkey>
    {
        Tkey Id { get; set; }

        /// <summary>
        ///     租户名称
        /// </summary>
        string Name { get; set; }

        string Remark { get; set; }
    }
}