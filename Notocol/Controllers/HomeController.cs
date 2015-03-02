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
           SaveSource(null, null);
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
        public ActionResult Home()
        {
            ViewBag.Title = "Home";
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

        public long AddTag(Tag objTag)
        {
            objTag.UserID = 2;
            TagRepository objTagRepository = new TagRepository();
            return objTagRepository.SaveTag(objTag).ID;
        }

        public bool DeleteTag(Tag objTag)
        {
            objTag.UserID = 2;
            TagRepository objTagRepository = new TagRepository();
            return objTagRepository.DeleteTag(objTag);
        }
        /// <summary>
        /// Controller Method to save Source Data
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="lstTags"></param>
        /// <returns></returns>
        public long SaveSource(Source objSource, IList<Tag> lstTags)
        {
            Source objSourceTemp = new Source();
            IList<Tag> lstTagsTemp = new List<Tag>();
            objSourceTemp.Title = "abcd123";
            objSourceTemp.UserID = 2;

            Tag objTag = new Tag();
            objTag.ID = 5;
            objTag.Name = "Project Management";
            objTag.ParentID = 1;
            objTag.UserID = 2;
            lstTagsTemp.Add(objTag);

            Tag objTag1 = new Tag();
            objTag1.ID = 7;
            objTag1.Name = "Javascript";
            objTag1.ParentID = 1;
            objTag1.UserID = 2;
            lstTagsTemp.Add(objTag1);

            Tag objTag2 = new Tag();
            objTag2.ID = 0;
            objTag2.Name = "Widgets";
            objTag2.ParentID = 7;
            objTag2.UserID = 2;
            lstTagsTemp.Add(objTag2);

            SourceRepository obSourceRepository = new SourceRepository();
            obSourceRepository.SaveSource(objSourceTemp, lstTagsTemp);


            return 0;
        }
    }
}
