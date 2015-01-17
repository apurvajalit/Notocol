using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository
{
    public class SourceRepository : BaseRepository
    {
        public SourceRepository()
        {
            CreateDataContext();
        }
        public Source SaveSource(Source objSource)
        {
            try
            {
                using (GetDataContext())
                {
                    foreach (SourceTag objSourceTag in objSource.SourceTags) // Loop over the Tags.
                    {
                        context.SourceTags.Add(objSourceTag);
                    }
                    context.Sources.Add(objSource);
                    // Save Source in Database
                    context.SaveChanges();
                }
                //Save all context object changes to database. This will act like bulk insert.
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
