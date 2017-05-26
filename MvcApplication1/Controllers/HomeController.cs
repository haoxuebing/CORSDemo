using MvcApplication1.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [Cors(".*")]
        public JsonResult CorsApi()
        {
            return Json(new { data = "跨域请求成功" }, JsonRequestBehavior.AllowGet);
        }

        [Cors]
        public ActionResult About()
        {
            HttpWebRequest http = (HttpWebRequest)HttpWebRequest.Create("http://localhost:20735/home/CorsApi");
            var reader = new StreamReader(http.GetResponse().GetResponseStream(), Encoding.UTF8).ReadToEnd();
            ViewBag.result = reader;
            return View();
        }

    }
}
