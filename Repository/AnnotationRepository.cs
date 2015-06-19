using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;
//using Notocol.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Repository
{
    public class AnnotationRepository : BaseRepository
    {
        public AnnotationRepository()
        {
            CreateDataContext();
        }

        public IList<Annotation> GetAnnotationsForPage(long sourceID)
        {
            IList<Annotation> lstAnnotations = null;
            try
            {

                using (GetDataContext())
                {
         
                    if (sourceID != 0)
                    {

                        lstAnnotations = (from annotations in context.Annotations
                                          where annotations.SourceID == sourceID
                                          select annotations).ToList();
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

            
            return lstAnnotations;
        }
       
        public List<Annotation> getAnnotations(string uri, long userID)
        {
            List<Annotation> lstAnnotations = null;
            
            try
            {
                using (GetDataContext())
                {
                    lstAnnotations = (from annotations in context.Annotations
                                          where annotations.UserID == userID && annotations.Uri == uri 
                                          select annotations).ToList();
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

            return lstAnnotations;
        }

        //public IList<AnnotationDataResponse> getAnnotations()
        //{
        //    IList<Annotation> lstAnnotations = null;
        //    IList<AnnotationDataResponse> annData = new List<AnnotationDataResponse>();
        //    try
        //    {

        //        using (GetDataContext())
        //        {
                    
        //            lstAnnotations = (from annotations in context.Annotations
         
        //                              select annotations).ToList();


        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        DisposeContext();
        //    }

        //    foreach (var annotation in lstAnnotations)
        //    {
        //        annData.Add(new AnnotationDataResponse(annotation));
        //    }
        //    return annData;
        //}
        //public IList<AnnotationDataResponse> getAnnotations(string pageURL, long userID)
        //{
        //    IList<Annotation> lstAnnotations = null;
        //    IList<AnnotationDataResponse> annData = new List<AnnotationDataResponse>();
        //    try
        //    {
                
        //        using (GetDataContext())
        //        {
        //            SourceRepository sourceRepository = new SourceRepository();
        //            long sourceID = sourceRepository.getSourceID(pageURL, userID);
        //            if (sourceID != 0) { 

        //              lstAnnotations = (from annotations in context.Annotations
        //                                  where annotations.SourceID == sourceID
        //                                  select annotations).ToList();
        //            }
                
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        DisposeContext();
        //    }

        //    if ((lstAnnotations != null) && (lstAnnotations.Count() > 0))
        //    {
        //        foreach (var annotation in lstAnnotations)
        //        {
        //            annData.Add(new AnnotationDataResponse(annotation));
        //        }
        //    }
        //    return annData;
        //}

        //public IList<AnnotationData> getAnnotations(long userID)
        //{
        //    IList<Annotation> lstAnnotations = null;
        //    IList<AnnotationData> annData = new List<AnnotationData>();
        //    try
        //    {

        //        using (GetDataContext())
        //        {
        //            //IList<long> sourceID = getSourceID(userID);
        //            //if (sourceID.Count != 0)
        //            {
        //                lstAnnotations = (from annotations in context.Annotations
        //                                 where annotations.User == userID
        //                                  select annotations).ToList();
        //            }

        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        DisposeContext();
        //    }
        //    foreach (var annotation in lstAnnotations)
        //    {
        //        annData.Add(new AnnotationData(annotation));
        //    }
        //    return annData;
            
        //}

        public Annotation getAnnotation(long annotationID)
        {

            IList<Annotation> annotation;
            try
            {
                using (GetDataContext())
                {
                    annotation = (from annotations in context.Annotations
                                  where annotations.ID == annotationID
                                  select annotations).ToList();
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
            if (annotation.Count() > 0)
                return annotation[0];
            else
                return null;
        }

        public int AddAnnotation(Annotation annotation)
        {
            TagRepository tagRepository = new TagRepository();
            try
            {
                using (GetDataContext())
                {
                    SourceRepository sourceRepository = new SourceRepository();
                    long sourceID = sourceRepository.GetSourceIDFromSourceURI(annotation.Uri, annotation.UserID);
                    if (sourceID == 0)
                    {
                        //Create the source for this user
                        Source source = new Source();
                        IList<Tag> tags = new List<Tag>();        
                        source.SourceURI = annotation.Uri;
                        source.UserID = annotation.UserID;
                        dynamic data = JObject.Parse(annotation.Document);
                        source.Title = data.title;
                        source.Link = null;
                        if (annotation.Uri.StartsWith("urn:x-pdf"))
                        {

                            foreach (var link in data.link)
                            {
                                string value = link.href;
                                if (value.StartsWith("http"))
                                {
                                    source.Link = value;
                                    break;
                                }
                            }
                            if (source.Link == null)
                            {
                                source.Link = "localFile:" + data.filename;
                            }
                        }else
                            source.Link = data.link[0].href;
                      
                        try
                        {
                            sourceRepository.SaveSource(annotation.UserID, source, tags);
                            sourceID = source.ID;
                        }
                        catch
                        {
                            throw;
                        }

                    }
                    
                    annotation.SourceID = sourceID;
                    context.Entry(annotation).State = EntityState.Added;
                    context.SaveChanges();
                    if(annotation.Tags != null)
                        tagRepository.AddUserTagsToSource(annotation.UserID, sourceID, JsonConvert.DeserializeObject<string[]>(annotation.Tags));

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

            
            return annotation.ID;
        }

        public bool UpdateAnnotation(Annotation annotation)
        {
            TagRepository tagRepository = new TagRepository();
            try
            {
                using (GetDataContext())
                {
                    context.Entry(annotation).State = EntityState.Modified;
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
            if (annotation.Tags != null) {
                tagRepository.UpdateUserTagsForSource(annotation.UserID, (long)annotation.SourceID, JsonConvert.DeserializeObject<string[]>(annotation.Tags));
            }
            return true;
        }

        ////public bool deleteAnnotation(AnnotationData annData)
        ////{
        ////    Annotation objAnnotation = AnnotationData.GetAnnotationObject(annData);

        ////    try
        ////    {
        ////        using (GetDataContext())
        ////        {
        ////            context.Entry(objAnnotation).State = EntityState.Deleted;
        ////            context.SaveChanges();
        ////        }
        ////    }
        ////    catch
        ////    {
        ////        throw;
        ////    }
        ////    finally
        ////    {
        ////        DisposeContext();
        ////    }
        ////    return true;
        ////}


        public bool DeleteAnnotation(long id, long userID)
        {
            Annotation annotation = getAnnotation(id);
            if (annotation == null || annotation.UserID != userID)
                return false;

            try
            {
                using (GetDataContext())
                {
                    context.Entry(annotation).State = EntityState.Deleted;
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
            return true;
        }
    }
}
