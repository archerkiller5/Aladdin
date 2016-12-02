// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ExcelFileResult.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 16:22
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Magicodes.WeiChat.Infrastructure.MvcExtension.Results
{
    //https://github.com/christophano/CsvHelper.Excel
    public class ExcelFileResult<T> : FileResult where T : class
    {
        private IEnumerable<T> _data;


        public ExcelFileResult(IEnumerable<T> data)
            : base("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            _data = data;
        }


        protected override void WriteFile(HttpResponseBase response)
        {
            //var outPutStream = response.OutputStream;
            //using (var streamWriter = new StreamWriter(outPutStream, Encoding.UTF8))
            //{
            //    using (var workbook = new XLWorkbook(outPutStream, XLEventTracking.Disabled))
            //    {
            //        using (var writer = new CsvWriter(new ExcelSerializer(workbook)))
            //        {
            //            writer.WriteHeader<T>();
            //            foreach (var item in _data)
            //            {
            //                writer.WriteRecord(item);
            //            }
            //            streamWriter.Flush();
            //            response.Flush();
            //        }
            //    }


            //}
        }
    }
}