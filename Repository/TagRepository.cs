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
        public IList<Tag> AddTags(long userID, IList<string> lstTagNames)
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
                        objTag.UserID = userID;
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

        public bool DeleteSourceTagMapping(long sourceID)
        {
            using (GetDataContext())
            {
                try
                {
                    var remove = from sourceTag in context.SourceTags
                                 where sourceTag.SourceID == sourceID
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
        private long GetTagID(long userID, string tagString)
        {
            IList<Tag> lstTags = null;

            try
            {
                using (GetDataContext())
                {
                    if (tagString != "")
                    {

                        lstTags = (from tags in context.Tags
                                   where tags.UserID == userID && tags.Name.Equals(tagString)
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
        public IList<String> SearchTagNames(string charactersToSearch, long userID = -1)
        {
            IList<String> lstTags = null;
            try{
                using (GetDataContext()){
                    if (charactersToSearch == ""){
                        if (userID > -1){
                            lstTags = (from tags in context.Tags
                                       where tags.UserID == userID && tags.Name.Contains(charactersToSearch)
                                       orderby tags.Name

                                       select tags.Name)
                                       .ToList();
                        }
                        else{
                            lstTags = (from tags in context.Tags
                                       where tags.Name.Contains(charactersToSearch)
                                       orderby tags.Name
                                       select tags.Name).Distinct().ToList();
                        }
                    }
                    else{
                        if (userID > 1){
                            lstTags = (from tags in context.Tags
                                       where tags.Name.Contains(charactersToSearch) && tags.UserID == userID
                                       select tags.Name).ToList();
                        }else{
                            lstTags = (from tags in context.Tags
                                       where tags.Name.Contains(charactersToSearch)
                                       select tags.Name).Distinct().ToList();
                        }
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

        private void UpdateSourceTagMapping(long sourceID, IList<long> tagIDs)
        {
            try
            {
                foreach (long tagID in tagIDs)
                {
                    using (GetDataContext())
                    {
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

        public void AddTagsToSource(long userID, long sourceID, string[] tags)
        {
            List<long> tagIDs = new List<long>();
            if (tags == null) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDataContext().Database.Connection.ConnectionString))
                {
                    conn.Open();

                    foreach (string tag in tags)
                    {
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
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@TagName", tag.Trim());

                        cmd.ExecuteNonQuery();
                        tagIDs.Add(outputIdParam.Value as int? ?? default(int));


                    }
                    conn.Close();
                    DeleteSourceTagMapping(sourceID);
                    UpdateSourceTagMapping(sourceID, tagIDs);

                }
            }
            catch
            {
                throw;
            }

        }
                
        /* Gets all the tags associated with a source */
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


        //public IList<String> GetTagNames(string charactersToSearch, long userID = -1)
        //{
        //    IList<String> lstTags = null;
        //    try
        //    {
        //        using (GetDataContext())
        //        {
        //            if (charactersToSearch == "")
        //            {
        //                if(userID > -1){
        //                    lstTags = (from tags in context.Tags
        //                               where tags.UserID == userID && tags.Name.Contains(charactersToSearch)
        //                               orderby tags.Name

        //                               select tags.Name)
        //                               .ToList();
        //                }
        //                else
        //                {
        //                    lstTags = (from tags in context.Tags
        //                               where tags.Name.Contains(charactersToSearch)
        //                               orderby tags.Name
        //                               select tags.Name).Distinct().ToList();
        //                }
        //            }
        //            else
        //            {
        //                if (userID > 1) { 
        //                    lstTags = (from tags in context.Tags
        //                           where tags.Name.Contains(charactersToSearch) && tags.UserID == userID
        //                           select tags.Name).ToList();
        //                }
        //                else
        //                {
        //                    lstTags = (from tags in context.Tags
        //                               where tags.Name.Contains(charactersToSearch)
        //                               select tags.Name).Distinct().ToList();
        //                }
        //            }

        //        }
        //    }catch{
        //        throw;
        //    }
        //    finally
        //    {
        //        DisposeContext();
        //    }
        //    return lstTags;
        //}
        /// <summary>
        /// Method used to Save Tags
        /// </summary>
        /// <param name="objTag"></param>
        /// <returns></returns>

        //public bool DeleteSourceTagForSource(long sourceID)
        //{
        //    IList<SourceTag> mappedTagList = null;
        //    try
        //    {
        //        using (GetDataContext())
        //        {
        //                mappedTagList = (from sourceTag in context.SourceTags
        //                                    where sourceTag.SourceID == sourceID
        //                                     select sourceTag).ToList();
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
        //    if(mappedTagList.Count > 0){
        //        foreach(SourceTag tag in mappedTagList){
        //            context.Entry(tag).State = EntityState.Deleted;
        //        }
        //        context.SaveChanges();
        //        return true;
        //    }

        //    return true;
        //}
    }
}   