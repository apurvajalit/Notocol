using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    public class TagHelper
    {
        public static IList<String> GetAllUserTag(string searchStr = "")
        {
            return new TagRepository().SearchTags(searchStr);
            
        }
        public static IList<String> GetCurrentUserTag(long userID, string searchStr = "")
        {
           return new TagRepository().SearchTags(searchStr, userID);

        }

        public static IList<long> GetCurrentUserIDs(long userID, string[] tagStrings)
        {
            return new TagRepository().GetTagIDs(tagStrings, userID);
        }
        public static IList<long> GetAllUserTagIDs(string[] tagStrings)
        {
            return new TagRepository().GetTagIDs(tagStrings);
        }
        
    }
}