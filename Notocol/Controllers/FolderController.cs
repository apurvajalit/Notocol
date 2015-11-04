using Business;
using Model.Extended;
using Notocol.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Notocol.Controllers
{
    public class FolderController : BaseController
    {
        // GET: Folder
        public JsonResult GetUserFolderTree()
        {
            FolderHelper helper = new FolderHelper();
            FolderTree tree = helper.GetUserFolderTree(Utility.GetCurrentUserID());
            return Json(tree.children, JsonRequestBehavior.AllowGet);
        }
    }
}