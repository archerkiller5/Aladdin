// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_ResourcesViewModel.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:16
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Magicodes.WeiChat.Data.Models.Site;

namespace Magicodes.Shop.Models
{
    public class GetJsonDataByMediaIdViewModel
    {
        public Site_FileBase FileBase { get; set; }
        public SiteResourceTypes ResourceType { get; set; }
    }
}