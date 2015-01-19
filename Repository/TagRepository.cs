using System.Collections.Generic;
using System.Linq;
using Model;

namespace Repository
{
    public class TagRepository : BaseRepository
    {
        public TagRepository()
        {
            CreateDataContext();
        }

        public IList<Tag> SearchTags(string charactersToSearch,int userID)
        {
            IList<Tag> lstTags = null;
            try
            {
                using (GetDataContext())
                {
                    if (charactersToSearch=="")
                    {
                        lstTags = (from tags in context.Tags
                                   where tags.UserID == userID
                                   select tags).ToList();
                    }
                    else
                    {
                        lstTags = (from tags in context.Tags
                                   where tags.Name.StartsWith(charactersToSearch) && tags.UserID == userID
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

    }
}