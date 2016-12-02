// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : CsvFileResult.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CsvHelper;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.Results
{
    /// <summary>
    ///     导出CSV格式文件
    /// </summary>
    /// <typeparam name="T">实体数据类型</typeparam>
    //http://joshclose.github.io/CsvHelper/
    public class CsvFileResult<T> : FileResult where T : class
    {
        private readonly IEnumerable<T> _data;

        public CsvFileResult(IEnumerable<T> data)
            : base("text/csv")
        {
            FileDownloadName = string.Format("{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss"));
            _data = data;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            var outPutStream = response.OutputStream;
            using (var streamWriter = new StreamWriter(outPutStream, Encoding.UTF8))
            {
                using (var writer = new CsvWriter(streamWriter))
                {
                    writer.WriteHeader<T>();
                    foreach (var item in _data)
                        writer.WriteRecord(item);
                    streamWriter.Flush();
                    response.Flush();
                }
            }
        }
    }
}