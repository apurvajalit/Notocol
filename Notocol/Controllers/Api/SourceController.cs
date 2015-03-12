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
        public long Add([FromBody]SourceTagRequest source)//(SourceTagRequest sourceTags1)
        {
            SourceRepository obSourceRepository = new SourceRepository();
            obSourceRepository.SaveSource(source.Source, source.Tags);
            return source.Source.ID;
        }
    }
}
