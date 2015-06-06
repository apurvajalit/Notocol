using Notocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{
    public class SourceController : Controller
    {
        // GET: Source
        public ActionResult SourceItems(string keywordFilter = "", string tagString = "")
        {

            long userID = Utility.GetCurrentUserID();
            return PartialView(SourceHelper.GetSourceItems(keywordFilter, tagString, userID));
        }
    }
}