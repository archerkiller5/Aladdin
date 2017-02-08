using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Magicodes.Shop.Controllers
{
    public class HelloWorldController : Controller
    {
        // GET: HelloWorld
        public ActionResult Index()
        {
            Magicodes.Shop.Models.HelloWorlda hehe = new Models.HelloWorlda();
            
            return PartialView(hehe);
        }

        // GET: HelloWorld/Details/5
        public ActionResult Details(int id)
        {
            Magicodes.Shop.Models.HelloWorlda hehe = new Models.HelloWorlda();

            return View(hehe);
        }

        // GET: HelloWorld/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HelloWorld/Create
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

        // GET: HelloWorld/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HelloWorld/Edit/5
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

        // GET: HelloWorld/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HelloWorld/Delete/5
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
