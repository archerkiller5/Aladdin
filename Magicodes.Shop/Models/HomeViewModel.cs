// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : HomeViewModel.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:16
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

namespace Magicodes.Shop.Models
{
    public class HomeUserViewModel
    {
        public string NickName { get; set; }
        public string SubscribeTime { get; set; }
        public string Style { get; set; }
    }

    public class HomeChartViewModel
    {
        public string X { get; set; }
        public string Y { get; set; }
    }
}