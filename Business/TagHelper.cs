using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Business
{
    public class TagHelper
    {
        public static IList<String> GetAllUserTag(string searchStr = "")
        {
            return new TagRepository().SearchTagNames(searchStr);
            
        }
        public static IList<String> GetCurrentUserTag(long userID, string searchStr = "")
        {
           return new TagRepository().SearchTagNames(searchStr, userID);

        }

        public static IList<long> GetCurrentUserIDs(long userID, string[] tagStrings)
        {
            return new TagRepository().GetTagIDs(tagStrings, userID);
        }
        public static IList<long> GetAllUserTagIDs(string[] tagStrings)
        {
            return new TagRepository().GetTagIDs(tagStrings);
        }


        public string[] GetSourceTags(long sourceID)
        {
            TagRepository tagRepository = new TagRepository();
            IList<Tag> tags = tagRepository.GetTagsForSource(sourceID);
            return (from tag in tags select tag.Name).ToArray();
        }

        public bool UpdateSourceTags(Source source, string[] tagNames)
        {
            TagRepository tagRepository = new TagRepository();
            tagRepository.AddTagsToSource(source.UserID, source.ID, tagNames);
            return true;
        }
    }
}