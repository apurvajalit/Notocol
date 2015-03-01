using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using EntityFramework.BulkInsert.Extensions;
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
                // Save Source
                using (GetDataContext())
                {
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
                    //using (var transactionScope = new TransactionScope())
                    //{
                    //    context.BulkInsert(lstSourceTags);
                    //    context.SaveChanges();
                    //    transactionScope.Complete();
                    //}
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
    }
}
