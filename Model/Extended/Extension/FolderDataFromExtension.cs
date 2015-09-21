using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class FolderDataFromExtensionSelectedFolder
    {
        public long folderID { get; set; }
        public string folderName { get; set; }
    }

    public class FolderDataFromExtension
    {
        public FolderDataFromExtensionSelectedFolder selectedFolder { get; set; }


        public FolderTree addedFolders { get; set; }
    }
}
