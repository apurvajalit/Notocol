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
                    context.DeleteSource(source.ID);
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

        public SourceUser UpdateSourceUser(SourceUser sourceUser)
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
            return sourceUser;
        }

        public SourceUser AddSourceUser(SourceUser sourceUser)
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
            if (sourceUser.ID > 0 && sourceUser.Summary != null)
            {
                ElasticSearchTest es = new ElasticSearchTest();
                es.UpdateSourceUserSummary(sourceUser);

                if (sourceUser.thumbnailImageUrl != null || sourceUser.thumbnailText != null)
                    es.UpdateSourceTNData((long)sourceUser.SourceID, sourceUser.thumbnailText, sourceUser.thumbnailImageUrl);
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
                ElasticSearchTest es = new ElasticSearchTest();
                es.AddSourceSearchIndex(source, null, null, null);
            }
            return source;
        }

        public bool DeleteSourceUser(SourceUser sourceuser)
        {
            using (GetDataContext())
            {
                try
                {
                    context.Entry(sourceuser).State = EntityState.Deleted;
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
    }
}
