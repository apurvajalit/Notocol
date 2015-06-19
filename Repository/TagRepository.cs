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
namespace Repository
{
    public class TagRepository : BaseRepository
    {
        public TagRepository()
        {
            CreateDataContext();
        }
        /// <summary>
        /// Method to search Tags. Used for autocomplete.
        /// </summary>
        /// <param name="charactersToSearch"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// 
        public long checkTag(long userID, string tagString){
            IList<Tag> lstTags = null;
          
            try
            {
                using (GetDataContext())
                {
                    if (tagString != "")
                    {
                        
                        lstTags = (from tags in context.Tags
                                   where tags.Name.Equals(tagString) && tags.UserID == userID
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
            if(lstTags != null && lstTags.Count !=0 ) return lstTags[0].ID;
            return 0;
        }
        

        public IList<String> SearchTags(string charactersToSearch, long userID = -1)
        {
            IList<String> lstTags = null;
            try
            {
                using (GetDataContext())
                {
                    if (charactersToSearch == "")
                    {
                        if(userID > -1){
                            lstTags = (from tags in context.Tags
                                       where tags.UserID == userID && tags.Name.Contains(charactersToSearch)
                                       orderby tags.Name

                                       select tags.Name)
                                       .ToList();
                        }
                        else
                        {
                            lstTags = (from tags in context.Tags
                                       where tags.Name.Contains(charactersToSearch)
                                       orderby tags.Name
                                       select tags.Name).Distinct().ToList();
                        }
                    }
                    else
                    {
                        if (userID > 1) { 
                            lstTags = (from tags in context.Tags
                                   where tags.Name.Contains(charactersToSearch) && tags.UserID == userID
                                   select tags.Name).ToList();
                        }
                        else
                        {
                            lstTags = (from tags in context.Tags
                                       where tags.Name.Contains(charactersToSearch)
                                       select tags.Name).Distinct().ToList();
                        }
                    }

                }
            }catch{
                throw;
            }
            finally
            {
                DisposeContext();
            }
            return lstTags;
        }
        /// <summary>
        /// Method used to Save Tags
        /// </summary>
        /// <param name="objTag"></param>
        /// <returns></returns>
        public Tag SaveTag(Tag objTag)
        {
            objTag.Name = objTag.Name.Trim();
            try
            {
                using (GetDataContext())
                {
                    context.Entry(objTag).State = objTag.ID == 0 ? EntityState.Added : EntityState.Modified;
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

        /// <summary>
        /// Save collection of tags with bulkinsert
        /// </summary>
        /// <param name="lstTag"></param>
        /// <returns></returns>
        public IList<Tag> SaveTags(long userID, IList<Tag> lstTag)
        {
            try {
                using (GetDataContext())
                {
                    foreach (Tag objTag in lstTag)
                    {
                        if (objTag.ID == 0) { 
                            objTag.UserID = userID;
                            objTag.Name = objTag.Name.Trim();
                            context.Entry(objTag).State = EntityState.Added;
                        }

                    }
                    context.SaveChanges();

                    // By implementing Bulkinsert plugin
                    // using (var transactionScope = new TransactionScope())
                    // {
                    //    context.BulkInsert(lstTag);
                    //    context.SaveChanges();
                    //    transactionScope.Complete();
                    // }
                }
            }
            catch
            {
                throw;
                //Duplicate Tags handling here
            }
            return lstTag;
        }

        /// <summary>
        /// Method used to Delete Tags
        /// </summary>
        /// <param name="objTag"></param>
        /// <returns></returns>
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

        public IList<Tag> AddIfNotExistTags(long userID, long sourceID, IList<Tag> tagList)
        {

            for (int i = 0; i < tagList.Count; i++)
            {
                if (tagList[i].ID == 0)
                {
                    long ID = 0;
                    //Add the tag
                    //TODO Currentl all IDs are 0, hence checking string for all tags
                    if ((ID = checkTag(userID, tagList[i].Name)) == 0) tagList[i] = SaveTag(tagList[i]);
                    else tagList[i].ID = ID;

                }
                
            }
            return tagList;
        }

        private void UpdateSourceTagMapping(long sourceID, IList<long> tagIDs)
        {
            try
            {
                foreach (long tagID in tagIDs)          
                {
                    using (GetDataContext()){
                        SourceTag objSourceTag = new SourceTag();
                        objSourceTag.SourceID = sourceID;
                        objSourceTag.TagsID = tagID;
                        context.SourceTags.Add(objSourceTag);
                        try
                        {

                            context.SaveChanges();
                        }
                        catch (Exception)
                        {
                            
                            //ignore if duplicate mapping added
                        }
                        finally
                        {
                            
                        }
                    }
                    
                }
            }
            catch (Exception)
            {
                throw;

            }
            finally
            {
                DisposeContext();
            }

        }
     
        public void AddUserTagsToSource(long userID, long sourceID, string[] tags)
        {
            List<long> tagIDs = new List<long>();
            if (tags == null) return;
            try{
                using (SqlConnection conn = new SqlConnection(GetDataContext().Database.Connection.ConnectionString)) {
                    conn.Open();
                           
                    foreach (string tag in tags)
                    {
                        SqlCommand cmd = new SqlCommand("GetTagID", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter outputIdParam = new SqlParameter("@TagID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        
                        cmd.Parameters.Add(outputIdParam);
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@TagName",  tag.Trim());

                        cmd.ExecuteNonQuery();
                        tagIDs.Add(outputIdParam.Value as int? ?? default(int));
                        
                     
                    }
                    conn.Close();
                    UpdateSourceTagMapping(sourceID, tagIDs);

                }
            }catch{
                throw;
            }
            
        }


        public void UpdateUserTagsForSource(long userID, long sourceID, string[] tags)
        {
            AddUserTagsToSource(userID, sourceID, tags);
        }


        public IList<Tag> GetTagsForSource(long sourceID)
        {
            IList<Tag> tagList = null;
            try
            {
                using(GetDataContext()){
                    tagList = (from tags in context.Tags join sourceTags in context.SourceTags
                                                    on tags.ID equals sourceTags.TagsID
                                                    where sourceTags.SourceID == sourceID
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


        public IList<long> GetTagIDs(string[] tagStrings, long userID = -1)
        {
            IList<long> tagIDList = null;
            try
            {
                using (GetDataContext())
                {
                    if (userID > -1) {
                        tagIDList = (from tags in context.Tags.Where(t => t.UserID == userID && tagStrings.Contains(t.Name))
                               select tags.ID).ToList();
                    }
                    else
                    {
                        tagIDList = (from tags in context.Tags.Where(t => tagStrings.Contains(t.Name))
                                     select tags.ID).ToList();
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
            return tagIDList;
            
        }
    }
}   