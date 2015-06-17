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
        public static IList<Tag> GetAllUserTags(long userID)
        {

            return new TagRepository().SearchTags("", userID);
        }

        public static IList<string> GetAllUserTagNames(long userID, string searchStr = "")
        {
            IList<Tag> tagList = new TagRepository().SearchTags(searchStr, userID);
            IList<string> tagNames = new List<string>();

            foreach (Tag tag in tagList)
            {
                tagNames.Add(tag.Name);
            }
            return tagNames;


        }
    }
}