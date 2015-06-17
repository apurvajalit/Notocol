using Model;
using Notocol.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{
    public class TagController : Controller
    {
        // GET: Tag
        public JsonResult GetCurrentUserTagNames(string searchStr = "")
        {

            long userID = Utility.GetCurrentUserID(); 
            return Json(TagHelper.GetAllUserTagNames(userID, searchStr), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCurrentUserTags()
        {
            long userID = Convert.ToInt64(Session["userID"]);
            return Json(TagHelper.GetAllUserTags(userID), JsonRequestBehavior.AllowGet);
            

        }
    }
}