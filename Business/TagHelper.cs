using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Search;
using Model.Extended.Extension;

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


        public List<SourceTagData> GetSourceTags(long sourceID)
        {
            TagRepository tagRepository = new TagRepository();
            IList<Tag> tags = tagRepository.GetTagsForSourceUser(sourceID);
            List<SourceTagData> retList = new List<SourceTagData>();
            foreach(var sourcetag in tags){
                SourceTagData n = new SourceTagData{text = sourcetag.Name, id = sourcetag.ID};
                retList.Add(n);
            }
            return retList;
        }

        public bool UpdateSourceTags(SourceUser sourceUser, List<SourceTagData> tags)
        {
            TagRepository tagRepository = new TagRepository();
            string[] tagNames = new string[] { };
            
            tagRepository.UpdateSourceUserTags(sourceUser, (from tag in tags select tag.text).ToArray());
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

        public IList<SourceTagData> GetTagNames(string tagQuery)
        {
            IList<SourceTagData> tags = new List<SourceTagData>();
            IList<string> tagNames =  new TagRepository().GetTagNames(tagQuery);
            if (tagNames != null)
            {
                foreach (var tag in tagNames)
                {
                    tags.Add(new SourceTagData() { text = tag });
                }
            }
            return tags;
        }
    }
}