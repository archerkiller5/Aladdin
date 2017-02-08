using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Magicodes.Shop.Areas.App.Controllers
{
    [RouteArea("App")]
    public class PersonalsController : AppBaseController
    {
        // GET: App/Personals
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 完善资料
        /// </summary>
        /// <returns></returns>
        public ActionResult EditInfo()
        {
            return View();
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        public ActionResult Out()
        {


            return View();
        }
        /// <summary>
        /// 充值
        /// </summary>
        /// <returns></returns>
        public ActionResult Recharge()
        {
            return View();
        }
        /// <summary>
        /// 发布悬赏
        /// </summary>
        /// <returns></returns>
        public ActionResult reward()
        {
            return View();
        }
        /// <summary>
        /// 任务列表
        /// </summary>
        /// <returns></returns>
        public ActionResult publish_task()
        {
            return View();
        }
        /// <summary>
        /// 提现
        /// </summary>
        /// <returns></returns>
        public ActionResult extract()
        {
            return View();
        }
    }
}