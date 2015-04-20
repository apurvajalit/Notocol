using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

using System.Transactions;
namespace Repository
{
    public class TagRepository : BaseRepository
    {
        public TagRepository()
        {
            CreateDataContext();
        }
        /// <summary>
        /// Method to search Tags. Used for autocomplete.
        /// </summary>
        /// <param name="charactersToSearch"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IList<Tag> SearchTags(string charactersToSearch,long userID)
        {
            IList<Tag> lstTags = null;
            try
            {
                using (GetDataContext())
                {
                    if (charactersToSearch == "")
                    {
                        lstTags = (from tags in context.Tags
                                   where tags.UserID == userID
                                   select tags).ToList();
                    }
                    else
                    {
                        lstTags = (from tags in context.Tags
                                   where tags.Name.Contains(charactersToSearch) && tags.UserID == userID
                                   select tags).ToList();
                    }

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
            return lstTags;
        }
        /// <summary>
        /// Method used to Save Tags
        /// </summary>
        /// <param name="objTag"></param>
        /// <returns></returns>
        public Tag SaveTag(Tag objTag)
        {
            try
            {
                using (GetDataContext())
                {
                    context.Entry(objTag).State = objTag.ID == 0 ? EntityState.Added : EntityState.Modified;
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
            return objTag;
        }

        /// <summary>
        /// Save collection of tags with bulkinsert
        /// </summary>
        /// <param name="lstTag"></param>
        /// <returns></returns>
        public IList<Tag> SaveTags(long userID, IList<Tag> lstTag)
        {
            try {
                using (GetDataContext())
                {
                    foreach (Tag objTag in lstTag)
                    {
                        objTag.UserID = userID;
                        context.Entry(objTag).State = objTag.ID == 0 ? EntityState.Added : EntityState.Modified;

                    }
                    context.SaveChanges();

                    // By implementing Bulkinsert plugin
                    //using (var transactionScope = new TransactionScope())
                    //{
                    //    context.BulkInsert(lstTag);
                    //    context.SaveChanges();
                    //    transactionScope.Complete();
                    //}
                }
            }
            catch
            {
                throw;
            }
            return lstTag;
        }

        /// <summary>
        /// Method used to Delete Tags
        /// </summary>
        /// <param name="objTag"></param>
        /// <returns></returns>
        public bool DeleteTag(Tag objTag)
        {
            try
            {
                using (GetDataContext())
                {
                    context.Tags.Attach(objTag); // connect to tag to delete
                    context.Tags.Remove(objTag);
                    context.SaveChanges();
                    return true;
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
            return false;
        }

    }
}