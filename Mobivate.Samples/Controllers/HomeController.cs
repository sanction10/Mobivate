using Mobivate.Samples.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mobivate.Samples.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult SendSMS(MobivateModel model)
        {
            Mobivate.Api api = new Mobivate.Api(model.Username, model.Password);
            api.Send(model.From, model.To, model.Body, "API Test");

            return RedirectToAction("Index");
        }
    }
}
