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
        public IList<Source> Source(long userID = 2)
        {
            SourceRepository obSourceRepository = new SourceRepository();
            return  obSourceRepository.GetSource(userID);
        }

        /// <summary>
        /// Save Source and its related Tag(s)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpPost]
        public long Add([FromBody]SourceDataRequest source)
        {
            SourceRepository obSourceRepository = new SourceRepository();
            obSourceRepository.SaveSource(source.Source, source.Tags);
            return source.Source.ID;
        }
        
        [HttpGet]
        public SourceDataRequest GetSourceData(string pageURL, long userID)
        {
            SourceRepository obSourceRepository = new SourceRepository();
            return obSourceRepository.getSourceData(pageURL, userID);
        }


    }
}
