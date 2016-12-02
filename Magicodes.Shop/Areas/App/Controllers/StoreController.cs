// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StoreController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;

namespace Magicodes.Shop.Areas.App.Controllers
{
    /// <summary>
    ///     商城相关
    /// </summary>
    [RouteArea("App")]
    public class StoreController : AppBaseController
    {
        /// <summary>
        ///     商城首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     商品类目
        /// </summary>
        /// <returns></returns>
        public ActionResult Category()
        {
            return View();
        }
    }
}