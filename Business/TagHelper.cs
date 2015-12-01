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


        public List<SourceTagData> GetSourceUserTags(long sourceUserID)
        {
            TagRepository tagRepository = new TagRepository();
            IList<Tag> tags = tagRepository.GetTagsForSourceUser(sourceUserID);
            List<SourceTagData> retList = new List<SourceTagData>();
            foreach(var sourcetag in tags){
                SourceTagData n = new SourceTagData{text = sourcetag.Name, id = sourcetag.ID};
                retList.Add(n);
            }
            return retList;
        }

        public List<SourceTagData> GetSourceTags(long sourceID)
        {
            TagRepository tagRepository = new TagRepository();
            IList<Tag> tags = tagRepository.GetTagsForSource(sourceID);
            List<SourceTagData> retList = new List<SourceTagData>();
            foreach (var sourcetag in tags)
            {
                SourceTagData n = new SourceTagData { text = sourcetag.Name, id = sourcetag.ID };
                retList.Add(n);
            }
            return retList;
        }
        public bool UpdateSourceTags(SourceUser sourceUser, List<SourceTagData> tags)
        {
            TagRepository tagRepository = new TagRepository();
            List<string> tagNames = tagRepository.UpdateSourceUserTags(sourceUser, (from tag in tags select tag.text).ToArray());
            if (tagNames != null && tagNames.Count > 0)
            {
                new NotificationHelper().UpdateNotifications(sourceUser, NotificationHelper.NOTIFICATION_REASON_TAG, tagNames);
            }
            return true;
        }

        public static IList<Tag> GetRecentUserTags(long userID)
        {
            return new TagRepository().GetRecentTags(userID);
        }

        public void UpdateAnnotationTags(Annotation annotation, string[] tagNames, SourceUser sourceUser)
        {
            TagRepository tagRepository = new TagRepository();
            List<string> addedtags = tagRepository.UpdateAnnotationTags(annotation, tagNames, sourceUser.ID);
            if (!new AnnotationHelper().IsAnnotationPrivate(annotation) &&
                addedtags != null && addedtags.Count > 0  )
            {
                new NotificationHelper().UpdateNotifications(sourceUser, NotificationHelper.NOTIFICATION_REASON_TAG, addedtags);
            }
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

        public List<long> GetUsersForTags(List<string> tags)
        {
            return new TagRepository().GetUsersForTags(tags);
        }

    }
}