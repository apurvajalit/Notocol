using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class FolderRepository : BaseRepository
    {
        public Folder AddFolder(Folder folder)
        {
            try
            {
                using (GetDataContext())
                {
                    folder.created = folder.updated = DateTime.Now;
                    context.Entry(folder).State = EntityState.Added;
                    context.SaveChanges();

                }
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            //}
            return folder;
        }

        public bool DeleteFolder(Folder folder)
        {
            if (folder.ID <= 0) return false;
            try
            {
                using (GetDataContext())
                {
                    context.Entry(folder).State = EntityState.Deleted;
                    context.SaveChanges();

                }
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            //}
            return true;
        }

        public Folder GetFolder(long userID, string name)
        {
            Folder folder = null;
            try
            {
                using (GetDataContext())
                {
                    IList<Folder> folders = (from folderObjects in context.Folders
                                             where folderObjects.userID == userID && folderObjects.name.Equals(name)
                                    select folder).ToList();

                    if (folders.Count > 0) folder = folders[0];
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            //}
            return folder;
        }

        public Folder GetFolder(long folderID)
        {
            Folder folder = null;
            try
            {
                using (GetDataContext())
                {
                    IList<Folder> folders = (from folderObjects in context.Folders
                                             where folderObjects.ID == folderID 
                                             select folder).ToList();

                    if (folders.Count > 0) folder = folders[0];
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            //}
            return folder;
        }

        public IList<Folder> GetFoldersForUser(long userID)
        {
            IList<Folder> folders;
            try
            {
                using (GetDataContext())
                {
                    folders = (from folderObjects in context.Folders
                                             where folderObjects.userID == userID
                               select folderObjects).ToList();

                    
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            //}
            return folders;

        }

        public Folder UpdateFolder(Folder folder)
        {
            try
            {
                using (GetDataContext())
                {
                    folder.updated = DateTime.Now;
                    context.Entry(folder).State = EntityState.Modified;
                    context.SaveChanges();

                }
            }
            catch
            {
                throw;
            }
            finally
            {
                DisposeContext();
            }
            //}
            return folder;
        }
    }
}
