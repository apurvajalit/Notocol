using Model;
using Model.Extended;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class FolderHelper
    {
        FolderRepository folderRepository = new FolderRepository();
        public const long USER_ROOT_FOLDER_ID = -1;
        
        private void FillChildrenForNode(FolderTree node, IList<Folder> folders)
        {
            IList<Folder> childFolders = (from folder in folders 
                                          where folder.parentID == node.ID 
                                          select folder).ToList();
         
            node.Children = new List<FolderTree>();

            foreach (var folder in childFolders)
            {
                FolderTree childTree = new FolderTree();
                childTree.ID = folder.ID;
                childTree.Name = folder.name;
                childTree.ParentID = node.ID;

                folders.Remove(folder);
                FillChildrenForNode(childTree, folders);
                node.Children.Add(childTree);
            }

        }

        public FolderTree GetUserFolderTree(long userID)
        {
            
            FolderTree userFolderTree = new FolderTree();

            IList<Folder> folders = folderRepository.GetFoldersForUser(userID);
            Folder folder = new Folder();
            userFolderTree.ID = USER_ROOT_FOLDER_ID;
            FillChildrenForNode(userFolderTree, folders);


            return userFolderTree;

        }

        private long AddFolderTreeNode(FolderTree folderTree, long userID)
        {
            Folder folder = new Folder();
            folder.name = folderTree.Name;
            folder.parentID = folderTree.ParentID;
            folder.created = folder.updated = DateTime.Now;
            folder.userID = userID;
            if (AddFolder(ref folder)) return folder.ID;
            else return 0;
        }
        public FolderTree AddFolderTree(FolderTree newFolderTree, long userID)
        {
            long folderID = 0;
            if ((folderID = AddFolderTreeNode(newFolderTree, userID)) > 0)
            {
                newFolderTree.ID = folderID;
                foreach (var foldertree in newFolderTree.Children)
                {
                    foldertree.ParentID = folderID;
                    if (AddFolderTree(newFolderTree, userID) == null) return null;
                }
                return newFolderTree;
            }
            else return null;
            
        }

        public bool DeleteFolder(Folder folder, long userID){
            return false;
        }

        public Folder UpdateFolder(Folder folder, long userID)
        {
            return folder;
        }

        public bool AddFolder(ref Folder folder)
        {
            Folder checkFolder = folderRepository.GetFolderUnderParent((long)folder.userID, folder.name, folder.parentID);
            long pid = folder.parentID;
            if (checkFolder != null) return false;

            folder = folderRepository.AddFolder(folder);
            if (folder == null || folder.ID <= 0) return false;
            return true;
        }
    }
}
