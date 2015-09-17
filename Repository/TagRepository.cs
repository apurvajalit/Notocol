using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Model;

using System.Transactions;
using System.Data.Entity.Core.Objects;
using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using Repository.Search;

namespace Repository
{
    public class TagRepository : BaseRepository
    {
        public const int MAX_RECENT_TAG_COUNT = 10;
        public TagRepository()
        {
            CreateDataContext();
        }
        private Tag AddTag(Tag objTag)
        {
            objTag.Name = objTag.Name.Trim();
            try
            {
                using (GetDataContext())
                {
                    objTag.updated = DateTime.Now;
                    context.Entry(objTag).State = EntityState.Added;
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
            return objTag;
        }
        public IList<Tag> AddTags(IList<string> lstTagNames)
        {
            IList<Tag> listTag = new List<Tag>();
            try
            {
                using (GetDataContext())
                {
                    foreach (string tagName in lstTagNames)
                    {
                        Tag objTag = new Tag();
                        objTag.updated = DateTime.Now;
                        objTag.Name = tagName.Trim();
                        context.Entry(objTag).State = EntityState.Added;
                    }
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
                //Duplicate Tags handling here
            }
            return listTag;
        }
        public bool DeleteTag(Tag objTag)
        {
            try
            {
                using (GetDataContext())
                {
                    context.Tags.Attach(objTag); // connect to tag to delete
                    context.Tags.Remove(objTag);
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
        }

        public bool DeleteSourceUserTagMapping(long sourceUserID)
        {
            using (GetDataContext())
            {
                try
                {
                    var remove = from sourceTag in context.SourceUserTags
                                 where sourceTag.SourceUserID == sourceUserID
                                 select sourceTag;

                    foreach (var sourceTag in remove)
                    {
                        context.Entry(sourceTag).State = EntityState.Deleted;
                    }
                    context.SaveChanges();
                }catch{

                }finally{
                    DisposeContext();
                }
            }
            return true;
        }

        private long GetTagID(string tagString)
        {
            IList<Tag> lstTags = null;

            try
            {
                using (GetDataContext())
                {
                    if (tagString != "")
                    {

                        lstTags = (from tags in context.Tags
                                   where tags.Name.Equals(tagString)
                                   select tags).ToList();
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

            if (lstTags != null && lstTags.Count != 0) return lstTags[0].ID;
            return 0;
        }

        public IList<String> SearchTagNames(string charactersToSearch)
        {
            IList<String> lstTags = null;
            try{
                using (GetDataContext()){
                    if (charactersToSearch == ""){
                        
                            lstTags = (from tags in context.Tags
                                       where tags.Name.Contains(charactersToSearch)
                                       orderby tags.Name
                                       select tags.Name)
                                       .ToList();
                     }else{
                            lstTags = (from tags in context.Tags
                                       where tags.Name.Contains(charactersToSearch)
                                       orderby tags.Name
                                       select tags.Name).Distinct().ToList();
                     }
                    
                }
            }
            catch{
                throw;
            }finally{
                DisposeContext();
            }
            return lstTags;
        }

        public void UpdateSourceUserTags(SourceUser sourceuser, string[] tagNames)
        {
            IList<long> addedTagIDs = null;
            IList<long> tagIDs = tagNames != null ? GetTagIDs(tagNames.ToArray()): new List<long>();
            using (GetDataContext())
            {

                try
                {
                    List<SourceUserTag> sourceTags = (from sourceTag in context.SourceUserTags
                                                      where sourceTag.SourceUserID == sourceuser.ID
                                                      select sourceTag).ToList();

                    //Remove tags not present anymore
                    List<SourceUserTag> removedTags = (from sourceTag in sourceTags
                                                   where !tagIDs.Contains(sourceTag.TagID)
                                                   select sourceTag).ToList();


                    foreach (var sourceTag in removedTags)
                        context.Entry(sourceTag).State = EntityState.Deleted;

                    addedTagIDs = tagIDs.Except((from sourceTag in sourceTags
                                            select sourceTag.TagID).ToList()).ToList();



                    foreach (long tagID in addedTagIDs)
                    {
                        SourceUserTag objSourceTag = new SourceUserTag();
                        objSourceTag.SourceUserID = sourceuser.ID;
                        objSourceTag.TagID = tagID;
                        context.SourceUserTags.Add(objSourceTag);
                        
                    }
                    context.SaveChanges();
                }
                catch
                {
                    throw;
                }
                UpdateRecentUserTag(sourceuser.UserID, tagIDs);
                long sourceID = (long)sourceuser.SourceID;
                
                if (addedTagIDs.Count > 0)
                {
                    ElasticSearchTest es = new ElasticSearchTest();
                    List<string> tagNamesToAdd = new List<string>();
                    foreach (var id in addedTagIDs)
                    {
                         tagNamesToAdd.Add(tagNames[tagIDs.IndexOf(id)]);
                                
                    }

                    es.AddSourceUserTagsData(sourceID, sourceuser.ID, tagNamesToAdd.ToArray());
                }
            }

        }
                
        /* Gets all the tags associated with a source */
        public IList<Tag> GetTagsForSourceUser(long sourceUserID)
        {
            IList<Tag> tagList = null;
            try
            {
                using(GetDataContext()){
                    tagList = (from tags in context.Tags join sourceTags in context.SourceUserTags
                                                    on tags.ID equals sourceTags.TagID
                                                    where sourceTags.SourceUserID == sourceUserID
                                                    select tags).ToList();
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
            return tagList;
        }
        public List<long> TryGetTagIDs(string[] tagStrings)
        {
            List<long> tagIDList = null;
            try
            {
                using (GetDataContext())
                {
                    tagIDList = (from tags in context.Tags.Where(t => tagStrings.Contains(t.Name))
                                     select tags.ID).ToList();
                    
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
            return tagIDList;
            
        }

        public IList<Tag> GetRecentTags(long userID)
        {
            List<Tag> tags = null;
            using (GetDataContext())
            {
                try
                {
                    tags = (from userTags in context.UserTagUsages.Include("Tag")
                                where userTags.userID == userID
                                select userTags.Tag).ToList();
                }catch{
                    throw; 
                }

            }

            return tags;
        }

        private List<UserTagUsage> GetUserTagUsage(long userID)
        {
            List<UserTagUsage> tags = null;
            using (GetDataContext())
            {
                try
                {
                    tags = (from userTags in context.UserTagUsages
                                where userTags.userID == userID
                            select userTags).OrderBy(x=>x.lastUsed).ToList();
                }
                catch
                {
                    throw;
                }

            }

            return tags;
        }

        public bool UpdateRecentUserTag(long userID, long tagID)
        {
            List<UserTagUsage> userTagUsage = GetUserTagUsage(userID);
            UserTagUsage tagUsage = (from userTag in userTagUsage 
                                     where userTag.tagID == tagID 
                                     select userTag).FirstOrDefault();


            using (GetDataContext()) { 
                if (tagUsage == null)
                {
                    if (userTagUsage.Count == MAX_RECENT_TAG_COUNT)
                        context.Entry(userTagUsage[0]).State = EntityState.Deleted;

                    tagUsage = new UserTagUsage();
                    tagUsage.tagID = tagID;
                    tagUsage.userID = userID;
                    tagUsage.lastUsed = DateTime.Now;
                    context.Entry(tagUsage).State = EntityState.Added;
                }
                else
                {
                    context.Entry(tagUsage).State = EntityState.Modified;
                }
                tagUsage.lastUsed = DateTime.Now;
                context.SaveChanges();
            }
            return true;

        }
        public List<long> GetTagIDs(string[] tagNames)
        {
            
            List<long> tagIDs = new List<long>();

            SqlConnection conn = new SqlConnection(GetDataContext().Database.Connection.ConnectionString);
            conn.Open();

            foreach (string tag in tagNames){
            
                //Following stored procedur get tagIDs for each of the tagnames in tags list
                //If the tag exists, it simply returns
                //else creates a tag and gets the ID
                SqlCommand cmd = new SqlCommand("GetTagID", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter outputIdParam = new SqlParameter("@TagID", SqlDbType.Int)
                {
                   Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputIdParam);
                cmd.Parameters.AddWithValue("@TagName", tag.Trim());
                cmd.ExecuteNonQuery();
                tagIDs.Add(outputIdParam.Value as int? ?? default(int));
            }        
            conn.Close();

            
            return tagIDs;
        }
        public void UpdateAnnotationTags(Annotation annotation, string[] tagNames, long sourceID = 0)
        {

            List<long> tagIDs = tagNames != null ? GetTagIDs(tagNames) : new List<long>();
            List<long> newTagIDs = null;
            using(GetDataContext()){

                 try{
                     List<AnnotationTag> annotationTags = (from annotationTag in context.AnnotationTags
                                  where annotationTag.annotationID == annotation.ID
                                        
                                  select annotationTag).ToList();

                     //Remove tags not present anymore
                     List<AnnotationTag> removedannnotionTags = (from annotationTag in annotationTags
                                  where !tagIDs.Contains(annotationTag.tagID)
                                  select annotationTag).ToList();

                     
                     foreach (var annotationTag in removedannnotionTags)
                        context.Entry(annotationTag).State = EntityState.Deleted;

                     newTagIDs = tagIDs.Except((from annotationTag in annotationTags
                                             select annotationTag.tagID).ToList()).ToList();

                     foreach (var tagId in newTagIDs)
                     {
                         AnnotationTag annotationTag = new AnnotationTag();
                         annotationTag.annotationID = annotation.ID;
                         annotationTag.tagID = tagId;
                         context.Entry(annotationTag).State = EntityState.Added;
                         
                     }
                     
                     context.SaveChanges();


                 }
                 catch
                 {
                     throw;
                 }
                 if (newTagIDs.Count > 0)
                 {
                     ElasticSearchTest es = new ElasticSearchTest();
                     if (sourceID == 0)
                     {
                         SourceRepository sourceRepository = new SourceRepository();
                         sourceID = sourceRepository.GetSourceIDFromSourceUser(annotation.SourceUserID);
                         
                     }
                     if (sourceID > 0)
                     {
                         List<string> tagNamesToAdd = new List<string>();
                         foreach (var id in newTagIDs)
                         {
                             tagNamesToAdd.Add(tagNames[tagIDs.IndexOf(id)]);

                         }

                         es.AddSourceUserTagsData(sourceID, annotation.SourceUserID, tagNamesToAdd.ToArray());
                     }
                 }

                 UpdateRecentUserTag(annotation.UserID, tagIDs);

             }
        }

        private void UpdateRecentUserTag(long userID, IList<long> tagIDs)
        {
            List<UserTagUsage> userTagUsage = GetUserTagUsage(userID);
            
            using (GetDataContext())
            {
                List<long> newTags = new List<long>();
                foreach (var tagId in tagIDs)
                {
                    UserTagUsage tagUsage = (from tags in userTagUsage
                                             where tags.tagID == tagId
                                             select tags).FirstOrDefault();
                    if (tagUsage != null)
                    {
                        tagUsage.lastUsed = DateTime.Now;
                        context.Entry(tagUsage).State = EntityState.Modified;

                    }
                    else newTags.Add(tagId);

                }
                if (newTags.Count > 0){
                    List<UserTagUsage> userTagList = (from tags in userTagUsage
                                     select tags).OrderBy(x=>x.lastUsed).ToList();
                    newTags = newTags.Take(MAX_RECENT_TAG_COUNT).ToList();

                    if (newTags.Count > (MAX_RECENT_TAG_COUNT - userTagList.Count))
                    {
                        //Freeing slots for new tags
                        for (int i = 0; i < (newTags.Count - (MAX_RECENT_TAG_COUNT - userTagList.Count)); i++)
                            context.Entry(userTagList[i]).State = EntityState.Deleted;
                    }
                    foreach (var tagID in newTags)
                    {
                        UserTagUsage  tagUsage = new UserTagUsage();
                        tagUsage.tagID = tagID;
                        tagUsage.userID = userID;
                        tagUsage.lastUsed = DateTime.Now;
                        context.Entry(tagUsage).State = EntityState.Added;
                    }

                    
                }
                
                context.SaveChanges();
            }
        }

        public string[] GetAnnotationTagNames(long annotationID)
        {
            string[] tags = null;
            using (GetDataContext())
            {
                tags = (from annotationTags in context.AnnotationTags.Include("Tag") 
                        where annotationTags.annotationID == annotationID
                        select annotationTags.Tag.Name).ToArray();
            }
            return tags;
        }
    }
}   