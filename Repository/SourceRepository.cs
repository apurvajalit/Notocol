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

namespace Repository
{
    public class SourceRepository : BaseRepository
    {
        public SourceRepository()
        {
            CreateDataContext();
        }
        
        public Source AddSource(Source source)
        {
            //long sourceID = GetSourceIDFromSourceURI(source.SourceURI, source.UserID);
            //if (sourceID <= 0){
                try
                {
                    using (GetDataContext())
                    {
                        source.ModifiedAt = DateTime.Now;
                        if (source.Privacy == null) source.Privacy = false;
                        context.Entry(source).State = EntityState.Added;
                        context.SaveChanges();
                        
                    }
                }catch
                {
                    throw;
                }
                finally
                {
                    DisposeContext();
                }
            //}
            return source;
        }

        public bool UpdateSource(Source source)
        {

            if (source.ID <= 0)
            {
                return false;
            }
            try
            {
                using (GetDataContext())
                {
                    source.ModifiedAt = DateTime.Now;
                    
                    context.Entry(source).State = EntityState.Modified;
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

        public Source GetSource(long sourceID)
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

        /*Get sourceID for given user and sourceURI */
        public long GetSourceIDFromSourceURI(string sourceURI, long userID)
        {
            long sourceID = 0;
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
                DisposeContext();
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

        /* Get all the sources for a user */
        public IList<Source> GetSourcesForUser(long userID)
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

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SourceData source = new SourceData();
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

    }
}
