using AttributeRouting.Web.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data.Entity;
using Model.Extended.Extension;
using Repository;
using Model;
using Notocol.Models;
using AutoMapper;

namespace h_store.Controllers
{

    public class annotationsController : ApiController
    {

        //static IList<ClientAnnotationData> annotationsList = new List<ClientAnnotationData>();
        [HttpGet]
        public JObject storeInfo()
        {
            //TODO Change the hardcoded server details
            string info = "{\"message\": \"Annotator Store API\",\"links\": {\"search\": {\"url\": \"https://localhost:44301//api/annotations/search\", \"method\": \"GET\", \"desc\": \"Basic search API\"}, \"annotation\": {\"read\": {\"url\": \"https://localhost:44301//api/annotations/:id\", \"method\": \"GET\", \"desc\": \"Get an existing annotation\"}, \"create\": {\"url\": \"https://localhost:44301//api/annotations\", \"method\": \"POST\", \"desc\": \"Create a new annotation\"}, \"update\": {\"url\": \"https://localhost:44301//api/annotations/:id\", \"method\": \"PUT\", \"desc\": \"Update an existing annotation\"}, \"delete\": {\"url\": \"https://localhost:44301//api/annotations/:id\", \"method\": \"DELETE\", \"desc\": \"Delete an annotation\"}}}}";
            JObject json = JObject.Parse(info);
            return json;
        }

        [HttpGet]
        public ExtensionSearchResponse Search(int limit, int offset, string order, string sort, string uri)
        {
            ExtensionSearchResponse res = new ExtensionSearchResponse();
            AnnotationRepository objAnnotationRepository = new AnnotationRepository();

            List<Annotation> annotationList = objAnnotationRepository.getAnnotations(uri, Utility.GetCurrentUserID());
           
            res.total = annotationList.Count;
            foreach (var annotation in annotationList)
                res.rows.Add(Utility.AnnotationToExtensionAnnotation(annotation));

            return res;
        }

        [HttpPost]
        public ExtensionAnnotationData annotations(ExtensionAnnotationData extAnnotation)
        {
            AnnotationRepository objAnnotationRepository = new AnnotationRepository();
            extAnnotation.created = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            extAnnotation.updated = extAnnotation.created;
            extAnnotation.consumer = Utility.GetCurrentUserName();
            Annotation annotation = Utility.ExtensionAnnotationToAnnotation(extAnnotation);

            //TODO Take care of the following
            annotation.UserID = (int)Utility.GetCurrentUserID();

            if ((annotation.ID = objAnnotationRepository.AddAnnotation(annotation)) <= 0)
            {
                return null; //TODO add a more informative error
            }

            extAnnotation.id = annotation.ID;
            return extAnnotation;

        }


        [HttpGet]
        public ExtensionAnnotationData annotations(long id)
        {
            AnnotationRepository objAnnotationRepository = new AnnotationRepository();
            Annotation annotation = null;
            if ((annotation = objAnnotationRepository.getAnnotation(id)) != null)
                return Utility.AnnotationToExtensionAnnotation(annotation);
             
            return null;
           
        }


        [System.Web.Http.HttpPut]
        //[Route("api/Annotation/annotations/{id}")]
        public ExtensionAnnotationData annotations(long id, ExtensionAnnotationData extAnnotation)
        {
         
            AnnotationRepository objAnnotationRepository = new AnnotationRepository();
            Annotation annotation = Utility.ExtensionAnnotationToAnnotation(extAnnotation);
            annotation.UserID = (int)Utility.GetCurrentUserID();
            annotation.ID = (int)id;
            if(objAnnotationRepository.UpdateAnnotation(annotation))
                return extAnnotation;

            return null;
        }


        //[System.Web.Http.HttpDelete]
        ////[Route("api/Annotation/annotations/{id}")]
        //public System.Web.Mvc.ActionResult annotationsDelete(long id, [FromBody] Annotation req)
        //{
        //    return new System.Web.Mvc.HttpStatusCodeResult(HttpStatusCode.NoContent);
        //}

      }
}
