// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : StoreApiController.cs
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
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeiChat.Data.Models.PhotoGallery;
using Magicodes.WeiChat.Data.Models.Product;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Domain.Product;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.Logger;
using Newtonsoft.Json;
using Magicodes.WeiChat.Data.Models.Order;
using Magicodes.WeiChat.Data.Models;
using System.Data.Entity.Core.Objects;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{

    [RoutePrefix("api/Store")]
    public class StoreApiController : TenantBaseApiController<Site_ResourceType>
    {
    }

    [RoutePrefix("api/Advertise")]
    public class AdvertiseApiController : TenantBaseApiController<Site_Photo>
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get(Guid typeId)
        {
            var photos = db.Site_Photos.Where(p => p.GalleryId == typeId).Take(5).ToList();
            return Ok(photos);
        }
    }

    [RoutePrefix("api/Product")]
    public class ProductApiController : TenantBaseApiController<Product_Info>
    {
        private readonly LoggerBase log = Loggers.Current.DefaultLogger;

        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var pdo = new ProductDO();
            var data = pdo.GetProductAllInfoList();
            return Ok(data);
        }

        [HttpGet]
        [Route("GetForTag")]
        public IHttpActionResult GetForTag(string tagType, int pageStart = 0, int pageSize = 10)
        {
           
            log.Log(Logger.LoggerLevels.Debug, "GetForTag得到tagType[" + tagType + "]");
            Guid tag = Guid.Empty;
            if (!string.IsNullOrEmpty(tagType))
            {
                var tags = db.Product_Tags.ToList();
                var t = tags.Find(p => p.Name == tagType);
                if (t != null)
                    tag = t.Id;
            }
            log.Log(Logger.LoggerLevels.Debug, "GetForTag得到tag[" + tag.ToString() + "]");
            var q = (from product in db.Product_Infos
                     join t in db.Product_ProductTags on product.Id equals t.ProductId
                     where ((t.TagId == tag && tag != Guid.Empty) || tag == Guid.Empty)
                     select new
                     {
                         product,
                         photos = db.Site_Photos.Where(
                         photo => product.Id == photo.GalleryId)
                     }).OrderByDescending(p => p.product.SellCount);
            var list = q.Skip(pageStart).Take(pageSize).ToList().Select(p => new ProductAllInfo
            {
                ProductInfo = p.product,
                Photos = p.photos.ToList()
            });

            return Ok(list);
        }
        /// <summary>
        /// 保存评价信息
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveProductComment")]
        public IHttpActionResult SaveProductComment([FromBody] dynamic Json)
        {
            if (Json == null)
                return Ok(new { success = false, message = "评价信息不存在!" });
            try
            {                
                if (string.IsNullOrWhiteSpace(Convert.ToString(Json.CommentContent)))
                {
                    return Ok(new { success = false, message = "评价内容不能为空!" });
                }
                if (string.IsNullOrWhiteSpace(Convert.ToString(Json.CommentLevel)))
                {
                    return Ok(new { success = false, message = "评价等级不能为空!" });
                }
                if (string.IsNullOrEmpty(Convert.ToString(Json.OrderID)))
                {
                    return Ok(new { success = false, message = "传入订单ID为空!" });
                }
                if (string.IsNullOrEmpty(Convert.ToString(Json.ProductId)))
                {
                    return Ok(new { success = false, message = "传入商品ID为空!" });
                }
                Guid order_id = Guid.Parse(Convert.ToString(Json.OrderID));
                Guid product_id = Guid.Parse(Convert.ToString(Json.ProductId));
                var lst = db.Product_Comments.Where(p => p.OrderID == order_id && p.ProductId == product_id).ToList();
                if (lst != null && lst.Count > 0)
                {
                    return BadRequest("提示，您已经填写了本次订单商品的评论！");
                }
                Guid NewId = Guid.NewGuid();
                Product_Comment comment = new Product_Comment();
                comment.Id = NewId;
                comment.CommentContent = Convert.ToString(Json.CommentContent);
                comment.CommentLevel = (CommentLevels)Convert.ToInt32(Json.CommentLevel);
                comment.CreateTime = DateTime.Now;
                comment.IsAnonymous = Convert.ToBoolean(Json.IsAnonymous);
                comment.OpenId = WeiChatApplicationContext.Current.WeiChatUser.OpenId;
                comment.OrderID = order_id;
                comment.ProductId = product_id;
                comment.TenantId = WeiChatApplicationContext.Current.WeiChatUser.TenantId;
                db.Product_Comments.Add(comment);
                Order_Detail detail= db.Order_Details.FirstOrDefault(p => p.OrderID == order_id && p.ProductID == product_id);
                log.Log(LoggerLevels.Debug, NewId.ToString());
                detail.CommentId = NewId;
                //需要更新商品的评价数
                Product_Info product = db.Product_Infos.FirstOrDefault(p => p.Id == product_id);
                if (product == null) {
                    return BadRequest("提示，商品信息已删除，无法评论!");
                }
                product.CommentCount = product.CommentCount + 1;
                db.SaveChanges();
                return Ok(comment);
            }
            catch (Exception ea)
            {
                log.Log(LoggerLevels.Error, "api/Product/SaveProductComment：" + ea.Message);
                return BadRequest("系统异常，请联系管理员！");
            }
            
        }

        /// <summary>
        /// 查看评论
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>        
        [HttpPost]
        [Route("GetProductComment")]
        public IHttpActionResult GetProductComment([FromBody] dynamic Json)
        {
            if (Json == null)
                return Ok(new { success = false, message = "订单商品信息不存在!" });
            try
            {
                log.Log(LoggerLevels.Debug, JsonConvert.SerializeObject(Json));
                Guid order_id = new Guid(Convert.ToString(Json.OrderID));
                Guid product_id = new Guid(Convert.ToString(Json.ProductId));
                var lst = db.Product_Comments.Where(p => p.OrderID == order_id && p.ProductId == product_id).ToList();
                if (lst == null || lst.Count == 0)
                {
                    return BadRequest("提示，未发现该订单商品的评论,请刷新页面后查看!");
                }
                return Ok(lst[0]);
            }
            catch (Exception ea)
            {
                log.Log(LoggerLevels.Error, "api/Product/GetProductComment：" + ea.Message);
                return BadRequest("系统异常，请联系管理员！");
            }
        }

        [HttpGet]
        [Route("GetPdComment")]
        public IHttpActionResult GetPdComment(Guid ProductId, int pageStart = 0, int pageSize = 10)
        {
            var lst = (from user in db.WeiChat_Users
                       join comment in db.Product_Comments on user.OpenId equals comment.OpenId
                       select new
                       {
                           HeadImg = user.HeadImgUrl,
                           UserName = comment.IsAnonymous == false ? user.NickName : "匿名用户",
                           CommentContent = comment.CommentContent,
                           CommentLevel = comment.CommentLevel,
                           CommentDate =  comment.CreateTime.Year.ToString() + "-" +comment.CreateTime.Month.ToString() + "-" + comment.CreateTime.Day.ToString() + "-" + comment.CreateTime.Hour.ToString() + "-" + comment.CreateTime.Minute.ToString() 
                       }).OrderByDescending(p => p.CommentDate);

            var list = lst.Skip(pageStart).Take(pageSize).ToList();

            return Ok(list);
        }
    }
}