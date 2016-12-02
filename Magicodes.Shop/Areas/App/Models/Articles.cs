// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Articles.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using Magicodes.WeiChat.Data.Models.Site;

namespace Magicodes.Shop.Areas.App.Models
{
    public class Articles : Site_Article
    {
        /// <summary>
        ///     ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     分类名
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     当前分类下的文章数
        /// </summary>
        public int Count { get; set; }
    }
}