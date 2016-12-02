// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : ModulesController.cs
//          description :
//  
//          created by 李文强 at  2016/09/26 17:15
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Magicodes.WeiChat.Data;
using Microsoft.AspNet.Identity;
using Magicodes.WeiChat.Data.Models.Site;
using System.Collections.Generic;

namespace Magicodes.Shop.Controllers
{
    public class ModulesController : Controller
    {
        [Route("Modules/Map/GetPoint", Name = "GetPointByBaiduMapRoute")]
        public ActionResult GetPoint()
        {
            return View();
        }

        [Route("Modules/Icons", Name = "GetIconsRoute")]
        public ActionResult Icons()
        {
            return View();
        }
        /// <summary>
        /// 获取左侧导航菜单
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public ActionResult Menus(string tag = "Tenant")
        {
            using (var db = new AppDbContext())
            {
                var userId = User.Identity.GetUserId();
                var appUser = db.Users.Include(p => p.Roles).FirstOrDefault(p => p.Id == userId);
                if (appUser != null)
                {
                    var roles = appUser.Roles.Select(p => p.RoleId).ToArray();
                    var siteMenus = db.Site_Menus.ToList();
                    var menus =
                        db.Site_Menus.Where(
                                p =>
                                    (p.Tag == tag) &&
                                    db.Role_Menus.Any(p1 => (p1.MenuId == p.Id) && roles.Any(p2 => p2 == p1.RoleId)))
                            .ToList().Distinct().ToList();
                    var toAddList = new List<Site_Menu>();
                    foreach (var item in menus)
                    {
                        if (item.ParentId.HasValue && !menus.Any(p => p.Id == item.ParentId.Value) && !toAddList.Any(p=>p.Id == item.ParentId.Value))
                        {
                            toAddList.Add(siteMenus.First(p => p.Id == item.ParentId.Value));
                        }
                    }
                    menus.AddRange(toAddList);
                    return PartialView("_Navigation", menus);
                }
                return Content("您无权操作此内容！");
            }
        }
    }
}