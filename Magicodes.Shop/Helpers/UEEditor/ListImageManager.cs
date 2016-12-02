// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ListImageManager.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Magicodes.WeiChat.Data;
using Magicodes.WeiChat.Infrastructure;

namespace Magicodes.Shop.Helpers.UEEditor
{
    public class ListImageManager
    {
        private string[] FileList;

        private readonly HttpRequest Request;
        private string[] SearchExtensions;
        private HttpServerUtility Server;
        private int Size;

        private int Start;
        private ResultState State;
        private int Total;

        public ListImageManager()
        {
            Request = HttpContext.Current.Request;
            Server = HttpContext.Current.Server;
        }

        public object GetFileList()
        {
            try
            {
                Start = string.IsNullOrEmpty(Request["start"]) ? 0 : Convert.ToInt32(Request["start"]);
                Size = string.IsNullOrEmpty(Request["size"])
                    ? ConfigHelper.GetInt("imageManagerListSize")
                    : Convert.ToInt32(Request["size"]);
            }
            catch (FormatException)
            {
                State = ResultState.InvalidParam;
                return WriteResult();
            }
            var buildingList = new List<string>();
            try
            {
                var tenantId = WeiChatApplicationContext.Current.TenantId;
                using (var db = new AppDbContext())
                {
                    Total = db.Site_Images.Where(p => p.TenantId == tenantId).Count();
                    FileList =
                        db.Site_Images.Where(p => p.TenantId == tenantId)
                            .OrderByDescending(x => x.CreateTime)
                            .Select(p => p.SiteUrl)
                            .Skip(Start)
                            .Take(Size)
                            .ToArray();
                }
                //var localPath = Server.MapPath(PathToList);
                //buildingList.AddRange(Directory.GetFiles(localPath, "*", SearchOption.AllDirectories)
                //    .Where(x => SearchExtensions.Contains(Path.GetExtension(x).ToLower()))
                //    .Select(x => PathToList + x.Substring(localPath.Length).Replace("\\", "/")));
                //Total = buildingList.Count;
                //FileList = buildingList.OrderBy(x => x).Skip(Start).Take(Size).ToArray();
            }
            catch (UnauthorizedAccessException)
            {
                State = ResultState.AuthorizError;
            }
            catch (DirectoryNotFoundException)
            {
                State = ResultState.PathNotFound;
            }
            catch (IOException)
            {
                State = ResultState.IOError;
            }
            return WriteResult();
        }

        private object WriteResult()
        {
            return new
            {
                state = GetStateString(),
                list = FileList == null ? null : FileList.Select(x => new {url = x}),
                start = Start,
                size = Size,
                total = Total
            };
        }

        private string GetStateString()
        {
            switch (State)
            {
                case ResultState.Success:
                    return "SUCCESS";
                case ResultState.InvalidParam:
                    return "参数不正确";
                case ResultState.PathNotFound:
                    return "路径不存在";
                case ResultState.AuthorizError:
                    return "文件系统权限不足";
                case ResultState.IOError:
                    return "文件系统读取错误";
            }
            return "未知错误";
        }

        private enum ResultState
        {
            Success,
            InvalidParam,
            AuthorizError,
            IOError,
            PathNotFound
        }
    }
}