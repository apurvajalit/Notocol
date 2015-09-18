using Model.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Notocol.Models;

namespace Notocol.Controllers.Api
{
    public class FolderController : BaseApiController
    {
        // GET: Folder

        [HttpGet]
        public FolderTree GetUserFolders()
        {
            return new FolderHelper().GetUserFolderTree(Utility.GetCurrentUserID());
        }

        [HttpPost]
        public FolderTree AddNewUserFolder(FolderTree newFolder)
        {
            return new FolderHelper().AddFolderTree(newFolder, Utility.GetCurrentUserID());
        }
    }
}