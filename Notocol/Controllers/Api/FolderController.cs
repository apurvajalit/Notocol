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
        public FolderTreeRecursive AddNewUserFolder(FolderTreeRecursive newFolder)
        {
            return new FolderHelper().AddFolderTree(newFolder, Utility.GetCurrentUserID());
            
        }

        [HttpGet]
        public IList<string> GetUserFolderNameSuggestions(string query)
        {
            return new FolderHelper().GetUserFolderNameSuggestions(query, Utility.GetCurrentUserID());
        }

    }
}