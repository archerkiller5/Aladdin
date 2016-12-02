// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : UploadHelper.cs
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
using System.Linq;
using System.Web;
using Magicodes.Shop.Helpers.Resource;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Data.Models.Site;
using Magicodes.WeiChat.Infrastructure.MvcExtension.Ajax;

namespace Magicodes.Shop.Helpers.UEEditor
{
    public class UploadHelper
    {
        private readonly HttpRequest Request;
        private readonly HttpServerUtility Server;

        public UploadHelper(UploadConfig uploadConfig)
        {
            Server = HttpContext.Current.Server;
            UploadConfig = uploadConfig;
            Request = HttpContext.Current.Request;
            Result = new UploadResult {State = UploadState.Unknown};
        }

        public UploadConfig UploadConfig { get; }
        public UploadResult Result { get; }

        private object WriteResult()
        {
            return new
            {
                state = GetStateMessage(Result.State),
                url = Result.Url,
                title = Result.OriginFileName,
                original = Result.OriginFileName,
                error = Result.ErrorMessage
            };
        }

        private string GetStateMessage(UploadState state)
        {
            switch (state)
            {
                case UploadState.Success:
                    return "SUCCESS";
                case UploadState.FileAccessError:
                    return "文件访问出错，请检查写入权限";
                case UploadState.SizeLimitExceed:
                    return "文件大小超出服务器限制";
                case UploadState.TypeNotAllow:
                    return "不允许的文件格式";
                case UploadState.NetworkError:
                    return "网络错误";
            }
            return "未知错误";
        }

        public object Upload()
        {
            byte[] uploadFileBytes = null;
            string uploadFileName = null;

            if (UploadConfig.Base64)
            {
                uploadFileName = UploadConfig.Base64Filename;
                uploadFileBytes = Convert.FromBase64String(Request[UploadConfig.UploadFieldName]);
            }
            else
            {
                var file = HttpContext.Current.Request.Files[UploadConfig.UploadFieldName];
                uploadFileName = file.FileName;

                if (!CheckFileType(uploadFileName))
                {
                    Result.State = UploadState.TypeNotAllow;
                    return WriteResult();
                }
                if (!CheckFileSize(file.ContentLength))
                {
                    Result.State = UploadState.SizeLimitExceed;
                    return WriteResult();
                }

                uploadFileBytes = new byte[file.ContentLength];
                try
                {
                    file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
                }
                catch (Exception)
                {
                    Result.State = UploadState.NetworkError;
                    return WriteResult();
                }
            }
            Result.OriginFileName = uploadFileName;
            //TODO:添加租户目录，以隔离租户间的文件
            var savePath = PathFormatter.Format(uploadFileName, UploadConfig.PathFormat);
            var localPath = Server.MapPath(savePath);
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                File.WriteAllBytes(localPath, uploadFileBytes);
                Result.Url = savePath;
                Result.State = UploadState.Success;
            }
            catch (Exception e)
            {
                Result.State = UploadState.FileAccessError;
                Result.ErrorMessage = e.Message;
            }
            return WriteResult();
        }

        public object UploadImages()
        {
            byte[] uploadFileBytes = null;
            string uploadFileName = null;

            if (UploadConfig.Base64)
            {
                uploadFileName = UploadConfig.Base64Filename;
                uploadFileBytes = Convert.FromBase64String(Request[UploadConfig.UploadFieldName]);
                Result.State = UploadState.FileAccessError;
                Result.ErrorMessage = "不支持此格式上传";
                return WriteResult();
            }
            var file = HttpContext.Current.Request.Files[UploadConfig.UploadFieldName];
            uploadFileName = file.FileName;

            if (!CheckFileType(uploadFileName))
            {
                Result.State = UploadState.TypeNotAllow;
                return WriteResult();
            }
            if (!CheckFileSize(file.ContentLength))
            {
                Result.State = UploadState.SizeLimitExceed;
                return WriteResult();
            }
            uploadFileBytes = new byte[file.ContentLength];
            try
            {
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
            }
            catch (Exception)
            {
                Result.State = UploadState.NetworkError;
                return WriteResult();
            }
            try
            {
                using (var db = new AppDbContext())
                {
                    var defaultType =
                        db.Site_ResourceTypes.FirstOrDefault(
                            p => p.IsSystemResource && (p.ResourceType == SiteResourceTypes.Gallery));
                    if (defaultType == null)
                        defaultType = new Site_ResourceType
                        {
                            IsSystemResource = false,
                            ResourceType = SiteResourceTypes.Gallery
                        };
                    var ajaxMessage = new AjaxResponse {Success = true};
                    var fileBase = SiteResourceHelper.Upload(defaultType, Path.GetFileName(file.FileName),
                        uploadFileBytes, db, out ajaxMessage);
                    Result.OriginFileName = fileBase.Name;
                    Result.Url = fileBase.SiteUrl;
                    Result.State = UploadState.Success;
                    return WriteResult();
                }
            }
            catch (Exception)
            {
                Result.State = UploadState.NetworkError;
                return WriteResult();
            }
        }

        private bool CheckFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var fileExtension = extension.ToLower();
                return UploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension);
            }
            throw new ArgumentNullException("fileName");
        }

        private bool CheckFileSize(int size)
        {
            return size < UploadConfig.SizeLimit;
        }
    }
}