using Magicodes.Shop.Controllers;
using Magicodes.WeiChat.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using System.Data.Entity;
using System.Net;
using Magicodes.Shop.Models;

namespace Magicodes.Shop.Areas.App.Controllers
{
    [AllowAnonymous]
    public class AppWeiChat_UserController : TenantBaseController<WeiChat_User>
    {
        // GET: App/AppWeiChat_User
        public async Task<ActionResult> Index(string q, int pageIndex = 1, int pageSize = 10 )
        {
            //  WeiChatViewUserAndUserInfo WeiChat = new WeiChatViewUserAndUserInfo();
            var queryable = db.WeiChat_Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                queryable =
                    queryable.Where(
                        p =>
                            p.NickName.Contains(q) || p.City.Contains(q) || p.Country.Contains(q) ||
                            p.Province.Contains(q) || p.Remark.Contains(q));
            queryable = queryable.OrderByDescending(p => p.SubscribeTime);
          
            var pagedList = new PagedList<WeiChat_User>(
                await queryable
                    .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                pageIndex, pageSize, await queryable.CountAsync());
            var myGroups = db.WeiChat_UserGroups.Where(p => p.TenantId == TenantId).ToList();
            foreach (var item in pagedList)
                //多对多修改
                //item.UserGroups = myGroups.FirstOrDefault(p =>p.GroupId==item.GroupIds);
            foreach (var d in item.GroupIds)
            {
                foreach (var ff in myGroups.Where(p => p.GroupId==d))
                {
                    item.UserGroups.Add(ff);
                };
            }
            ViewBag.UserGroups = new SelectList(myGroups, "GroupId", "Name");
            return View(pagedList);
        }
        public async Task<ActionResult> Details(string id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WeiChatViewUserAndUserInfo weichaeview = new WeiChatViewUserAndUserInfo();

            weichaeview.User = await db.WeiChat_Users.FindAsync(id);
            weichaeview.Userinfo = await db.User_Infos.FindAsync(id);
            if (weichaeview.User == null)
                return HttpNotFound();
            return View(weichaeview);
        }
    }
}