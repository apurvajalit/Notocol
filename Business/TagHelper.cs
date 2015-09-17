using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Search;

namespace Business
{
    public class TagHelper
    {
        public static IList<String> GetAllUserTag(string searchStr = "")
        {
            return new TagRepository().SearchTagNames(searchStr);
            
        }
        

        public static IList<long> TryGetTagIDs(string[] tagStrings)
        {
            return new TagRepository().TryGetTagIDs(tagStrings);
        }


        public string[] GetSourceTags(long sourceID)
        {
            TagRepository tagRepository = new TagRepository();
            IList<Tag> tags = tagRepository.GetTagsForSourceUser(sourceID);
            return (from tag in tags select tag.Name).ToArray();
        }

        public bool UpdateSourceTags(SourceUser sourceUser, string[] tagNames)
        {
            TagRepository tagRepository = new TagRepository();
            tagRepository.UpdateSourceUserTags(sourceUser, tagNames);
            return true;
        }

        public static IList<Tag> GetRecentUserTags(long userID)
        {
            return new TagRepository().GetRecentTags(userID);
        }

        public void UpdateAnnotationTags(Annotation annotation, string[] tagNames, long sourceID = 0)
        {
            TagRepository tagRepository = new TagRepository();
            tagRepository.UpdateAnnotationTags(annotation, tagNames, sourceID);
        }

        public string[] GetAnnotationTagNames(long annotationID)
        {
            return new TagRepository().GetAnnotationTagNames(annotationID);
            
        }
    }
}