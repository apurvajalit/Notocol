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
        public IList<Tag> Tags(string strSearch="")
        {
            TagRepository objTagRepository = new TagRepository();
            IList<Tag> searchTags = objTagRepository.SearchTags(strSearch,  Convert.ToInt64(Request.Properties["userID"]));
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
        public string GetPageTags(string uri)
        {
            TagRepository tagRepository = new TagRepository();
            SourceRepository sourceRepository = new SourceRepository();
            string tagStringValues = "";
            long sourceID = 0, userID = 0;

            if((userID = Utility.GetCurrentUserID()) > 0 
                && ((sourceID = sourceRepository.GetSourceIDFromSourceURI(uri, userID)) > 0)) 
            {
                IList<Tag> tagList = tagRepository.GetTagsForSource(sourceID);
                if(tagList != null){
                    foreach (Tag tag in tagList)
                    {
                        
                        tagStringValues += tag.Name+",";

                    }
                    if (tagStringValues.Length > 1)
                        tagStringValues = tagStringValues.Substring(0,tagStringValues.Length-1);
                }
            
            }
            return tagStringValues;
        }

        [HttpPost]
        public bool UpdatePageTags(TagDataForBookmark tagData)
        {
            long sourceID = new SourceRepository().GetSourceIDFromSourceURI(tagData.source, Utility.GetCurrentUserID());
            if(sourceID > 0){
                TagRepository tagRepository = new TagRepository();
                tagRepository.UpdateUserTagsForSource(Utility.GetCurrentUserID(),sourceID, tagData.tag.Split(','));
                
            }
            return true;

        }
    }
}
