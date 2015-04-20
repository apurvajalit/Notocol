using System.Collections.Generic;
using System.Web.Http;
using Model;
using Repository;
using System;

namespace Notocol.Controllers.Api
{
    [CustomAuthFilter]
    public class TagController : BaseApiController
    {
        
        [HttpGet]
        public IList<Tag> Tags(string strSearch="")
        {
            TagRepository objTagRepository = new TagRepository();
            IList<Tag> searchTags = objTagRepository.SearchTags(strSearch,  Convert.ToInt64(Request.Properties["userID"]));
            return searchTags;
        }

        [HttpPost]
        public long Tag(Tag objTag)
        {
            objTag.UserID = Convert.ToInt64(Request.Properties["userID"]);
            TagRepository objTagRepository = new TagRepository();
            return objTagRepository.SaveTag(objTag).ID;
        }

        [HttpDelete]
        public bool DeleteTag(Tag objTag)
        {
            objTag.UserID = Convert.ToInt64(Request.Properties["userID"]);
            TagRepository objTagRepository = new TagRepository();
            return objTagRepository.DeleteTag(objTag);
        }

    }
}
