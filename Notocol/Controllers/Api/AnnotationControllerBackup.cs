//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using Model;
//using Notocol.Models;
//using Repository;

//namespace Notocol.Controllers.Api
//{
//    //    [CustomAuthFilter]
//    //    public class AnnotationControllerBackup : BaseApiController
//    //{
            
//    //      AnnotationRepository objAnnotationRepository = new AnnotationRepository();

//    //    [HttpGet]
//    //    public IList<AnnotationDataResponse> annotations()
//    //    {
//    //        return objAnnotationRepository.getAnnotations();
//    //       // return annDataObjList;
//    //    }

        
//    //    [HttpPost]
//    //    public AnnotationDataResponse annotations([FromBody]NewAnnotationDataFromRequest anndata)
//    //    {
//    //        long userID = Convert.ToInt64(Request.Properties["userID"]);
//    //        AnnotationDataResponse response = objAnnotationRepository.createAnnotation(anndata, userID);
//    //        return response;
//    //    }

        
//    //    [HttpGet]
//    //    public AnnotationDataResponse annotations(long id)
//    //    {
            
//    //        return objAnnotationRepository.getAnnotation(id);
//    //    }

        
//    //    [System.Web.Http.HttpPut]
//    //    [Route("api/Annotation/annotations/{id}")]
//    //    public AnnotationDataResponse annotations(long id, [FromBody] AnnotationDataResponse annotationData)
//    //    {
//    //        long userID = Convert.ToInt64(Request.Properties["userID"]);
//    //        return objAnnotationRepository.updateAnnotation(annotationData, userID) ;
//    //    }

        
//    //    [System.Web.Http.HttpDelete]
//    //    [Route("api/Annotation/annotations/{id}")]
//    //    public System.Web.Mvc.ActionResult annotationsDelete(long id, [FromBody] AnnotationDataResponse req)
//    //    {
//    //        return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.NoContent);
//    //    }

        
//    //    [HttpGet]
//    //    [Route("api/Annotation/search")]
//    //    public AnnotationSearchResults Search(string uri)
//    //    {
//    //        long userID = Convert.ToInt64(Request.Properties["userID"]);
//    //        AnnotationSearchResults res = new AnnotationSearchResults();
//    //        IList<AnnotationDataResponse> listRows = objAnnotationRepository.getAnnotations(uri, userID);
//    //        res.rows = listRows;
//    //        res.total = listRows.Count();
            
//    //        //string resEncoded = "Insicm93cyI6W3siY29uc3VtZXIiOiJhbm5vdGF0ZWl0IiwicXVvdGUiOiJlbXBsb3llZCIsInJhbmdlcyI6W3sic3RhcnQiOiIvZGl2WzFdL2RpdlsyXS9wWzFdIiwiZW5kIjoiL2RpdlsxXS9kaXZbMl0vcFsxXSIsInN0YXJ0T2Zmc2V0Ijo5NDUsImVuZE9mZnNldCI6OTU0fV0sInRleHQiOiJoZWxsbyIsInBlcm1pc3Npb25zIjp7InJlYWQiOlsiZ3JvdXA6X193b3JsZF9fIl0sImRlbGV0ZSI6W10sImFkbWluIjpbXSwidXBkYXRlIjpbXX0sInVwZGF0ZWQiOiIyMDE1LTA0LTA5VDEwOjQ3OjI1Ljg0Nzg3MyswMDowMCIsImNyZWF0ZWQiOiIyMDE1LTA0LTA5VDEwOjQ3OjI1LjgzNzc3NCswMDowMCIsImlkIjoiQVV5ZHlxV1QxWmgxLS1XSVlZMWoiLCJsaW5rcyI6W3sidHlwZSI6InRleHQvaHRtbCIsInJlbCI6ImFsdGVybmF0ZSIsImhyZWYiOiJodHRwOi8vYW5ub3RhdGVpdC5vcmcvYW5ub3RhdGlvbnMvQVV5ZHlxV1QxWmgxLS1XSVlZMWoifV0sInVzZXIiOiJsYWJ5cmludGgiLCJ0YWdzIjpbXSwidXJpIjoiaHR0cDovL2xvY2FsaG9zdDo1NTU1L0hvbWUvVGVzdFBvcHVwIn1dLCJ0b3RhbCI6MX0i";

//    //        //return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(resEncoded));
//    //        return res;
//    //    }        

//    //}
//}