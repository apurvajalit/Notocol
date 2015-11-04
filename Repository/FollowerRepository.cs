using Model;
using Repository.Search;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class FollowerRepository:BaseRepository
    {
        public FollowerRepository()
        {
            CreateDataContext();
        }

        public bool AddFollower(long follower, long followee)
        {
            Follow follow = new Follow
            {
				followee = followee,
				follower = follower,
				lastUpdated = DateTime.Now
            };

            try
            {
                using (GetDataContext())
                {
                    context.Entry(follow).State = EntityState.Added;
                    context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
            //ElasticSearchTest es = new ElasticSearchTest();
            //es.AddFollower(follower, followee);
            return true;
        }

        public bool DeleteFollower(long follower, long followee)
        {
            Follow follow = null;

            try
            {
                using (GetDataContext())
                {
                    follow = (from followers in context.Follows
                              where followers.follower == follower && followers.followee == followee
                              select followers).FirstOrDefault();
                    if (follow != null)
                    {
                        context.Entry(follow).State = EntityState.Deleted;
                        context.SaveChanges();
                    }
                    
                }
            }
            catch
            {
                throw;
            }
            //ElasticSearchTest es = new ElasticSearchTest();
            //es.DeleteFollower(follower, followee);
            return true;
        }
        public List<long> GetAllFollowees(long follower)
        {
            try
            {
                using (GetDataContext())
                {
                    return (from followers in context.Follows
                            where followers.follower == follower
                            select followers.followee).ToList();
                }
            }
            catch
            {
                throw;
            }
            
        }

        public bool IsUserFollower(long followee, long follower)
        {
            bool res = false;
            try
            {
                using (GetDataContext())
                {
                    if((from followers in context.Follows
                            where followers.follower == follower
                            select followers.followee).FirstOrDefault() != null)
                        res = true;
                }
            }
            catch
            {
                throw;
            }
            return res;

        }

        public List<long> GetAllFollowers(long userID)
        {
            try
            {
                using (GetDataContext())
                {
                    return (from followers in context.Follows
                            where followers.followee == userID
                            select followers.follower).ToList();
                }
            }
            catch
            {
                throw;
            }

        }
    }
}
