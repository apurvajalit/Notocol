using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;
using Notocol.Models;
using Repository;

namespace Notocol.Controllers.Api
{
    public class SourceController : BaseApiController
    {
        [HttpGet]
        public string Source()
        {
            return "Source API Get";
        }

        /// <summary>
        /// Save Source and its related Tag(s)
        /// </summary>
        /// <param name="userID"> User's Unique ID</param>
        /// <param name="sourceTags">This object holds definition of source and all the tags used.</param>
        /// <returns></returns>
        [HttpPost]
        public long Add(long userID, [FromBody] SourceTagRequest sourceTags)
        {
            //Source objSourceTemp = new Source();
            //IList<Tag> lstTagsTemp = new List<Tag>();
            //objSourceTemp.Title = "Testing from Fiddler";
            //objSourceTemp.UserID = 2;

            //Tag objTag = new Tag();
            //objTag.ID = 5;
            //objTag.Name = "Project Management";
            //objTag.ParentID = 1;
            //objTag.UserID = 2;
            //lstTagsTemp.Add(objTag);

            //Tag objTag1 = new Tag();
            //objTag1.ID = 7;
            //objTag1.Name = "Javascript";
            //objTag1.ParentID = 1;
            //objTag1.UserID = 2;
            //lstTagsTemp.Add(objTag1);

            //Tag objTag2 = new Tag();
            //objTag2.ID = 0;
            //objTag2.Name = "Widgets";
            //objTag2.ParentID = 7;
            //objTag2.UserID = 2;
            //lstTagsTemp.Add(objTag2);

            SourceRepository obSourceRepository = new SourceRepository();
            obSourceRepository.SaveSource(sourceTags.Source, sourceTags.Tags);
            return 0;
        }
    }
}
