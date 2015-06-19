using System.Collections.Generic;
using System.Web.Http;
using Model;
using Repository;
using System;
using Notocol.Models;
using Model.Extended.Extension;

namespace Notocol.Controllers.Api
{
    public class TagController : BaseApiController
    {
        
        [HttpGet]
        public IList<String> Tags(string strSearch="")
        {
            TagRepository objTagRepository = new TagRepository();
            IList<String> searchTags = objTagRepository.SearchTags(strSearch,  Convert.ToInt64(Request.Properties["userID"]));
            return searchTags;
        }

        //[HttpPost]
        //public long Tag(Tag objTag)
        //{
        //    objTag.UserID = Convert.ToInt64(Request.Properties["userID"]);
        //    TagRepository objTagRepository = new TagRepository();
        //    return objTagRepository.SaveTag(objTag).ID;
        //}

        [HttpDelete]
        public bool DeleteTag(Tag objTag)
        {
            objTag.UserID = Convert.ToInt64(Request.Properties["userID"]);
            TagRepository objTagRepository = new TagRepository();
            return objTagRepository.DeleteTag(objTag);
        }

        [HttpGet]
        public TagDataForBookmark GetPageTags(string uri)
        {
            TagRepository tagRepository = new TagRepository();
            SourceRepository sourceRepository = new SourceRepository();
            string tagStringValues = "";
            long userID = 0;
            Source source = null;
            TagDataForBookmark returnValues = new TagDataForBookmark();
            if((userID = Utility.GetCurrentUserID()) > 0 
                && ((source = sourceRepository.GetSourceFromSourceURI(uri, userID)) != null)) 
            {
                IList<Tag> tagList = tagRepository.GetTagsForSource(source.ID);
                if(tagList != null){
                    foreach (Tag tag in tagList)
                    {
                        
                        tagStringValues += tag.Name+",";

                    }
                    if (tagStringValues.Length > 1)
                        tagStringValues = tagStringValues.Substring(0,tagStringValues.Length-1);
                }
            
            }
            returnValues.tag = tagStringValues;
            returnValues.isPrivate = source.Privacy== true?true:false;
            return returnValues;
        }

        [HttpPost]
        public bool UpdatePageTags(TagDataForBookmark tagData)
        {
            SourceRepository sourceRepository = new SourceRepository();
            Source source = sourceRepository.GetSourceFromSourceURI(tagData.source, Utility.GetCurrentUserID());
            if(source != null){
                if (source.Privacy != tagData.isPrivate)
                    sourceRepository.UpdateSourcePrivacy(source, tagData.isPrivate);

                TagRepository tagRepository = new TagRepository();
                tagRepository.UpdateUserTagsForSource(Utility.GetCurrentUserID(),source.ID, tagData.tag.Split(','));
                
            }
            return true;

        }
    }
}
