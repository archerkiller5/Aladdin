// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : WeiChat_PayConfigController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:14
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Magicodes.Mvc.RoleMenuFilter;
using Magicodes.WeiChat.Data.Models;

namespace Magicodes.Shop.Controllers.WeChat
{
    /// <summary>
    ///     微信支付配置
    /// </summary>
    [RoleMenuFilter("系统设置", "72A9DBB1-4982-407E-9A78-31DEB153AB24", "Admin,TenantManager,ShopManager",
         iconCls: "fa fa-wrench")]
    public class WeiChat_PayConfigController : AdminUniqueTenantBaseController<WeiChat_Pay>
    {
        /// <summary>
        ///     获取支付通知地址
        /// </summary>
        /// <returns></returns>
        private string GetNotifyUrl()
        {
            return string.Format("{0}://{1}/WeiChat/PayNotify/{2}", Request.Url.Scheme, Request.Url.Host, TenantId);
        }

        [RoleMenuFilter("微信支付设置", "9796147C-0E46-4C34-8AF1-FA0A349F3F9C", "Admin,TenantManager,ShopManager",
             url: "/WeiChat_PayConfig", parentId: "72A9DBB1-4982-407E-9A78-31DEB153AB24")]
        public override ActionResult Index()
        {
            var model = db.Set<WeiChat_Pay>().FirstOrDefault();
            if (model == null)
            {
                model = new WeiChat_Pay
                {
                    CreateBy = UserId,
                    CreateTime = DateTime.Now,
                    TenantId = TenantId,
                    Notify = GetNotifyUrl()
                };
                db.Set<WeiChat_Pay>().Add(model);
                db.SaveChanges();
            }
            return View(model);
        }

        [HttpPost]
        public override ActionResult Index(WeiChat_Pay model)
        {
            if ((Request.Files.Count > 0) && (Request.Files["PayCertFile"].ContentLength > 0) &&
                (Path.GetExtension(Request.Files["PayCertFile"].FileName).ToLower() != ".p12"))
                ModelState.AddModelError("PayCertFile", "上传的支付证书文件有误！");
            if (string.IsNullOrWhiteSpace(model.CertPassword))
                ModelState.AddModelError("CertPassword", "证书密钥是必须的！");
            if (string.IsNullOrWhiteSpace(model.MchId))
                ModelState.AddModelError("MchId", "商户Id是必须的！");
            if (string.IsNullOrWhiteSpace(model.Notify))
                ModelState.AddModelError("Notify", "回调地址是必须的！");
            if (string.IsNullOrWhiteSpace(model.TenPayKey))
                ModelState.AddModelError("TenPayKey", "支付密钥是必须的！");
            if (ModelState.IsValid)
            {
                if ((Request.Files.Count > 0) && (Request.Files["PayCertFile"].ContentLength > 0) &&
                    (Path.GetExtension(Request.Files["PayCertFile"].FileName).ToLower() == ".p12"))
                {
                    var file = Request.Files["PayCertFile"];

                    #region 保存证书，用于红包发放等接口

                    var path = Path.Combine(Server.MapPath("~/App_Data"), "certs", TenantId.ToString());
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path = Path.Combine(path, "pay.p12");
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    file.SaveAs(path);

                    #endregion

                    model.PayCertPath = string.Format("/App_Data/certs/{0}/pay.p12", TenantId);
                }
                model.Notify = GetNotifyUrl();
                SetModelWithSaveChanges(model, model.Id);
            }
            return View(model);
        }
    }
}