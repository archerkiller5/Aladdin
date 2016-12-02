// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SuCai.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.IO;

namespace Magicodes.Shop.Areas.App.Models
{
    public class SuCai
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public Stream InputStream { get; set; }
    }
}