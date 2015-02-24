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
        public Source SaveSource(UserSourceTags objUserSourceTags)
        {
            try
            {
                SourceTag objSourceTag = new SourceTag();
                using (GetDataContext())
                {
                    context.Sources.Add(objUserSourceTags.Source);
                    // Save Source in Database
                    context.SaveChanges();

                    // loop for all the tags assigned
                    foreach (Tag objTag in objUserSourceTags.Tags) // Loop over the Tags.
                    {
                        var tag = (from s in context.Tags where s.Name == objTag.Name && s.UserID == objTag.UserID select s)
                                .FirstOrDefault();
                        tag = context.Tags.Add(tag);

                        objSourceTag.TagsID = tag.ID;
                        objSourceTag.SourceID = objUserSourceTags.Source.ID;
                        context.SourceTags.Add(objSourceTag);
                    }
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

            return objUserSourceTags.Source;
        }
    }
}
