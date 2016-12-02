// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AppDemoApiController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:58
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Magicodes.Shop.Controllers.WebApi;
using Magicodes.WeChat.SDK;
using Magicodes.WeChat.SDK.Pays;
using Magicodes.WeChat.SDK.Pays.EnterprisePay;
using Magicodes.WeChat.SDK.Pays.RedPackApi;
using Magicodes.WeiChat.Data.Models.Site;
using Newtonsoft.Json;
using NLog;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/AppDemo")]
    public class AppDemoApiController : TenantBaseApiController<Site_ResourceType>
    {
        [HttpGet]
        [Route("TestException")]
        public IHttpActionResult TestException()
        {
            throw new Exception("异常处理测试！");
        }

        #region 相册

        [HttpGet]
        [Route("PhotoGallery")]
        public IHttpActionResult PhotoGallery()
        {
            var resourceTypes =
                db.Site_ResourceTypes.Where(p => p.ResourceType == SiteResourceTypes.Gallery).Take(10).ToList();
            foreach (var item in resourceTypes)
                item.Cover = db.Site_Images.FirstOrDefault(p => p.ResourcesTypeId == item.Id).SiteUrl;
            return Ok(resourceTypes);
        }

        [HttpGet]
        [Route("PhotoGallery/Slides")]
        public IHttpActionResult PhotoGallerySlides()
        {
            return Ok(db.Site_Images.OrderByDescending(p => p.CreateTime).Take(5).ToList());
        }

        [HttpGet]
        [Route("PhotoGallery/{typeId}/Photos")]
        public IHttpActionResult GetPhotosByTypeId(Guid typeId)
        {
            return Ok(db.Site_Images.Where(p => p.ResourcesTypeId == typeId).ToList());
        }

        /// <summary>
        ///     上传照片处理
        ///     从临时素材获取
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PhotoGallery/{typeId}/Upload")]
        public IHttpActionResult UploadPhotos(Guid typeId)
        {
            var ids = HttpContext.Current.Request.Form["mediaIds"].Trim(',').Split(',');
            foreach (var mediaId in ids)
            {
                var fileName = DateTime.Now.ToString("yyyy-MM-dd");
                var fileSaveName = Guid.NewGuid().ToString("N") + ".png";
                var dirName = TenantId.ToString();
                var path = Path.Combine(HttpContext.Current.Server.MapPath("~/MediaFiles"), dirName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path = Path.Combine(path, fileSaveName);
                //从临时素材获取
                using (var stream = File.Create(path))
                {
                    //TODO:切换SDK
                    MediaApi.Get(WeChatConfigManager.Current.GetAccessToken(), mediaId, stream);
                }
                //上传到永久素材
                var result = MediaApi.UploadForeverMedia(AccessToken, path);
                if (!string.IsNullOrWhiteSpace(result.errmsg))
                    return BadRequest(result.errmsg);
                //写入数据
                var pic = new Site_Image
                {
                    IsFrontCover = !db.Site_Images.Any(p => p.ResourcesTypeId == typeId),
                    MediaId = result.media_id,
                    Name = fileName,
                    SiteUrl = string.Format("/MediaFiles/{0}/{1}", dirName, fileSaveName),
                    Url = result.url,
                    ResourcesTypeId = typeId
                };
                SetModel(pic, default(Guid));
                db.Site_Images.Add(pic);
                db.SaveChanges();
            }
            return Ok();
        }

        #endregion

        #region 支付模块

        [HttpPost]
        [Route("Pay/Redpack")]
        public IHttpActionResult SendNormalRed()
        {
            var model = new NormalRedPackRequest();
            model.ReOpenId = "ojyLxw31E_RvvrAw_m8HM-f6NpNA";
            model.ActName = "测试红包";
            model.MchBillno = PayUtil.GenerateOutTradeNo();
            model.NonceStr = PayUtil.GetNoncestr();
            model.SendName = "新来科技";
            model.TotalAmount = "100";
            model.TotalNum = "1";
            model.Wishing = "测试测试测试测试测试测试测试测试测试测试";
            LogManager.GetCurrentClassLogger().Debug(JsonConvert.SerializeObject(model));
            var relust = WeChatApisContext.Current.RedPackApi.SendNormalRedPack(model);

            LogManager.GetCurrentClassLogger().Debug(JsonConvert.SerializeObject(relust));
            return Ok(relust);
        }

        [HttpPost]
        [Route("Pay/Enternpire")]
        public IHttpActionResult Enternpire()
        {
            var model = new EnterpriseRequest();
            model.Amount = "100";
            model.CheckName = "NO_CHECK";
            model.Desc = "测试付款";
            model.DeviceInfo = "";
            model.NonceStr = PayUtil.GetNoncestr();
            model.OpenId = "ojyLxw31E_RvvrAw_m8HM-f6NpNA";
            model.SpbillCreateIp = "127.0.0.1";
            model.PartnerTradeNo = PayUtil.GenerateOutTradeNo();
            LogManager.GetCurrentClassLogger().Debug(JsonConvert.SerializeObject(model));

            var relust = WeChatApisContext.Current.EnterprisePayApi.EnterprisePayment(model);

            LogManager.GetCurrentClassLogger().Debug(JsonConvert.SerializeObject(relust));
            return Ok(relust);
        }

        #endregion
    }
}