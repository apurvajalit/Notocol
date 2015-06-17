using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Model;
using Notocol.Models;
using Repository;
using Model.Extended.Extension;

namespace Notocol.Controllers.Api
{
    public class SourceController : BaseApiController
    {
        //[HttpGet]
        //public IList<Source> Source()
        //{
        //    long userID = Convert.ToInt64(Request.Properties["userID"]);
        //    SourceRepository obSourceRepository = new SourceRepository();
        //    return  obSourceRepository.GetSource(userID);
        //}

        /// <summary>
        /// Save Source and its related Tag(s)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        //[HttpPost]
        //public long Add([FromBody]SourceData source)
        //{
        //    long userID = Convert.ToInt64(Request.Properties["userID"]);
        //    SourceRepository obSourceRepository = new SourceRepository();
        //    obSourceRepository.SaveSource(userID, source.Source, source.Tags);
        //    return source.Source.ID;
        //}


        [HttpGet]
        [Route("api/Source/Search/")]
        public IList<Source> Search(string keyword="", long[] tagIDs=null)
        {
            long userID = Convert.ToInt64(Request.Properties["userID"]);
            SourceRepository obSourceRepository = new SourceRepository();
            return obSourceRepository.Search(keyword, tagIDs, userID);
             
        }

        [HttpPost]
        public long UpdateSource([FromBody]SourceDataForBookmark sourceData)
        {
            Source source = new Source();
            if ((source.UserID = Utility.GetCurrentUserID()) <= 0)
                return 0;

            source.SourceURI = sourceData.sourceURI;
            source.Link = sourceData.sourceLink;
            source.Title = sourceData.title;

            return new SourceRepository().AddSourceIfNotFound(source);
        }

    }
}
