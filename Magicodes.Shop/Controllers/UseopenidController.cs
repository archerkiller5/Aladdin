using Magicodes.WeiChat.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Magicodes.Shop.Controllers
{
    public class UseopenidController : Controller
    {
        // GET: Useopenid

        [AllowAnonymous]
        [WeiChat.Infrastructure.MvcExtension.Filters.WeChatOAuth]
        public ActionResult Index()
        {
            return Content(WeiChatApplicationContext.Current.TenantId + WeiChatApplicationContext.Current.WeiChatUser.OpenId);
        }
    }
}