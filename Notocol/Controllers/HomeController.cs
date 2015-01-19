using System.Web.Mvc;
using Repository;

namespace Notocol.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult AboutUs()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult MyTags()
        {
            TagRepository objTagRepository = new TagRepository();

            return View(objTagRepository.SearchTags("",1));
        }

        public ActionResult HowItWorks()
        {
            ViewBag.Title = "How it works";
            return View();
        }

    }
}
