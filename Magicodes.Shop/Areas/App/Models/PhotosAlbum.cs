// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : PhotosAlbum.cs
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
    public class PhotosAlbum
    {
        public Guid Id { get; set; }
        public SiteResourceTypes ResourceType { get; set; }
        public bool IsSystemResource { get; set; }
        public string Title { get; set; }
        public string SiteUrl { get; set; }
    }
}