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
using Repository.Search;

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

                        lstAnnotations = (from annotations in context.Annotations.Include("AnnotationTags.Tag")
                                          where annotations.SourceID == sourceID && annotations.SourceUserID != 0
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
        
        public Annotation GetAnnotation(long annotationID)
        {

            IList<Annotation> annotation;
            try
            {
                using (GetDataContext())
                {
                    annotation = (from annotations in context.Annotations
                                  where annotations.ID == annotationID && annotations.SourceUserID != 0
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
        public long AddAnnotation(Annotation annotation, SourceUser sourceUser = null, string[] tags = null)
        {
            //TODO Move the commented code to 
            //TagRepository tagRepository = new TagRepository();
            try
            {
                using (GetDataContext())
                {
                    
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

            if (annotation.ID > 0)
            {
                ElasticSearchTest es = new ElasticSearchTest();
                es.UpdateNotesForSource(annotation,true,tags);
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

            //if (annotation.Tags != null) {
            //    tagRepository.UpdateImpliedTagsForSource(annotation.UserID, (long)annotation.SourceID, JsonConvert.DeserializeObject<string[]>(annotation.Tags));
            //}

            ElasticSearchTest es = new ElasticSearchTest();
            es.UpdateNotesForSource(annotation, false);
            
            return true;
        }
        public bool DeleteAnnotation(long annotationId)
        {
            Annotation annotation = GetAnnotation(annotationId);
            if (annotation == null)
                return false;

            try
            {
                using (GetDataContext())
                {
                    context.DeleteAnnotation(annotation.ID);
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

        public List<Annotation> GetAnnotationsForPage(string uri, long userID)
        {
            List<Annotation> annotation = new List<Annotation>();
            try
            {
                using (GetDataContext())
                {
                    annotation = (from annotations in context.Annotations
                                  where annotations.UserID == userID && 
                                        annotations.SourceUserID != 0 && 
                                        annotations.Uri == uri
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
            return annotation;
        }

        public IList<Annotation> GetAnnotationsForPageUserAtTop(long sourceID, long userID)
        {
            List<Annotation> annotation = new List<Annotation>();
            try
            {
                using (GetDataContext())
                {
                    annotation = (from annotations in context.Annotations
                                                             .Include("AnnotationTags.Tag")
                                  where annotations.SourceID == sourceID && annotations.SourceUserID != 0
                                  orderby annotations.UserID == userID ? 1 : 0 descending
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
            return annotation;
        }

        public List<Annotation> GetAnnotationWithUserForSource(long sourceID)
        {
            List<Annotation> ann = new List<Annotation>();

            try
            {
                using(GetDataContext())
                {
                    ann = (from notes in context.Annotations
                           .Include("User1")
                           .Include("AnnotationTags.Tag")
                           where notes.SourceID == sourceID && notes.SourceUserID != 0
                           orderby notes.UserID
                           select notes).ToList();
                }
            }
            catch
            {
                throw;
            }
            return ann;
        }
    }
}
