using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository
{
    public class TagRepository : BaseRepository
    {
        public TagRepository()
        {
            CreateDataContext();
        }

        public IList<Tag> SearchTags(string charactersToSearch)
        {
            IList<Tag> lstTags = null;
            try
            {
                using (GetDataContext())
                {
                    lstTags = (from tags in context.Tags
                                          where tags.Name == charactersToSearch
                                          select tags).ToList();
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