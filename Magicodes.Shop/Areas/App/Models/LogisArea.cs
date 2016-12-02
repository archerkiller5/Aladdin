// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : LogisArea.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Magicodes.Shop.Areas.App.Models
{
    /// <summary>
    ///     微信端前台需要的地区基类
    /// </summary>
    public class BaseArea
    {
        public virtual string name { get; set; }
    }

    /// <summary>
    ///     微信端前台需要的地区类
    /// </summary>
    public class LogisArea : BaseArea
    {
        public override string name
        {
            get { return base.name; }

            set { base.name = value; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<LogisArea> sub { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? type { get; set; }
    }
}