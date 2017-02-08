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
using System.Web.Http.Results;
using System.Text;

namespace Magicodes.Shop.Areas.App.Controllers.Api
{
    [RoutePrefix("api/UploadApi")]
    public class UploadApiController : TenantBaseApiController<Site_ResourceType>
    {
        //[HttpPost]
        //public JsonResult Upload(HttpPostedFileBase upImg)
        //{
        //    //定义允许上传的文件扩展名
        //    const string fileTypes = "gif,jpg,png,bmp";
        //    string fileExt = Path.GetExtension(upImg.FileName);
        //    if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(fileTypes.Split(','), fileExt.Substring(1).ToLower()) == -1)
        //    {
        //        return Json("上传文件不支持文件格式！");
        //    }
        //    //设置最大文件大小（2M）
        //    const int maxSize = 2050000;
        //    if (upImg.ContentLength > maxSize)
        //    {
        //        return Json("上传文件中的图片大小超出2M!!");
        //    }
        //    //存储文件名
        //    string fileName = DateTime.Now.ToString("yyyyMMddhhmmssff") + "_" + System.IO.Path.GetFileName(upImg.FileName) + CreateRandomCode(8);
        //    string filePhysicalPath = Server.MapPath("~/upload/" + fileName);
        //    string imgUrl = "", error = "";
        //    try
        //    {
        //        upImg.SaveAs(filePhysicalPath);
        //        imgUrl = "/upload/" + fileName;
        //    }
        //    catch (Exception ex)
        //    {
        //        error = "上传失败！";
        //        throw new Exception("上传失败！", ex);
        //    }
        //    return Json(new
        //    {
        //        imgUrl = imgUrl,
        //        error = error
        //    });
        //}
        /// <summary>
        /// 生成指定长度的随机码。
        /// </summary>
        private string CreateRandomCode(int length)
        {
            string[] codes = new string[36] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            StringBuilder randomCode = new StringBuilder();
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                randomCode.Append(codes[rand.Next(codes.Length)]);
            }
            return randomCode.ToString();
        }
    }
}
