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
        public class AnnotationController : BaseApiController
    {
          
        AnnotationRepository objAnnotationRepository = new AnnotationRepository();

        [HttpGet]
        public IList<AnnotationDataResponse> annotations()
        {
            return objAnnotationRepository.getAnnotations();
           // return annDataObjList;
        }


        [HttpPost]
        public AnnotationDataResponse annotations([FromBody]NewAnnotationDataFromRequest anndata)
        {
            
            AnnotationDataResponse response = objAnnotationRepository.createAnnotation(anndata);
            return response;
        }

        [HttpGet]
        public AnnotationDataResponse annotations(long id)
        {
            
            return objAnnotationRepository.getAnnotation(id);
        }
            
        [System.Web.Http.HttpPut]
        [Route("api/Annotation/annotations/{id}")]
        public AnnotationDataResponse annotations(long id, [FromBody] AnnotationDataResponse annotationData)
        {
            
            return objAnnotationRepository.updateAnnotation(annotationData) ;
        }

        [System.Web.Http.HttpDelete]
        [Route("api/Annotation/annotations/{id}")]
        public System.Web.Mvc.ActionResult annotationsDelete(long id, [FromBody] AnnotationDataResponse req)
        {
            return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("api/Annotation/search")]
        public AnnotationSearchResults Search(int count, string uri, long user)
        {
            AnnotationSearchResults res = new AnnotationSearchResults();
            IList<AnnotationDataResponse> listRows = objAnnotationRepository.getAnnotations(uri, user);
            res.rows = listRows;
            res.total = listRows.Count();

            
            return res;
        }        

    }
}