// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : AdminUniqueTenantBaseController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Linq;
using System.Web.Mvc;
using Magicodes.WeiChat.Data.Models;
using Magicodes.Logger;
using Magicodes.WeiChat.Infrastructure;

namespace Magicodes.Shop.Controllers
{
    /// <summary>
    ///     此控制器主要用于配置代码设置
    /// </summary>
    /// <typeparam name="TEntry"></typeparam>
    [Authorize]
    public class AdminUniqueTenantBaseController<TEntry> : TenantBaseController<TEntry>
        where TEntry : WeiChat_AdminUniqueTenantBase<int>, new()
    {
        public readonly LoggerBase log = Loggers.Current.DefaultLogger;

        [HttpGet]
        public virtual ActionResult Index()
        {
            var model = db.Set<TEntry>().FirstOrDefault();
            if (model == null)
            {
                model = new TEntry
                {
                    CreateBy = UserId,
                    CreateTime = DateTime.Now,
                    TenantId = TenantId
                };
                db.Set<TEntry>().Add(model);
                db.SaveChanges();
            }
            ViewBag.Success = false;
            ViewBag.Message = "";
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Index(TEntry model)
        {
            if (ModelState.IsValid)
            {
                SetModelWithSaveChanges(model, model.Id);
                ViewBag.Success = true;
                ViewBag.Message = "提交成功!";
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = "";
            }
            return View(model);
        }
    }
}