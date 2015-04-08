using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Model;
using Notocol.Models;

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

        public long getSourceID(string pageURL, long userID)
        {
            long sourceID = 0;
            IList<Source> lstSource = null;

            try
            {
                using (GetDataContext()) { 
                    lstSource = (from sources in context.Sources
                                 where sources.UserID == userID && sources.Link == pageURL
                                 select sources).ToList();
                }
            }
            catch
            {
                throw;
            }
            finally
            {

            }

            if (lstSource.Count() > 0)
                sourceID = lstSource.First().ID;

            return sourceID;

        }

        public SourceDataForExtension getSourceData(string pageURL, long userID)
        {
            SourceDataForExtension sourceData = new SourceDataForExtension();
            try
            {
                using (GetDataContext())
                {
                    IList<Source> sources = (from Sources in context.Sources
                                         where Sources.UserID == userID && Sources.Link == pageURL  
                                         select Sources).ToList();
                    if(sources.Count > 0){
                        sourceData.Source = new SourceDetails();
                        sourceData.Source.id = sources[0].ID;
                        sourceData.Source.url = sources[0].Link;
                        IList<Tag> tags = (from Tags in context.Tags
                                           where Tags.SourceTags.FirstOrDefault().SourceID == sourceData.Source.id
                                           select Tags).ToList();
                        if (tags.Count() > 0) {
                            sourceData.Tags = new List<TagDetails>();
                            foreach (Tag tag in tags)
                            {
                                TagDetails tagDetails = new TagDetails();
                                tagDetails.id = tag.ID;
                                tagDetails.tagName = tag.Name;
                                sourceData.Tags.Add(tagDetails);
                            }
                        }
                        IList<Annotation> annotations = (from Annotations in context.Annotations
                                                  where Annotations.Source.ID == sourceData.Source.id
                                                  select Annotations).ToList();
                        sourceData.Annotations = new List<AnnotationDetails>();
                        if(annotations.Count() > 0){
                            foreach (var objAnnotation in annotations)
                            {
                                AnnotationDetails annDetails = new AnnotationDetails();
                                annDetails.annotationID = objAnnotation.ID;
                                annDetails.quote = objAnnotation.Quote;
                                annDetails.text = objAnnotation.Text;
                                annDetails.range = AnnotationRange.AnnotationRangeListFromString(objAnnotation.Ranges).FirstOrDefault() ;

                                sourceData.Annotations.Add(annDetails);
                            }
 
                        }
                        
                    } 
                }

            }
            catch
            {
                throw;
            }
            return sourceData;

        }

        public IList<Source> GetSource(long userID)
        {
            IList<Source> lstSources = null;
            try
            {
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
