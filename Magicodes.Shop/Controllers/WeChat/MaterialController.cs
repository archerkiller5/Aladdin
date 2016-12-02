// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MaterialController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Magicodes.Shop.Models;
using Magicodes.WeiChat.Data.Models;
using Magicodes.WeiChat.Unity.WeChat;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.Media;
using Webdiyer.WebControls.Mvc;

namespace Magicodes.Shop.Controllers.WeChat
{
    public class MaterialController : TenantBaseController<WeiChat_User>
    {
        /// <summary>
        ///     素材管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///     图片管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Images(int pageIndex = 1, int pageSize = 18)
        {
            MediaList_OthersResult data;
            IEnumerable<MaterialViewModel> dataList;
            GetImages(pageIndex, pageSize, out data, out dataList);
            var pagedList = new PagedList<MaterialViewModel>(dataList, pageIndex, pageSize, data.total_count);
            return View(pagedList);
        }

        /// <summary>
        ///     图片管理
        /// </summary>
        [Route("JSON/Material/Images/{pageIndex}/{pageSize}")]
        [HttpGet]
        public ActionResult GetImagesJson(int pageIndex = 1, int pageSize = 20)
        {
            MediaList_OthersResult data;
            IEnumerable<MaterialViewModel> dataList;
            GetImages(pageIndex, pageSize, out data, out dataList);
            var pagedList = new DataPageListViewModel<MaterialViewModel>(dataList, pageIndex, pageSize, data.total_count);
            return Json(pagedList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     语音管理
        /// </summary>
        [Route("JSON/Material/Voices/{pageIndex}/{pageSize}")]
        [HttpGet]
        public ActionResult GetVoicesJson(int pageIndex = 1, int pageSize = 20)
        {
            IEnumerable<MaterialViewModel> dataList;
            var data = MediaApi.GetOthersMediaList(AccessToken, UploadMediaFileType.voice, (pageIndex - 1)*pageSize,
                pageSize);
            dataList = data.item
                .Select(p =>
                    new MaterialViewModel
                    {
                        Id = p.media_id,
                        Name = p.name,
                        UpdateTime = p.update_time.ConvertToDateTime(),
                        Url = p.url
                    });
            var pagedList = new DataPageListViewModel<MaterialViewModel>(dataList, pageIndex, pageSize, data.total_count);
            return Json(pagedList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     多图文管理
        /// </summary>
        [Route("JSON/Material/News/{pageIndex}/{pageSize}")]
        [HttpGet]
        public ActionResult GetNewsJson(int pageIndex = 1, int pageSize = 6)
        {
            var data = MediaApi.GetNewsMediaList(AccessToken, (pageIndex - 1)*pageSize, pageSize);
            var dataList = data.item
                .Select(p =>
                    new MaterialNewsViewModel
                    {
                        Id = p.media_id,
                        Title = p.content.news_item.First().title,
                        UpdateTime = p.update_time.ConvertToDateTime(),
                        ThumbMediaId = p.content.news_item.First().thumb_media_id,
                        Digest = p.content.news_item.First().digest,
                        Url = p.content.news_item.First().url
                    });
            var path = Server.MapPath("~/MediaFiles");
            path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var item in dataList)
            {
                var mediaPath = Path.Combine(path, item.ThumbMediaId + ".jpg");
                if (!System.IO.File.Exists(mediaPath))
                    using (var fs = System.IO.File.Create(mediaPath))
                    {
                        MediaApi.GetForeverMedia(AccessToken, item.ThumbMediaId, fs);
                        fs.Close();
                    }
            }
            var pagedList = new DataPageListViewModel<MaterialNewsViewModel>(dataList, pageIndex, pageSize,
                data.total_count);
            return Json(pagedList, JsonRequestBehavior.AllowGet);
        }


        private void GetImages(int pageIndex, int pageSize, out MediaList_OthersResult data,
            out IEnumerable<MaterialViewModel> dataList)
        {
            data = MediaApi.GetOthersMediaList(AccessToken, UploadMediaFileType.image, (pageIndex - 1)*pageSize,
                pageSize);
            dataList = data.item
                .Select(p =>
                    new MaterialViewModel
                    {
                        Id = p.media_id,
                        Name = p.name,
                        UpdateTime = p.update_time.ConvertToDateTime(),
                        Url = p.url
                    });
        }

        /// <summary>
        ///     语音管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Voices(int pageIndex = 1, int pageSize = 18)
        {
            var data = MediaApi.GetOthersMediaList(AccessToken, UploadMediaFileType.voice, pageIndex, pageSize);
            var dataList = data.item
                .Select(p =>
                    new MaterialViewModel
                    {
                        Id = p.media_id,
                        Name = p.name,
                        UpdateTime = p.update_time.ConvertToDateTime(),
                        Url = p.url
                    });
            var pagedList = new PagedList<MaterialViewModel>(dataList, pageIndex, pageSize, data.total_count);
            return View(pagedList);
        }

        /// <summary>
        ///     图文消息管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Messages(int pageIndex = 1, int pageSize = 18)
        {
            var data = MediaApi.GetNewsMediaList(AccessToken, pageIndex, pageSize);
            var dataList = data.item
                .Select(p =>
                    new MaterialNewsViewModel
                    {
                        Id = p.media_id,
                        Title = p.content.news_item.First().title,
                        UpdateTime = p.update_time.ConvertToDateTime(),
                        ThumbMediaId = p.content.news_item.First().thumb_media_id,
                        Digest = p.content.news_item.First().digest,
                        Url = p.content.news_item.First().url
                    });
            var path = Server.MapPath("~/MediaFiles");
            path = Path.Combine(path, "thumb");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            foreach (var item in dataList)
            {
                var mediaPath = Path.Combine(path, item.ThumbMediaId + ".jpg");
                if (!System.IO.File.Exists(mediaPath))
                    using (var stream = System.IO.File.Create(mediaPath))
                    {
                        MediaApi.GetForeverMedia(AccessToken, item.ThumbMediaId, stream);
                    }
            }
            var pagedList = new PagedList<MaterialNewsViewModel>(dataList, pageIndex, pageSize, data.total_count);
            return View(pagedList);
        }

        /// <summary>
        ///     添加图文消息
        /// </summary>
        [Authorize]
        public ActionResult AddMessage()
        {
            return View();
        }

        /// <summary>
        ///     上传附件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadVoice()
        {
            var message = "上传成功！";
            var isError = false;
            foreach (var fileKey in Request.Files.AllKeys)
            {
                var file = Request.Files[fileKey];
                try
                {
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/MediaFiles"), fileName);
                        file.SaveAs(path);
                        //增加上传的超时时间
                        var result = MediaApi.UploadForeverMedia(AccessToken, path, 120000);
                        if (!string.IsNullOrWhiteSpace(result.errmsg))
                        {
                            isError = true;
                            message = result.errmsg;
                        }
                        else if (result.errcode == ReturnCode.系统繁忙此时请开发者稍候再试)
                        {
                            //再调用一次
                            result = MediaApi.UploadForeverMedia(AccessToken, path, 120000);
                            if (!string.IsNullOrWhiteSpace(result.errmsg))
                            {
                                isError = true;
                                message = result.errmsg;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    message = ex.Message;
                }
            }
            return Json(new {Message = message, IsError = isError});
        }

        /// <summary>
        ///     上传附件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadImage()
        {
            var message = "上传成功！";
            var isError = false;
            foreach (var fileKey in Request.Files.AllKeys)
            {
                var file = Request.Files[fileKey];
                try
                {
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/MediaFiles"), fileName);
                        file.SaveAs(path);
                        var result = MediaApi.UploadForeverMedia(AccessToken, path);
                        if (!string.IsNullOrWhiteSpace(result.errmsg))
                        {
                            isError = true;
                            message = result.errmsg;
                        }
                    }
                }
                catch (Exception ex)
                {
                    isError = true;
                    message = ex.Message;
                }
            }
            return Json(new {Message = message, IsError = isError});
        }

        /// <summary>
        ///     获取图片、语音、视频文件列表
        /// </summary>
        /// <returns>JSON</returns>
        public JsonResult GetMediaJsonResult(UploadMediaFileType fileType)
        {
            var accessToken = AccessToken;

            switch (fileType)
            {
                case UploadMediaFileType.news:

                    break;
                case UploadMediaFileType.image:
                    var images = MediaApi.GetOthersMediaList(accessToken, UploadMediaFileType.image, 0, 20);
                    return Json(images, JsonRequestBehavior.AllowGet);
                    break;
                case UploadMediaFileType.voice:
                    var voices = MediaApi.GetOthersMediaList(accessToken, UploadMediaFileType.voice, 0, 20);
                    return Json(voices, JsonRequestBehavior.AllowGet);
                    break;
                case UploadMediaFileType.video:
                    var videos = MediaApi.GetOthersMediaList(accessToken, UploadMediaFileType.video, 0, 20);
                    return Json(videos, JsonRequestBehavior.AllowGet);
                    break;
            }
            return null;
        }

        /// <summary>
        ///     获取图文消息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMsgJsonResult()
        {
            //todo:分页
            var msgs = MediaApi.GetNewsMediaList(AccessToken, 0, 20);
            return Json(msgs, JsonRequestBehavior.AllowGet);
        }
    }
}