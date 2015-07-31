using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Model;
//using Notocol.Models;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using Model.Extended;

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

        public Source SaveSource(long userID, Source objSource, IList<Tag> lstTags)
        {
            try
            {
                long sourceID = 0;
                objSource.UserID = userID;
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
                    objSource.ModifiedAt = DateTime.Now;
                    if (objSource.Privacy == null) objSource.Privacy = false;
                    context.Entry(objSource).State = objSource.ID == 0 ? EntityState.Added : EntityState.Modified;
                    context.SaveChanges();
                    sourceID = objSource.ID;
                }
                // save data to Tag table
                TagRepository objTagRepository = new TagRepository();
                objTagRepository.SaveTags(userID, lstTags);
                
                // save data to sourceTag table
                using (GetDataContext())
                {
                    IList<SourceTag> lstSourceTags = new List<SourceTag>();
                    //var alreadyAddedTags = lstTags.Intersect(objSource.SourceTags);
                    //CurrentCollection.AddRange(DownloadedItems.Except(duplicates));
                    
                    var lstSourceTagMapping = (from sourcetag in context.SourceTags
                                         where sourcetag.SourceID == sourceID
                                        select sourcetag).ToList();

                    foreach(var mapping in lstSourceTagMapping) context.SourceTags.Remove(mapping);
                    context.SaveChanges();

                    foreach (Tag objTag in lstTags)
                    {
                        SourceTag objSourceTag = new SourceTag();
                        objSourceTag.SourceID = sourceID;
                        objSourceTag.TagsID = objTag.ID;
                        try { 
                            context.SourceTags.Add(objSourceTag);
                        }
                        catch (Exception)
                        {
                            throw ;
                            //ignore if duplicate tag added
                        }
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

        public long GetSourceIDFromSourceURI(string sourceURI, long userID)
        {
            long sourceID = 0;
            IList<Source> lstSource = null;

            try
            {
                using (GetDataContext()) { 
                    lstSource = (from sources in context.Sources
                                 where sources.UserID == userID && sources.SourceURI == sourceURI
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

        public Source GetSourceFromSourceURI(string sourceURI, long userID)
        {
            
            IList<Source> lstSource = null;

            try
            {
                using (GetDataContext())
                {
                    lstSource = (from sources in context.Sources
                                 where sources.UserID == userID && sources.SourceURI == sourceURI
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
                return lstSource.First();

            return null;

        }
        //public SourceDataForExtension getSourceData(string pageURL, long userID)
        //{
        //    SourceDataForExtension sourceData = new SourceDataForExtension();
        //    try
        //    {
        //        using (GetDataContext())
        //        {
        //            IList<Source> sources = (from Sources in context.Sources
        //                                 where Sources.UserID == userID && Sources.Link == pageURL  
        //                                 select Sources).ToList();
        //            if(sources.Count > 0){
        //                sourceData.Source = new SourceDetails();
        //                sourceData.Source.id = sources[0].ID;
        //                sourceData.Source.url = sources[0].Link;
        //                sourceData.Source.summary = sources[0].Summary;
        //                IList<Tag> tags = (from Tags in context.Tags
        //                                   where Tags.SourceTags.FirstOrDefault().SourceID == sourceData.Source.id
        //                                   select Tags).ToList();
        //                if (tags.Count() > 0) {
        //                    sourceData.Tags = new List<TagDetails>();
        //                    foreach (Tag tag in tags)
        //                    {
        //                        TagDetails tagDetails = new TagDetails();
        //                        tagDetails.id = tag.ID;
        //                        tagDetails.tagName = tag.Name;
        //                        sourceData.Tags.Add(tagDetails);
        //                    }
        //                }
        //                IList<Annotation> annotations = (from Annotations in context.Annotations
        //                                          where Annotations.Source.ID == sourceData.Source.id
        //                                          select Annotations).ToList();
        //                sourceData.Annotations = new List<AnnotationDetails>();
        //                if(annotations.Count() > 0){
        //                    foreach (var objAnnotation in annotations)
        //                    {
        //                        AnnotationDetails annDetails = new AnnotationDetails();
        //                        annDetails.annotationID = objAnnotation.ID;
        //                        annDetails.quote = objAnnotation.Quote;
        //                        annDetails.text = objAnnotation.Text;
        //                        annDetails.range = AnnotationRange.AnnotationRangeListFromString(objAnnotation.Ranges).FirstOrDefault() ;

        //                        sourceData.Annotations.Add(annDetails);
        //                    }
 
        //                }
                        
        //            } 
        //        }

        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return sourceData;

        //}

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

        public IList<SourceItem> Search(string keyword, IList<long> tagIDs, long userID = -1)
        {

            IList<SourceItem> sourceList = new List<SourceItem>();           
            using (var con = new SqlConnection(GetDataContext().Database.Connection.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SearchForSource", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keywordStr", keyword);
                    if(userID > -1)cmd.Parameters.AddWithValue("@userID", userID);


                    var table = new DataTable();
                    table.Columns.Add("TagID", typeof(long));

                    foreach(long id in tagIDs)
                        table.Rows.Add(id);

                    var pList = new SqlParameter("@TagIDList", SqlDbType.Structured);
                    pList.TypeName = "dbo.TagIDList";
                    pList.Value = table;

                    cmd.Parameters.Add(pList);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SourceItem source = new SourceItem();
                            source.ID = Convert.ToInt64(dr["ID"].ToString());
                            source.Title = dr["Title"].ToString();
                            source.Link = dr["Link"].ToString();
                            source.Summary = dr["Summary"].ToString();
                            source.TagNames = dr["TagNames"].ToString();
                            source.TagIDs = dr["TagIDs"].ToString();
                            source.UserName = (dr["Username"].ToString());
                            sourceList.Add(source);
                                
                        }

                    }
                }
            }
            return sourceList;
        }

        

        //public void UpdateTags(long userID, long sourceID, IList<Tag> tags)
        //{
        //    TagRepository objTagRepository = new TagRepository();
        //    //Create tags that do not exist
        //    tags = objTagRepository.AddIfNotExistTags(userID, sourceID, tags);
        //    //Add mappings if they don't exist
        //    UpdateSourceTagMapping(sourceID, tags);
        //}


        public long AddSourceIfNotFound(Source source)
        {
            long sourceID = GetSourceIDFromSourceURI(source.SourceURI, source.UserID);
            if (sourceID <= 0)
            {
                try
                {
                    using (GetDataContext())
                    {
                        source.ModifiedAt = DateTime.Now;
                        if (source.Privacy == null) source.Privacy = false;
                        context.Entry(source).State = EntityState.Added;
                        context.SaveChanges();
                        sourceID = source.ID;
                    }
                }catch
                {
                    throw;
                }
                finally
                {
                    DisposeContext();
                }
            }
            return sourceID;
        }

        public void UpdateSourcePrivacy(Source source, bool pagePrivate)
        {
            try
            {
                using (GetDataContext())
                {
                    
                        source.ModifiedAt = DateTime.Now;
                        source.Privacy = pagePrivate;
                        context.Entry(source).State = source.ID == 0 ? EntityState.Added : EntityState.Modified;
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
        }

        public bool DeleteSource(Source source)
        {
            AnnotationRepository annRepo = new AnnotationRepository();
            TagRepository tagRepo = new TagRepository();
            try
            {
                using (GetDataContext())
                {
                    context.DeleteSource(source.ID);
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
            return true;
        }

        public Source GetExistingSource(long sourceID)
        {
            IList<Source> lstSource = null;

            try
            {
                using (GetDataContext())
                {
                    lstSource = (from sources in context.Sources
                                 where sources.ID == sourceID
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
                return lstSource.First();

            return null;
        }
    }
}
