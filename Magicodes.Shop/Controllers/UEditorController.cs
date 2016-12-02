// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : UEditorController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Web.Mvc;
using Magicodes.Shop.Helpers.UEEditor;

namespace Magicodes.Shop.Controllers
{
    public class UEditorController : BaseController
    {
        public ActionResult Index()
        {
            var action = Request.QueryString["action"];
            if (string.IsNullOrWhiteSpace(action))
                return Json(new
                {
                    state = "action 参数为空或者 action 不被支持。"
                }, JsonRequestBehavior.AllowGet);
            action = action.ToUpper();
            switch (action)
            {
                case "CONFIG":
                    return Config();
                case "UPLOADIMAGE":
                    return UploadImage();
                case "UPLOADSCRAWL":
                    return UploadScrawl();
                case "UPLOADVIDEO":
                    return UploadVideo();
                case "UPLOADFILE":
                    return UploadFile();
                case "LISTIMAGE":
                    return ListImage();
                case "LISTFILE":
                    return ListFile();
                case "CATCHIMAGE":
                    return CatchImage();
                default:
                    return Json(new
                    {
                        state = "action 参数为空或者 action 不被支持。"
                    }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///     获取UEditor配置信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Config()
        {
            var jobj = ConfigHelper.Items;
            var hostUrl = string.Format("{0}://{1}{2}/", Request.Url.Scheme, Request.Url.Host,
                Request.Url.Port == 80 ? "" : ":" + Request.Url.Port);
            jobj["imageUrlPrefix"] = hostUrl;
            jobj["scrawlUrlPrefix"] = hostUrl;
            jobj["snapscreenUrlPrefix"] = hostUrl;
            jobj["catcherUrlPrefix"] = hostUrl;
            jobj["videoUrlPrefix"] = hostUrl;
            jobj["fileUrlPrefix"] = hostUrl;
            jobj["imageManagerUrlPrefix"] = hostUrl;
            jobj["fileManagerUrlPrefix"] = hostUrl;
            return Json(jobj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     上传图片
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadImage()
        {
            var uploadHelper = new UploadHelper(new UploadConfig
            {
                AllowExtensions = ConfigHelper.GetStringList("imageAllowFiles"),
                PathFormat = ConfigHelper.GetString("imagePathFormat"),
                SizeLimit = ConfigHelper.GetInt("imageMaxSize"),
                UploadFieldName = ConfigHelper.GetString("imageFieldName")
            });
            return Json(uploadHelper.UploadImages(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadScrawl()
        {
            var uploadHelper = new UploadHelper(new UploadConfig
            {
                AllowExtensions = new[] {".png"},
                PathFormat = ConfigHelper.GetString("scrawlPathFormat"),
                SizeLimit = ConfigHelper.GetInt("scrawlMaxSize"),
                UploadFieldName = ConfigHelper.GetString("scrawlFieldName"),
                Base64 = true,
                Base64Filename = "scrawl.png"
            });
            return Json(uploadHelper.UploadImages(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadVideo()
        {
            var uploadHelper = new UploadHelper(new UploadConfig
            {
                AllowExtensions = ConfigHelper.GetStringList("videoAllowFiles"),
                PathFormat = ConfigHelper.GetString("videoPathFormat"),
                SizeLimit = ConfigHelper.GetInt("videoMaxSize"),
                UploadFieldName = ConfigHelper.GetString("videoFieldName")
            });
            return Json(uploadHelper.Upload(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadFile()
        {
            var uploadHelper = new UploadHelper(new UploadConfig
            {
                AllowExtensions = ConfigHelper.GetStringList("fileAllowFiles"),
                PathFormat = ConfigHelper.GetString("filePathFormat"),
                SizeLimit = ConfigHelper.GetInt("fileMaxSize"),
                UploadFieldName = ConfigHelper.GetString("fileFieldName")
            });
            return Json(uploadHelper.Upload(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListImage()
        {
            return Json(new ListImageManager().GetFileList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListFile()
        {
            return
                Json(
                    new ListFileManager(ConfigHelper.GetString("imageManagerListPath"),
                        ConfigHelper.GetStringList("fileManagerAllowFiles")).GetFileList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CatchImage()
        {
            return Json(new CrawlerHelper().Crawle(), JsonRequestBehavior.AllowGet);
        }
    }
}