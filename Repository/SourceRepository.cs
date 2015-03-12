using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Model;

namespace Repository
{
    public class SourceRepository : BaseRepository
    {
        public SourceRepository()
        {
            CreateDataContext();
        }

        /// <summary>
        /// Use to save Source
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="lstTags"></param>
        /// <returns></returns>

        public Source SaveSource(Source objSource, IList<Tag> lstTags)
        {
            try
            {
                long sourceID = 0;
                string tagNames="";
               // get tag names to assign to source.
                foreach (Tag objTag in lstTags)
                {
                    tagNames += ","+ objTag.Name;
                }

                // check for 0 length
                tagNames = tagNames.Length > 0 ? tagNames.Remove(0,1):"";
 // Save Source
                using (GetDataContext())
                {
                    objSource.TagNames = tagNames;
                    context.Entry(objSource).State = objSource.ID == 0 ? EntityState.Added : EntityState.Modified;
                    context.SaveChanges();
                    sourceID = objSource.ID;
                }
                // save data to Tag table
                TagRepository objTagRepository = new TagRepository();
                objTagRepository.SaveTags(lstTags);

                // save data to sourceTag table
                using (GetDataContext())
                {
                    IList<SourceTag> lstSourceTags = new List<SourceTag>();

                    foreach (Tag objTag in lstTags)
                    {
                        SourceTag objSourceTag = new SourceTag();
                        objSourceTag.SourceID = sourceID;
                        objSourceTag.TagsID = objTag.ID;
                        context.SourceTags.Add(objSourceTag);
                        objSourceTag = null;
                    }
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

            return objSource;
        }

        public IList<Source> GetSource(int userID)
        {
            IList<Source> lstSources = null;
            try
            {
                long sourceID = 0;
                // Save Source
                using (GetDataContext())
                {
                    lstSources = (from sources in context.Sources
                        where sources.UserID == userID
                        select sources).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return lstSources;
        }
    }
}
