// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ProductController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Web.Mvc;
using Magicodes.WeiChat.Domain.Product;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;

namespace Magicodes.Shop.Areas.App.Controllers
{
    /// <summary>
    ///     产品相关
    /// </summary>
    [RouteArea("App")]
    public class ProductController : AppBaseController
    {
        /// <summary>
        ///     商品类目
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductCategory()
        {
            return View();
        }

        /// <summary>
        ///     商品列表页
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductList(string tagType)
        {
            ViewBag.TagType = tagType;
            return View();
        }

        /// <summary>
        ///     商品详情页
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [WeChatOAuth]
        public ActionResult ProductDetail(Guid productId)
        {
            var pdo = new ProductDO();
            var productInfo = pdo.GetProductInfo(productId);
            return View(productInfo);
        }

        [WeChatOAuth]
        public ActionResult ProductComment(Guid orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}