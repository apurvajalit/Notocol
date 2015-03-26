using System.Collections.Generic;
using System.Web.Http;
using Model;
using Repository;

namespace Notocol.Controllers.Api
{
    public class TagController : BaseApiController
    {
        [HttpGet]
        public IList<Tag> Tags(string strSearch="")
        {
            TagRepository objTagRepository = new TagRepository();
            IList<Tag> searchTags = objTagRepository.SearchTags(strSearch, 2);
            return searchTags;
        }

        [HttpPost]
        public long Tag(Tag objTag)
        {
            objTag.UserID = 2;
            TagRepository objTagRepository = new TagRepository();
            return objTagRepository.SaveTag(objTag).ID;
        }

        [HttpDelete]
        public bool DeleteTag(Tag objTag)
        {
            objTag.UserID = 2;
            TagRepository objTagRepository = new TagRepository();
            return objTagRepository.DeleteTag(objTag);
        }

    }
}
