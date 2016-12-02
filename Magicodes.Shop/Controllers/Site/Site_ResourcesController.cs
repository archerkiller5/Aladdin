// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : Site_ResourcesController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:13
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
using Magicodes.Shop.Helpers.Resource;
using Magicodes.Shop.Models;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using NLog;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace Magicodes.Shop.Controllers.Site
{
    public class Site_ResourcesController : TenantBaseController<Site_ResourceType>
    {
        // GET: Site_Resources
        public ActionResult Index(int? resourceType)
        {
            if (
                !db.Site_ResourceTypes.Any(
                    p =>
                        (p.ResourceType == SiteResourceTypes.Gallery) || (p.ResourceType == SiteResourceTypes.Voice) ||
                        (p.ResourceType == SiteResourceTypes.Video)))
            {
                var pic = new Site_ResourceType
                {
                    Title = "默认",
                    IsSystemResource = true,
                    ResourceType = SiteResourceTypes.Gallery
                };
                SetModel(pic, default(Guid));
                db.Site_ResourceTypes.Add(pic);
                var voice = new Site_ResourceType
                {
                    Title = "默认",
                    IsSystemResource = true,
                    ResourceType = SiteResourceTypes.Voice
                };
                SetModel(voice, default(Guid));
                db.Site_ResourceTypes.Add(voice);
                var video = new Site_ResourceType
                {
                    Title = "默认",
                    IsSystemResource = true,
                    ResourceType = SiteResourceTypes.Video
                };
                SetModel(video, default(Guid));
                db.Site_ResourceTypes.Add(video);
                db.SaveChanges();
            }
            var types = db.Site_ResourceTypes.AsQueryable();
            if (resourceType != null)
            {
                var type = (SiteResourceTypes)resourceType.Value;
                types = types.Where(p => p.ResourceType == type);
            }
            return View(types.ToList());
        }


        public ActionResult ListItems(Guid? Id)
        {
            var resoureType = db.Site_ResourceTypes.Find(Id);
            if (resoureType == null)
                return HttpNotFound();
            switch (resoureType.ResourceType)
            {
                case SiteResourceTypes.Gallery:
                    {
                        var items = db.Site_Images.Where(p => p.ResourcesTypeId == resoureType.Id).ToList();
                        return Json(items, JsonRequestBehavior.AllowGet);
                    }
                case SiteResourceTypes.Voice:
                    {
                        var items = db.Site_Voices.Where(p => p.ResourcesTypeId == resoureType.Id).ToList();
                        return Json(items, JsonRequestBehavior.AllowGet);
                    }
                case SiteResourceTypes.Video:
                    {
                        var items = db.Site_Videos.Where(p => p.ResourcesTypeId == resoureType.Id).ToList();
                        return Json(items, JsonRequestBehavior.AllowGet);
                    }
                default:
                    break;
            }
            return View();
        }

        /// <summary>
        ///     删除素材
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="id"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult Delete(Guid? typeId, Guid id, string mediaId)
        {
            var response = new AjaxResponse { Success = true, Message = "删除成功！" };
            var resoureType = db.Site_ResourceTypes.Find(typeId);
            if (resoureType == null)
                return HttpNotFound();

            var result = MediaApi.DeleteForeverMedia(AccessToken, mediaId);
            if (result.errcode != ReturnCode.请求成功)
            {
                response.Success = false;
                response.Message = "删除失败！" + result.errmsg;
            }
            else
            {
                switch (resoureType.ResourceType)
                {
                    case SiteResourceTypes.Gallery:
                        {
                            RemoveSiteResourceData<Site_Image>(id);
                            break;
                        }
                    case SiteResourceTypes.Voice:
                        {
                            RemoveSiteResourceData<Site_Voice>(id);
                            break;
                        }
                    case SiteResourceTypes.Video:
                        {
                            RemoveSiteResourceData<Site_Video>(id);
                            break;
                        }
                    default:
                        break;
                }
            }
            return Json(response);
        }

        /// <summary>
        ///     移除数据库中的数据记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        private void RemoveSiteResourceData<T>(Guid Id) where T : Site_FileBase, new()
        {
            var item = db.Set<T>().Find(Id);

            if (item != null)
            {
                //删除相关网站文件
                var path = Server.MapPath("~" + item.SiteUrl);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                db.Set<T>().Remove(item);
                db.SaveChanges();
            }
        }

        [Route("Site_Resources/Upload/{Id}")]
        public ActionResult Upload(Guid? Id)
        {
            var resoureType = db.Site_ResourceTypes.Find(Id);
            if (resoureType == null)
                return HttpNotFound();
            //可以接受的文件类型
            //图片（image）: 2M
            //语音（voice）：5M，播放长度不超过60s
            //视频（video）：10MB，支持MP4格式
            //缩略图（thumb）：64KB，支持JPG格式
            var acceptedFilesDic = new Dictionary<SiteResourceTypes, string>
            {
                {SiteResourceTypes.Gallery, ".jpg,.png,.bmp,.jpeg,.gif"},
                {SiteResourceTypes.Video, ".mp4"},
                {SiteResourceTypes.Voice, ".mp3,.amr,.wma,.wav"}
                //{ SiteResourceTypes.Thumb,".jpg" },
            };
            //最大上传大小，单位：M
            var maxFilesizeDic = new Dictionary<SiteResourceTypes, double>
            {
                {SiteResourceTypes.Gallery, 2},
                {SiteResourceTypes.Video, 10},
                {SiteResourceTypes.Voice, 5}
                //{ SiteResourceTypes.Thumb,0.065 }
            };
            ViewBag.acceptedFiles = acceptedFilesDic[resoureType.ResourceType];
            ViewBag.maxFilesize = maxFilesizeDic[resoureType.ResourceType];
            return View(resoureType);
        }

        /// <summary>
        ///     根据媒体Id获取媒体
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        [Route("Site_Resources/GetJsonDataByMediaId/{mediaId}")]
        public ActionResult GetJsonDataByMediaId(string mediaId)
        {
            Site_FileBase fileBase = db.Site_Images.FirstOrDefault(p => p.MediaId == mediaId);
            if (fileBase != null)
                return
                    Json(
                        new GetJsonDataByMediaIdViewModel
                        {
                            FileBase = fileBase,
                            ResourceType = SiteResourceTypes.Gallery
                        }, JsonRequestBehavior.AllowGet);
            fileBase = db.Site_News.FirstOrDefault(p => p.MediaId == mediaId);
            if (fileBase != null)
                return
                    Json(
                        new GetJsonDataByMediaIdViewModel { FileBase = fileBase, ResourceType = SiteResourceTypes.News },
                        JsonRequestBehavior.AllowGet);
            fileBase = db.Site_Videos.FirstOrDefault(p => p.MediaId == mediaId);
            if (fileBase != null)
                return
                    Json(
                        new GetJsonDataByMediaIdViewModel { FileBase = fileBase, ResourceType = SiteResourceTypes.Video },
                        JsonRequestBehavior.AllowGet);
            fileBase = db.Site_Voices.FirstOrDefault(p => p.MediaId == mediaId);
            return Json(
                new GetJsonDataByMediaIdViewModel { FileBase = fileBase, ResourceType = SiteResourceTypes.Voice },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Site_Resources/Upload/{Id}", Name = "UploadFilesRoute")]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(Guid? Id, string message = null)
        {
            var ajaxMessage = new AjaxResponse { Success = true, Message = "上传成功！" };
            var resoureType = db.Site_ResourceTypes.Find(Id);
            if (resoureType == null)
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = "目录不存在或已删除！";
                return Json(ajaxMessage);
            }
            foreach (var fileKey in Request.Files.AllKeys)
            {
                var file = Request.Files[fileKey];
                try
                {
                    var uploadFileBytes = new byte[file.ContentLength];
                    try
                    {
                        file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
                    }
                    catch (Exception ex)
                    {
                        ajaxMessage.Success = false;
                        ajaxMessage.Message = "文件写入错误！";
                        Logger.Log(Magicodes.Logger.LoggerLevels.Error, ex.ToString());
                        return Json(ajaxMessage);
                    }
                    if (file != null)
                        SiteResourceHelper.Upload(resoureType, Path.GetFileName(file.FileName), uploadFileBytes, db,
                            out ajaxMessage);
                }
                catch (Exception ex)
                {
                    ajaxMessage.Success = false;
                    ajaxMessage.Message = ex.Message;
                    Logger.Log(Magicodes.Logger.LoggerLevels.Error, ex.ToString());
                }
            }
            return Json(ajaxMessage);
        }
    }
}