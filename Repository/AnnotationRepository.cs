using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data.Entity;

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

        public IList<string> GetAnnotations(string pageURL, long userID=2)
        {
            IList<string> lstAnnotations = null;
            
            try
            {
                
                using (GetDataContext())
                {
                    long sourceID = getSourceID(pageURL, userID);
                    if(sourceID != 0){
                        lstAnnotations = (from annotations in context.Annotations
                                          where annotations.SourceID == sourceID
                                          select annotations.Data).ToList();
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
        
        public Annotation CreateAnnotation(string pageURL, string objAnnotationData, long userID=2)
        {
            Annotation objAnnotation = new Annotation();
            try
            {
                using (GetDataContext())
                {
                    objAnnotation.SourceID = getSourceID(pageURL, userID);
                    
                    objAnnotation.Data = objAnnotationData;
                    context.Entry(objAnnotation).State = EntityState.Added;
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
            return objAnnotation;
        }

        public IList<Annotation> SaveAnnotations(IList<Annotation> lstAnnotation)
        {
            using (GetDataContext())
            {
                foreach (Annotation objAnnotation in lstAnnotation)
                {
                    context.Entry(objAnnotation).State = objAnnotation.ID == 0 ? EntityState.Added : EntityState.Modified;
                    
                }
                context.SaveChanges();

                // By implementing Bulkinsert plugin
                //using (var transactionScope = new TransactionScope())
                //{
                //    context.BulkInsert(lstTag);
                //    context.SaveChanges();
                //    transactionScope.Complete();
                //}
            }
            return lstAnnotation;
        }

        public bool DeleteAnnotation(Annotation objAnnotation)
        {
            try
            {
                using (GetDataContext())
                {
                    context.Annotations.Attach(objAnnotation); // connect to annotation DB
                    context.Annotations.Remove(objAnnotation);
                    context.SaveChanges();
                    return true;
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
            return false;
        }

    }
}
