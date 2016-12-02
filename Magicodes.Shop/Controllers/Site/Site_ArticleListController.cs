using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Magicodes.Shop.Controllers.Site
{
    public class Site_ArticleListController : Controller
    {
        // GET: Site_ArticleList
        public ActionResult Index()
        {
            return View();
        }

        // GET: Site_ArticleList/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Site_ArticleList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Site_ArticleList/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Site_ArticleList/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Site_ArticleList/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Site_ArticleList/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Site_ArticleList/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
