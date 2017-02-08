using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Magicodes.Shop.Areas.App.Controllers
{
    public class TextController : Controller
    {
        // GET: App/Text
        public ActionResult Index()
        {
            return View();
        }
    }
}