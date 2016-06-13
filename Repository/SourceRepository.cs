using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Model;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using Model.Extended;
using System.Security.Cryptography;
using System.Data.Entity.Validation;
using Repository.Search;

namespace Repository
{
    public class SourceRepository : BaseRepository
    {
        static string lastAddedURI;

        public SourceRepository()
        {
            CreateDataContext();
        }
        
        public bool UpdateSource(Source source)
        {

            if (source.ID <= 0)
            {
                return false;
            }
            using (GetDataContext()) 
            {
                try
                {
                    context.Entry(source).State = EntityState.Modified;
                    context.SaveChanges();
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
            
            return true;
        }

        public bool DeleteSource(Source source)
        {
            AnnotationRepository annRepo = new AnnotationRepository();
            TagRepository tagRepo = new TagRepository();
            using (GetDataContext()) 
            {
                
                try{
                   // context.DeleteSource(source.ID);
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
            
            return true;
        }

        public Source GetSource(long sourceID)
        {
            Source source = null;

            using (GetDataContext()) 
            {
                
                try{
                    source = (from sources in context.Sources
                                 where sources.ID == sourceID
                                 select sources).FirstOrDefault();
                }
                catch
                {
                    throw;
                }
                finally
                {

                }
            }
            

            return source;
        }

        /*Get sourceUserID for given user and sourceURI */
        public long GetSourceUserID(string URI, string link, long userID)
        {
            SourceUser su = GetSourceUser(URI, link, userID);
            if (su != null) return su.ID;
            return 0;

        }

        public SourceUser GetSourceUser(string URI, string link, long userID)
        {
            
            Source source = GetSource(URI, link);
            if (source == null) return null;
            return GetSourceUser(source.ID, userID);

        }
        
        public Source GetSource(string URI, string link)
        {
            Source source = null;
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] URIHash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(URI));

            using (GetDataContext())
            {
                try
                {
                    source = (from sources in context.Sources
                              where sources.uriHash == URIHash && sources.url == link
                              select sources).FirstOrDefault();

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
            return source;
        }

        public long GetSourceID(string URI, string link)
        {
            Source source = GetSource(URI, link);
            if(source != null) return source.ID;
            return 0;
            
        }

        public long GetSourceUserID(string URI, MappedSourceUser mappedSourceUser)
        {
            long ID = 0;
            if (mappedSourceUser.SUID != 0) return mappedSourceUser.SUID;
            ID = GetSourceUserID(URI, mappedSourceUser.link, mappedSourceUser.userID);
            
            //Already Exists
            if (ID != 0) return ID;


            return ID;

        }
        
        /* Search used for fetch source results for dashboardpage view */
        public IList<SourceData> Search(string keyword, IList<long> tagIDs, long userID = -1)
        {

            IList<SourceData> sourceList = new List<SourceData>();
            using (var con = new SqlConnection(GetDataContext().Database.Connection.ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SearchForSource", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@keywordStr", keyword);
                    if (userID > -1) cmd.Parameters.AddWithValue("@userID", userID);


                    var table = new DataTable();
                    table.Columns.Add("TagID", typeof(long));

                    foreach (long id in tagIDs)
                        table.Rows.Add(id);

                    var pList = new SqlParameter("@TagIDList", SqlDbType.Structured);
                    pList.TypeName = "dbo.TagIDList";
                    pList.Value = table;

                    cmd.Parameters.Add(pList);
                    TagRepository tagRepository = new TagRepository();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SourceData source = new SourceData();
                            source.ID = Convert.ToInt64(dr["ID"].ToString());
                            source.Title = dr["Title"].ToString();
                            source.Url = dr["SourceURI"].ToString();
                            source.Summary = dr["Summary"].ToString();
                            source.UserName = (dr["Username"].ToString());

                            source.Tags = tagRepository.GetTagsForSourceUser(source.ID);
                            sourceList.Add(source);

                        }

                    }
                }
            }
            return sourceList;
        }

        public SourceUser GetSourceUser(long sourceID, long userID)
        {
            SourceUser sourceUser = null;

            using (GetDataContext())
            {
                try
                {
                    sourceUser = (from sourceUsers in context.SourceUsers
                                  where sourceUsers.SourceID == sourceID && sourceUsers.UserID == userID
                                  select sourceUsers).FirstOrDefault();
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

            return sourceUser;
        }

        public SourceUser GetSourceUser(long sourceUserID)
        {
            SourceUser sourceUser = null;

            using (GetDataContext())
            {
                try
                {
                    sourceUser = (from sourceUsers in context.SourceUsers
                                  where sourceUsers.ID == sourceUserID
                                  select sourceUsers).FirstOrDefault();
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

            return sourceUser;
        }

        public SourceUser UpdateSourceUser(SourceUser sourceUser, string username = null)
        {
            if (sourceUser.FolderID == 0) sourceUser.FolderID = null;
            using (GetDataContext())
            {
                try
                {
                    sourceUser.ModifiedAt = DateTime.Now;
                    context.Entry(sourceUser).State = EntityState.Modified;
                    context.SaveChanges();
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
            ElasticSearchTest es = new ElasticSearchTest();
            es.UpdateSourceUserSummary(sourceUser);
            if (sourceUser.thumbnailImageUrl != null || sourceUser.thumbnailText != null)
                es.UpdateSourceTNData((long)sourceUser.SourceID, sourceUser.thumbnailText, sourceUser.thumbnailImageUrl);

            if (username != null)
            {
                if (sourceUser.Privacy == null || !((bool)sourceUser.Privacy))
                {
                    es.AddPublicUser((long)sourceUser.SourceID, username);
                }
                else
                {
                    es.DeletePublicUser((long)sourceUser.SourceID, username);
                }
            }
            return sourceUser;
        }

        public SourceUser AddSourceUser(SourceUser sourceUser, string username)
        {
            if (sourceUser.FolderID == 0) sourceUser.FolderID = null;
            using (GetDataContext())
            {
                try
                {
                    sourceUser.ModifiedAt = DateTime.Now;
                    context.Entry(sourceUser).State = EntityState.Added;
                    context.SaveChanges();
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
            
            if (sourceUser.ID > 0){
                ElasticSearchTest es = new ElasticSearchTest();
                es.AddUserToSource(sourceUser);
                if (sourceUser.thumbnailImageUrl != null || sourceUser.thumbnailText != null){
                    es.UpdateSourceTNData((long)sourceUser.SourceID, sourceUser.thumbnailText, sourceUser.thumbnailImageUrl);
                }
                if (sourceUser.Privacy == null || !((bool)sourceUser.Privacy))
                {
                    es.AddPublicUser((long)sourceUser.SourceID, username);
                }
            }
            return sourceUser;
        }

        public Source AddSource(string URI, Source source)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            source.uriHash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(URI));
            source.created = DateTime.Now;

            
            using (GetDataContext())
            {
                try
                {
                    context.Entry(source).State = EntityState.Added;
                    context.SaveChanges();
                }
                catch(Exception e) 
                {
                    
                    throw;
                }
                finally
                {
                    DisposeContext();
                }
            }

            if (source.ID > 0)
            {
                lastAddedURI = URI;
                ElasticSearchTest es = new ElasticSearchTest();
                es.AddSourceSearchIndex(source, null, null);
            }
            return source;
        }

        public bool DeleteSourceUser(SourceUser sourceuser, string username)
        {
            bool deleteSource = false;

            using (var con = new SqlConnection(GetDataContext().Database.Connection.ConnectionString))
            {
                con.Open();
                try
                {
                    using (SqlCommand cmd = new SqlCommand("DeleteSourceUser", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SourceUserID", sourceuser.ID);
                        cmd.Parameters.AddWithValue("@SourceID", sourceuser.SourceID);

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@DeleteSource";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.Bit;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);
                        cmd.ExecuteNonQuery();
                        deleteSource = Convert.ToBoolean(outPutParameter.Value.ToString());
                    }
                }
                catch
                {
                    throw;
                }
                ElasticSearchTest es = new ElasticSearchTest();
                es.DeleteUserForSource(sourceuser);

                if (deleteSource) es.DeleteSource((long)sourceuser.SourceID);
                else
                {
                    if (sourceuser.Privacy != null && ((bool)sourceuser.Privacy))
                    {
                        es.DeletePublicUser((long)sourceuser.SourceID, username);
                    }
                }
            }
            return true;
        }

        internal long GetSourceIDFromSourceUser(long sourceUserID)
        {
            long sourceID = 0;
            using (GetDataContext())
            {
                try
                {
                    var sourceCheck = (from sourceuser in context.SourceUsers
                                       where sourceuser.ID == sourceUserID
                                       select sourceuser.SourceID).FirstOrDefault();

                    if (sourceCheck != null) sourceID = (long)sourceCheck;

                }
                catch
                {

                }
                finally
                {
                    DisposeContext();
                }
            }
            return sourceID;
        }



        public List<NoteData> GetSourceSummarysWithUserAtTop(long sourceID, long userID)
        {
            List<SourceUser> list = null;
            List<NoteData> ret = new List<NoteData>();
            try
            {
                using (GetDataContext())
                {
                    list = (from su in context.SourceUsers
                            .Include("User")
                            orderby su.UserID == userID ? 1 : 0 descending
                            where su.SourceID == sourceID
                            select su).ToList();
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
            foreach (var su in list)
            {
                if (su.Summary != null && su.Summary.Length > 0)
                {
                    NoteData n = new NoteData
                    {
                        NoteText = su.Summary,
                        username = su.User.Name,
                        updated = su.ModifiedAt != null ? (DateTime)su.ModifiedAt : DateTime.Now,

                    };
                    ret.Add(n);
                }
            }
            return ret;
            
        }



        public Dictionary<UserKey, NoteData> GetSourceSummaries(long sourceID, long currentUserID)
        {
            List<SourceUser> list = null;
            Dictionary<UserKey, NoteData> userSummaries = new Dictionary<UserKey, NoteData>();
            try
            {
                using (GetDataContext())
                {
                    list = (from su in context.SourceUsers
                            .Include("User")
                            where su.SourceID == sourceID && (!(bool)su.Privacy || su.UserID == currentUserID)
                            select su).ToList();
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
            foreach (var su in list)
            {
                if (su.Summary != null && su.Summary.Length > 0)
                {   
                    userSummaries.Add(new UserKey(su.UserID, su.User.Username), new NoteData
                      {
                        id = 0,
                        NoteText = su.Summary,
                        updated = (DateTime)su.ModifiedAt
                });
                }
            }
            return userSummaries;
        }


        public List<long> GetSourceUsers(long sourceID)
        {
            List<long> userList = null;
            try
            {
                using (GetDataContext())
                {
                    userList = (from su in context.SourceUsers
                                where sourceID == su.SourceID
                                select su.UserID).ToList();
                }
            }
            catch
            {
                throw;
            }
            return userList;
        }

        public SourceUser GetSourceUserWithSource(long sourceUserID)
        {
            SourceUser su = null;
            try
            {
                using (GetDataContext())
                {
                    su = (from src in context.SourceUsers
                          .Include("Source")
                          .Include("User")
                          .Include("Folder")
                          where sourceUserID == src.ID
                          select src).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
            return su;
        }

        internal List<ESSource> GetSortedOwnSource(long userID, int offset, int size)
        {
            List<ESSource> ret = new List<ESSource>();
            List<SourceUser> inputList = new List<SourceUser>();

            try
            {
                using (GetDataContext())
                {
                    inputList = (from su in context.SourceUsers
                                 .Include("Source")
                                 .Include("Folder")
                                 where su.UserID == userID
                                 orderby su.ModifiedAt descending
                                 select su).Skip(offset).Take(size).ToList();
                }
            }
            catch
            {

            }
            string[] emptyStringArray = new List<string>().ToArray();
            TagRepository tagRepository = new TagRepository();
            
             foreach (var su in inputList)
            {
                ESSource res = new ESSource
                {
                    faviconURL = su.Source.faviconURL,
                    Id = (long)su.SourceID,
                    lastUsed = (DateTime)su.ModifiedAt,
                    link = su.Source.url,
                    popularity = 0,
                    publicUserNames = emptyStringArray,
                    sourceUserID = su.ID,
                    title = su.Source.title,
                    tnImage = su.thumbnailImageUrl,
                    tnText = (su.Summary != null)? su.Summary:su.thumbnailText
                    
                };

                res.tags = (from tags in tagRepository.GetTagsForSourceUser((long)su.SourceID).ToArray() select tags.Name).ToArray();
                ret.Add(res);
            }
            return ret;
        }

        public List<GetUserProfileSourcePages_Result1> GetProfileSourceData(long userID, int offset, int size)
        {
            List<GetUserProfileSourcePages_Result1> data = null; 
            
            try
            {
                using (GetDataContext())
                {
                    data = context.GetUserProfileSourcePages(userID, offset, size).ToList();
                    
                }
            }
            catch
            {
                throw;
            }
            
            
            return data;
        }
    }
}
