using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace urban_archive.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index1()
        {
            return View();
        }
        public ActionResult yewuke()
        {
            return View();
        }
        public ActionResult bangongshi()
        {
            return View();
        }
        public ActionResult jieyue()
        {
            return View();
        }
        public ActionResult shengxiang()
        {
            return View();
        }
        public ActionResult bianyanke()
        {
            return View();
        }
        public ActionResult administrator()
        {
            return View();
        }
        public ActionResult caiwushi()
        {
            return View();
        }
        public ActionResult fuyinshi()
        {
            return View();
        }
        public ActionResult guanlike()
        {
            return View();
        }
        public ActionResult guanzhang()
        {
            return View();
        }
        public ActionResult guanxianke()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}