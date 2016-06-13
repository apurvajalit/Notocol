using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult LongPageLoad(int number)
        {
            return View(number);
        }

        public ActionResult Home()
        {
            return View();
        }

        public JsonResult Test()
        {
            TagHelper tgh = new TagHelper();
            List<string> tags = new List<string> { 
                "halwa", "solar",
                "hell", "whynot" };

            return Json(tgh.GetUsersForTags(tags), JsonRequestBehavior.AllowGet);
            
        }
    }
}