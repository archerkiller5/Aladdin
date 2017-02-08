using Magicodes.WeiChat.Data.Models.WeChatStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Magicodes.Shop.SignSet
{
    public class SignSetController : Controller
    {
        // GET: SignSet
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Sign_Reward model)
        {
            return View();
        }
        public ActionResult Sign_list()
        {
            return View();
        }
    }
}