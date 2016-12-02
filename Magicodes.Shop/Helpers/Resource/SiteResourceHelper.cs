// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : SiteResourceHelper.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.IO;
using System.Web;
using Magicodes.WeChat.SDK;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;
using Microsoft.AspNet.Identity;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.Media;

namespace Magicodes.Shop.Helpers.Resource
{
    public class SiteResourceHelper
    {
        /// <summary>
        ///     上传站点资源
        /// </summary>
        /// <param name="resoureType"></param>
        /// <param name="file"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static Site_FileBase Upload(Site_ResourceType resoureType, string fileName, byte[] fileStream,
            AppDbContext db, out AjaxResponse ajaxMessage)
        {
            ajaxMessage = new AjaxResponse {Success = true};
            var fileSaveName = Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);
            var tenantId = WeiChatApplicationContext.Current.TenantId;

            var dirName = tenantId.ToString();
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/MediaFiles"), dirName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, fileSaveName);
            File.WriteAllBytes(path, fileStream);
            UploadForeverMediaResult result = null;
            result = resoureType.ResourceType == SiteResourceTypes.Video
                ? MediaApi.UploadForeverVideo(WeChatConfigManager.Current.GetAccessToken(), path, fileName, "test")
                : MediaApi.UploadForeverMedia(WeChatConfigManager.Current.GetAccessToken(), path);
            if (!string.IsNullOrWhiteSpace(result.errmsg))
            {
                ajaxMessage.Success = false;
                ajaxMessage.Message = result.errmsg;
            }
            else
            {
                switch (resoureType.ResourceType)
                {
                    case SiteResourceTypes.Gallery:
                    {
                        var pic = new Site_Image
                        {
                            IsFrontCover = false,
                            MediaId = result.media_id,
                            Name = fileName,
                            SiteUrl = string.Format("/MediaFiles/{0}/{1}", dirName, fileSaveName),
                            Url = result.url,
                            ResourcesTypeId = resoureType.Id,
                            CreateBy = HttpContext.Current.User.Identity.GetUserId(),
                            CreateTime = DateTime.Now,
                            TenantId = tenantId
                        };
                        db.Site_Images.Add(pic);
                        db.SaveChanges();
                        return pic;
                    }
                        break;
                    case SiteResourceTypes.Voice:
                    {
                        var voice = new Site_Voice
                        {
                            MediaId = result.media_id,
                            Name = fileName,
                            SiteUrl = string.Format("/MediaFiles/{0}/{1}", dirName, fileSaveName),
                            Url = result.url,
                            ResourcesTypeId = resoureType.Id,
                            CreateBy = HttpContext.Current.User.Identity.GetUserId(),
                            CreateTime = DateTime.Now,
                            TenantId = tenantId
                        };
                        db.Site_Voices.Add(voice);
                        db.SaveChanges();
                        return voice;
                    }
                        break;
                    case SiteResourceTypes.Video:
                    {
                        var video = new Site_Video
                        {
                            MediaId = result.media_id,
                            Name = fileName,
                            SiteUrl = string.Format("/MediaFiles/{0}/{1}", dirName, fileSaveName),
                            Url = result.url,
                            ResourcesTypeId = resoureType.Id,
                            CreateBy = HttpContext.Current.User.Identity.GetUserId(),
                            CreateTime = DateTime.Now,
                            TenantId = tenantId
                        };
                        db.Site_Videos.Add(video);
                        db.SaveChanges();
                        return video;
                    }
                        break;
                    default:
                        ajaxMessage.Success = false;
                        ajaxMessage.Message = "不支持上传此类型";
                        break;
                }
            }
            return null;
        }
    }
}