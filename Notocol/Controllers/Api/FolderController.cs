using Model.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Notocol.Models;
using Model;

namespace Notocol.Controllers.Api
{
    public class FolderController : BaseApiController
    {
        // GET: Folder

        [HttpGet]
        public IList<Folder> GetUserFolders()
        {
            return new FolderHelper().GetUserFolders(Utility.GetCurrentUserID());
        }

        [HttpPost]
        public FolderTree AddNewUserFolder(FolderTree newFolder)
        {
            return new FolderHelper().AddFolderTree(newFolder, Utility.GetCurrentUserID());
            
        }
    }
}