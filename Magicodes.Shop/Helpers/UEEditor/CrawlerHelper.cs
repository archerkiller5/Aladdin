// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : CrawlerHelper.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Linq;
using System.Web;

namespace Magicodes.Shop.Helpers.UEEditor
{
    public class CrawlerHelper
    {
        private Crawler[] Crawlers;
        private string[] Sources;

        public object Crawle()
        {
            Sources = HttpContext.Current.Request.Form.GetValues("source[]");
            if ((Sources == null) || (Sources.Length == 0))
                return new
                {
                    state = "参数错误：没有指定抓取源"
                };
            Crawlers = Sources.Select(x => new Crawler(x, HttpContext.Current.Server).Fetch()).ToArray();
            return new
            {
                state = "SUCCESS",
                list = Crawlers.Select(x => new
                {
                    state = x.State,
                    source = x.SourceUrl,
                    url = x.ServerUrl
                })
            };
        }
    }
}