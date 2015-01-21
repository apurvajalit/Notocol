using System.Collections.Generic;
using System.Web.Mvc;
using Model;
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
            IList<Tag> searchTags = SearchMyTags("");
            return View(searchTags);
        }

        public ActionResult HowItWorks()
        {
            ViewBag.Title = "How it works";
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SearchTags(string id)
        {
            return Json(SearchMyTags(id), JsonRequestBehavior.AllowGet);
        }

        public IList<Tag> SearchMyTags(string strSearch)
        {
            TagRepository objTagRepository = new TagRepository();
            IList<Tag> searchTags = objTagRepository.SearchTags(strSearch, 2);
            return searchTags;
        }

        public Tag AddTag(Tag objTag)
        {
            TagRepository objTagRepository = new TagRepository();
            return objTagRepository.SaveTag(objTag);
        }

    }
}
