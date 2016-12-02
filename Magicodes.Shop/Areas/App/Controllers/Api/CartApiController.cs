// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : CartApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web.Http;
using Magicodes.Logger;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Infrastructure;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/Cart")]
    public class CartApiController : TenantBaseApiController<Order_Info>
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;

        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get()
        {
            try
            {
                var carts =
                    db.Cart_Infos.Where(
                            o => !o.State && (o.OpenId == WeiChatApplicationContext.Current.WeiChatUser.OpenId))
                        .OrderByDescending(o => o.CreateTime)
                        .ToList();
                var val = carts.ConvertAll(v => new
                {
                    Cart = v,
                    Product = (from product in db.Product_Infos
                        join photo in db.Site_Photos on product.Id equals photo.GalleryId
                        where product.Id == v.ProductID
                        select new {product.Id, product.Name, product.Price, product.State, photo.Url}).FirstOrDefault(),
                    Package = db.Product_ProductAttributes.FirstOrDefault(o => o.AttributeId == v.PackageID)
                });
                return Ok(val);
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Error, "Cart：" + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     加入购物车
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add/{ProductId}/{AttributeId}")]
        public IHttpActionResult Add(Guid ProductId, Guid AttributeId)
        {
            try
            {
                var openId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                log.Log(LoggerLevels.Debug, string.Format("Cart/Add：{0},{1},{2}", openId, AttributeId, ProductId));
                var cart =
                    db.Cart_Infos.FirstOrDefault(
                        o =>
                            (o.ProductID == ProductId) && (o.PackageID == AttributeId) && !o.State &&
                            (o.UserID == openId));

                if (cart != null)
                {
                    cart.Quantity++;
                }
                else
                {
                    var package =
                        db.Product_ProductAttributes.FirstOrDefault(
                            o => (o.AttributeId == AttributeId) && (o.ProductId == ProductId));
                    if (package == null)
                        return BadRequest("商品属性不存在！");
                    cart = new Cart_Info
                    {
                        Id = Guid.NewGuid(),
                        UserID = openId,
                        ProductID = ProductId,
                        PackageID = AttributeId,
                        Quantity = 1,
                        Price = package.AttributePrice,
                        State = false,
                        TenantId = TenantId,
                        OpenId = openId,
                        CreateTime = DateTime.Now
                    };
                    db.Cart_Infos.Add(cart);
                }
                db.SaveChanges();
                //return Ok(new { success = true, message = "加入购物车成功！", ID = cart.Id });
                return Ok();
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Error, "Cart/Add：" + ex);
                return BadRequest("操作失败，请稍后重试！");
            }
        }

        /// <summary>
        ///     修改购物车数量
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SetNum/{id}/{num}")]
        public IHttpActionResult Cart(Guid id, int num)
        {
            try
            {
                var openId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                var cart = db.Cart_Infos.FirstOrDefault(o => (o.Id == id) && !o.State && (o.OpenId == openId));

                if (cart != null)
                {
                    cart.Quantity = num;
                    ;
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Error, "Cart/SetNum：" + ex.Message);
                return BadRequest("操作失败，请稍后重试！");
            }
        }

        /// <summary>
        ///     删除购物车
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete/{Id}")]
        public IHttpActionResult DeleteCart(Guid Id)
        {
            try
            {
                var openId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                var carts = db.Cart_Infos.Where(o => (o.Id == Id) && (o.UserID == openId)).FirstOrDefault();
                db.Cart_Infos.Remove(carts);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                log.Log(LoggerLevels.Error, "Cart/Delete：" + ex.Message);
                return BadRequest("操作失败，请稍后重试！");
            }
        }
    }
}