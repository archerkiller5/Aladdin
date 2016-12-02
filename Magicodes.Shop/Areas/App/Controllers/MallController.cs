// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MallController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:59
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Magicodes.Logger;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Magicodes.Shop.Areas.App.Controllers
{
    public class MallController : AppBaseController
    {
        private readonly string _key = "Product";
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;
        // GET: App/Mall
        public ActionResult Index()
        {
            return View();
        }

        [WeChatOAuth]
        public ActionResult ShoppingCart()
        {
            log.Log(LoggerLevels.Debug, "进入ShoppingCart的action");
            return View();
        }

        [HttpGet]
        public ActionResult OrderGeneratedByAddress(string aa = "")
        {
            var carts = new List<CratRelust>();
            decimal totalPrice = 0;
            if (Session["CartIds"] == null)
            {
                log.Log(LoggerLevels.Debug, "Session未找到CartIds");
                return RedirectToAction("MyAddress", "Personal");
            }
            var CartIds = Session["CartIds"].ToString();
            log.Log(LoggerLevels.Debug, "通过get请求的session获取cartids:" + CartIds);

            if (string.IsNullOrEmpty(CartIds))
                return RedirectToAction("ShoppingCart");
            var CartList = (JArray) JsonConvert.DeserializeObject(CartIds);
            foreach (var cartId in CartList)
            {
                log.Log(LoggerLevels.Debug, "cartId:" + cartId);
                var Cart = db.Cart_Infos.Find(Guid.Parse(cartId.ToString()));


                var Product = (from product in db.Product_Infos
                    join photo in db.Site_Photos on product.Id equals photo.GalleryId
                    where product.Id == Cart.ProductID
                    select new {product.Name, product.Price, product.State, photo.Url}).FirstOrDefault();

                var Package = db.Product_ProductAttributes.FirstOrDefault(o => o.AttributeId == Cart.PackageID);
                var cartRelust = new CratRelust
                {
                    Name = Product.Name,
                    Url = Product.Url,
                    AttributeName = Package.AttributeName,
                    Price = Cart.Price,
                    Quantity = Cart.Quantity
                };

                carts.Add(cartRelust);
                totalPrice += Cart.Quantity*Cart.Price;
            }
            ViewBag.Ids = CartIds;
            ViewBag.Price = totalPrice;
            ViewBag.Result = carts;
            Session[_key] = carts;

            return View("OrderGenerated");
        }


        [HttpPost]
        public ActionResult OrderGenerated()
        {
            var carts = new List<CratRelust>();
            decimal totalPrice = 0;
            var CartIds = Request["CartIds"] ?? string.Empty;
            log.Log(LoggerLevels.Debug, "给Session新增key");
            Session.Add("CartIds", CartIds);
            log.Log(LoggerLevels.Debug, "给Session新增key成功");
            //session.setAttribute("username", username);
            if (string.IsNullOrEmpty(CartIds))
                return RedirectToAction("ShoppingCart");
            var CartList = (JArray) JsonConvert.DeserializeObject(CartIds);
            foreach (var cartId in CartList)
            {
                log.Log(LoggerLevels.Debug, "cartId:" + cartId);
                var Cart = db.Cart_Infos.Find(Guid.Parse(cartId.ToString()));


                var Product = (from product in db.Product_Infos
                    join photo in db.Site_Photos on product.Id equals photo.GalleryId
                    where product.Id == Cart.ProductID
                    select new {product.Name, product.Price, product.State, photo.Url}).FirstOrDefault();

                var Package = db.Product_ProductAttributes.FirstOrDefault(o => o.AttributeId == Cart.PackageID);
                var cartRelust = new CratRelust
                {
                    Name = Product.Name,
                    Url = Product.Url,
                    AttributeName = Package.AttributeName,
                    Price = Cart.Price,
                    Quantity = Cart.Quantity
                };

                carts.Add(cartRelust);
                totalPrice += Cart.Quantity*Cart.Price;
            }
            ViewBag.Ids = CartIds;
            ViewBag.Price = totalPrice;
            ViewBag.Result = carts;
            Session[_key] = carts;
            //ViewBag.Result = carts.Select(v => new
            //{
            //    Url = ,
            //    Product = (from product in db.Product_Infos
            //               join photo in db.Site_Photos on product.Id equals photo.GalleryId
            //               where product.Id == v.ProductID
            //               select new { product.Id, product.Name, product.Price, product.State, photo.Url }).FirstOrDefault(),
            //    Package = db.Product_ProductAttributes.FirstOrDefault(o => o.AttributeId == v.PackageID)
            //});
            //log.Log(LoggerLevels.Debug, JsonConvert.SerializeObject(ViewBag.Result));
            return View();
        }

        public ActionResult CreateOrder()
        {
            var Price = 0.00;
            if (Session[_key] != null)
            {
                ViewBag.Result = Session[_key] as List<CratRelust>;
                foreach (var o in ViewBag.Result)
                {
                    var tempPrice = o.Quantity*o.Price;
                    Price = Price + (double) tempPrice;
                }
            }
            else
            {
                return RedirectToAction("ShoppingCart");
            }
            ViewBag.Price = Price;
            return View();
        }
    }

    public class CratRelust
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public decimal? Price { get; set; }

        public int Quantity { get; set; }

        public string AttributeName { get; set; }
    }
}