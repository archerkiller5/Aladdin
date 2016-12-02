// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : PermanentMaterialUploads.cs
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
using System.Net;
using System.Text;
using Magicodes.Shop.Areas.App.Models;

namespace Magicodes.Shop.Helpers
{
    public class PermanentMaterialUploads
    {
        /// <summary>
        ///     永久其它类型素材上传
        /// </summary>
        /// <param name="accessToken">AccessToken</param>
        /// <param name="isFile">素材</param>
        /// <param name="materialType">上传的数据类型</param>
        /// <param name="title">针对视频文件  文件名称</param>
        /// <param name="introduction">针对视频文件  文件描述</param>
        /// <returns></returns>
        public static string MaterialUploads(string accessToken, SuCai isFile, string materialType, string title = "",
            string introduction = "")
        {
            var url = "https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}";
            url = string.Format(url, accessToken);

            //设置提交表单的名称
            var formName = materialType == "video" ? "description" : "media";

            byte[] datas = null;

            #region 组合正文

            //换行
            var CRLF = "\r\n";

            //边界标识
            var Identification = "----" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                //生成表单信息
                var sb = new StringBuilder();
                sb.Append("--");
                sb.Append(Identification);
                sb.Append(CRLF);
                sb.Append("Content-Disposition: form-data; id=\"" + formName + "\"; name=\"" + formName +
                          "\"; filename=\"" + isFile.FileName + "\"");
                sb.Append(CRLF);
                sb.Append("Content-Type: " + isFile.ContentType.ToLower() + "");
                sb.Append(CRLF);
                sb.Append(CRLF);

                //如果是视频数据，就添加视频标题与视频的描述信息
                if (materialType == "video")
                {
                    sb.Append("description=\"{\"title\":\"" + title + "\", \"introduction\":\"" + introduction + "\"}\"");
                    sb.Append(CRLF);
                    sb.Append(CRLF);
                }

                //头部与尾部
                var header = sb.ToString();
                var foot = CRLF + "--" + Identification + "--" + CRLF;

                //转为byte[]流
                var headerbytes = Encoding.UTF8.GetBytes(header);
                var footbytes = Encoding.UTF8.GetBytes(foot);

                var datalist = new List<byte>();

                //转换为btye[]数据流
                var stream = isFile.InputStream;
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);

                // 设置当前流的位置为流的开始 
                stream.Seek(0, SeekOrigin.Begin);

                datalist.AddRange(headerbytes);
                datalist.AddRange(data);
                datalist.AddRange(footbytes);

                //组合完整的流数据
                datas = datalist.ToArray<byte>();
            }
            catch (Exception)
            {
                return "正文组合失败";
            }

            #endregion

            return MaterialUpload(datas, url, Identification);
        }

        /// <summary>
        ///     微信素材上传
        /// </summary>
        /// <param name="data"></param>
        /// <param name="url"></param>
        /// <param name="Identification">正文边界</param>
        /// <returns></returns>
        public static string MaterialUpload(byte[] data, string url, string Identification)
        {
            Stream responseStream;
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (request == null)
                throw new ApplicationException(string.Format("Invalid url string: {0}", url));
            request.UserAgent =
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            request.ContentType = "multipart/form-data; boundary=" + Identification;
            //request.ContentType = "application/x-www-form-urlencoded; boundary=" + Identification;
            request.Method = "POST";
            request.ContentLength = data.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            try
            {
                responseStream = request.GetResponse().GetResponseStream();
            }
            catch (Exception exception)
            {
                throw exception;
            }
            var str = string.Empty;
            using (var reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
            {
                str = reader.ReadToEnd();
            }
            responseStream.Close();
            return str;
        }
    }
}