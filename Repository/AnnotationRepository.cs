using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;
using Notocol.Models;

namespace Repository
{
    public class AnnotationRepository : BaseRepository
    {
        public AnnotationRepository()
        {
            CreateDataContext();
        }

        private long getSourceID(string pageURL,long userID)
        {
            long sourceID = 0;
            IList<Source> lstSource = null;
            lstSource = (from sources in context.Sources
                         where sources.UserID == userID && sources.Link == pageURL
                         select sources).ToList();
            if (lstSource != null)
                sourceID = lstSource.First().ID;
            
            return sourceID;

        }

        private IList<long> getSourceID(long userID)
        {
            IList<long> sourceID;
            
            sourceID = (System.Collections.Generic.IList<long>)(from sources in context.Sources
                         where sources.UserID == userID
                         select sources.ID);
            

            return sourceID;

        }

        public IList<AnnotationDataResponse> getAnnotations()
        {
            IList<Annotation> lstAnnotations = null;
            IList<AnnotationDataResponse> annData = new List<AnnotationDataResponse>();
            try
            {

                using (GetDataContext())
                {
                    
                    lstAnnotations = (from annotations in context.Annotations
         
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

            foreach (var annotation in lstAnnotations)
            {
                annData.Add(new AnnotationDataResponse(annotation, 101));
            }
            return annData;
        }
        public IList<AnnotationDataResponse> getAnnotations(string pageURL, long userID)
        {
            IList<Annotation> lstAnnotations = null;
            IList<AnnotationDataResponse> annData = new List<AnnotationDataResponse>();
            try
            {
                
                using (GetDataContext())
                {
                //    long sourceID = getSourceID(pageURL, userID);
                      lstAnnotations = (from annotations in context.Annotations
                                          where annotations.User== userID && annotations.Uri == pageURL
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

            foreach (var annotation in lstAnnotations)
            {
                annData.Add(new AnnotationDataResponse(annotation, 1));
            }
            return annData;
        }

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

        public AnnotationDataResponse getAnnotation(long annotationID)
        {
            
            IList<Annotation> annotation;
            try
            {

                using (GetDataContext())
                {
                    //IList<long> sourceID = getSourceID(userID);
                    //if (sourceID.Count != 0)
                    {
                        annotation = (from annotations in context.Annotations
                                          where annotations.ID == annotationID
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
            if (annotation.Count() > 0)
                return new AnnotationDataResponse(annotation[0], 201);
            else
                return null;
        }
        public AnnotationDataResponse createAnnotation(NewAnnotationDataFromRequest annDatareq)
        {
            Annotation annotation = annDatareq.toAnnotation();
            AnnotationDataResponse response = null;
            try
            {
                using (GetDataContext())
                {
                   // objAnnotation.SourceID = getSourceID(annData.uri, annData.user);
                    annotation.SourceID = 8;
                    context.Entry(annotation).State = EntityState.Added;
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

            response = new AnnotationDataResponse(annotation, 301);
            return response;
        }

        //public AnnotationDataResponse updateAnnotations(IList<Annotation> lstAnnotation)
        //{
        //    using (GetDataContext())
        //    {
        //        foreach (Annotation objAnnotation in lstAnnotation)
        //        {
        //            context.Entry(objAnnotation).State = objAnnotation.ID == 0 ? EntityState.Added : EntityState.Modified;
                    
        //        }
        //        context.SaveChanges();

        //        // By implementing Bulkinsert plugin
        //        //using (var transactionScope = new TransactionScope())
        //        //{
        //        //    context.BulkInsert(lstTag);
        //        //    context.SaveChanges();
        //        //    transactionScope.Complete();
        //        //}
        //    }
        //    return 0;
        //}

        public AnnotationDataResponse updateAnnotation(AnnotationDataResponse annData)
        {
            IList<Annotation> annotation;
            try
            {
                using (GetDataContext())
                {
                    annotation = (from annotations in context.Annotations
                                  where annotations.ID == annData.id
                                  select annotations).ToList();
                    if (annotation.Count() > 0)
                    {
                        AnnotationDataResponse.UpdateAnnotationObject(annotation[0], annData);
                        context.Entry(annotation[0]).State = EntityState.Modified;
                        context.SaveChanges();
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
            return annData;
        }

        //public bool deleteAnnotation(AnnotationData annData)
        //{
        //    Annotation objAnnotation = AnnotationData.GetAnnotationObject(annData);

        //    try
        //    {
        //        using (GetDataContext())
        //        {
        //            context.Entry(objAnnotation).State = EntityState.Deleted;
        //            context.SaveChanges();
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
        //    return true;
        //}

    }
}
