using Model;
using Model.Extended;
using Model.Extended.Extension;
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
        public const string USER_ROOT_FOLDER_ID = "0";
        
        private void FillChildrenForNode(FolderTree node, IList<Folder> folders)
        {
            IList<Folder> childFolders = (from folder in folders 
                                          where folder.parentID == Convert.ToInt64(node.ID) 
                                          select folder).ToList();
         
            node.Children = new List<FolderTree>();

            foreach (var folder in childFolders)
            {
                FolderTree childTree = new FolderTree();
                childTree.ID = folder.ID.ToString();
                childTree.Name = folder.name;
                //childTree.ParentID = node.ID;
                childTree.Parent = node;

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

        private long GetOrAddFolderTreeNode(FolderTree folderTree, long userID)
        {
            Folder folder = new Folder();
            folder.name = folderTree.Name;
            //folder.parentID = folderTree.ParentID;
            folder.parentID = Convert.ToInt64(folderTree.Parent.ID);
            folder.created = folder.updated = DateTime.Now;
            folder.userID = userID;
            if (GetOrAddFolder(ref folder)) return folder.ID;
            else return 0;
        }
        public FolderTree AddFolderTree(FolderTree newFolderTree, long userID)
        {
            long folderID = 0;
            if ((folderID = GetOrAddFolderTreeNode(newFolderTree, userID)) > 0)
            {
                newFolderTree.ID = folderID.ToString();
                if (newFolderTree.Children != null)
                {
                    int i = 0;
                    
                    for (; i < newFolderTree.Children.Count; i++)
                    {
                        FolderTree temp = null;
                        //newFolderTree.Children[i].ParentID = folderID;
                        newFolderTree.Children[i].Parent = newFolderTree;
                        if ((temp = AddFolderTree(newFolderTree.Children[i], userID)) == null) return null;
                        newFolderTree.Children[i] = temp;
                    }
                    
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

        public long GetFolderID(string folderName, FolderTree folderTree)
        {
            FolderTree currenFolderTree = null;
            Stack<FolderTree> folderTreesToCheck = new Stack<FolderTree>();
            folderTreesToCheck.Push(folderTree);
            do
            {
                currenFolderTree = folderTreesToCheck.Pop();
                if (currenFolderTree.Name == folderName) return Convert.ToInt64(currenFolderTree.ID);
                if (currenFolderTree.Children != null && currenFolderTree.Children.Count > 0)
                {
                    foreach (var child in currenFolderTree.Children) folderTreesToCheck.Push(child);
                }
            } while (folderTreesToCheck.Count > 0);

            return 0;
        }

        public string GetFolderName(long folderID, long userID)
        {
            Folder folder = null;
            if((folder = folderRepository.GetFolder(folderID)) != null) return folder.name;

            return null;
 
        }
        public bool GetOrAddFolder(ref Folder folder)
        {
            Folder checkFolder = folderRepository.GetFolderUnderParent((long)folder.userID, folder.name, folder.parentID);

            if (checkFolder != null)
            {
                folder = checkFolder;
                return true;
            }
            folder = folderRepository.AddFolder(folder);
            if (folder == null || folder.ID <= 0) return false;
            return true;
        }

        //internal FolderDataFromExtension ProcessExtensionFolderData(FolderDataFromExtension folderDataFromExtension, long userID)
        //{
            
        //    if (folderDataFromExtension.addedFolders != null)
        //    {
        //        folderDataFromExtension.addedFolders = AddFolderTree(folderDataFromExtension.addedFolders, userID);
        //    }

        //    if (folderDataFromExtension.selectedFolder.folderID == "0" && folderDataFromExtension.selectedFolder.folderName != null)
        //    {
        //        //Selected folder is amongst the new folders
        //        long folderID = 0;
        //        if ((folderID = GetFolderID(folderDataFromExtension.selectedFolder.folderName, folderDataFromExtension.addedFolders)) > 0)
        //        {
        //            folderDataFromExtension.selectedFolder.folderID = folderID.ToString();
        //        }
                
        //    }

        //    return folderDataFromExtension;
            
        //}

        public IList<Folder> GetUserFolders(long userID)
        {
            return folderRepository.GetFoldersForUser(userID);
        }

        public FolderTree CheckAndHandleFolderTreeAddition(FolderTree folderTree, long userID, ref string requiredFolderID){
            
            if(folderTree.ID.Contains("a")){
                Folder folder = new Folder();
                folder.name = folderTree.Name;
                folder.parentID = Convert.ToInt64(folderTree.Parent.ID);
                folder.userID = userID;
                folder.created = DateTime.Now;
                folder.updated = DateTime.Now;
                folder = folderRepository.AddFolder(folder);
                folderTree.ID = folder.ID.ToString();
                if (requiredFolderID != null && folderTree.ID == requiredFolderID)
                {
                    requiredFolderID = folderTree.ID;
                }
            }

            if(folderTree.Children != null && folderTree.Children.Count > 0){
                foreach (var child in folderTree.Children)
                {
                    child.Parent = folderTree;
                    CheckAndHandleFolderTreeAddition(child, userID, ref requiredFolderID);

                }
            }

            return folderTree;
            
        }

        public string HandleNewUserFolders(long userID, FolderTree folderTree, string requiredFolderID){
            string reqFolderId = null;
            CheckAndHandleFolderTreeAddition(folderTree, userID, ref reqFolderId);

            return reqFolderId;
        }


        public Dictionary<string, long> HandleNewAddedFolders(List<FolderTree> newFolderslist, long userID)
        {
            Dictionary<string, long> addedFolderIDs = new Dictionary<string, long>();
            int i = 0;
            for (; i < newFolderslist.Count; i++)
            {
                Folder folder = new Folder();
                if(newFolderslist[i].ParentID.Contains("a")){
                    long pid = 0;
                    if(addedFolderIDs.TryGetValue(newFolderslist[i].ParentID, out pid))
                        folder.parentID = Convert.ToInt64(pid);
                    else break;

                }
                else
                {
                    folder.parentID = Convert.ToInt64(newFolderslist[i].ParentID);
                }
                folder.name = newFolderslist[i].Name;
                folder.created = folder.updated = DateTime.Now;
                folder.userID = userID;
                folder = folderRepository.AddFolder(folder);
                if(folder == null || folder.ID <= 0) break;
                addedFolderIDs.Add(newFolderslist[i].ID, folder.ID);

            }
            return addedFolderIDs;
        }
    }
}
