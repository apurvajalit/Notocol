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
using Business;

namespace h_store.Controllers
{

    public class annotationsController : ApiController
    {

        //static IList<ClientAnnotationData> annotationsList = new List<ClientAnnotationData>();
        [HttpGet]
        public JObject storeInfo()
        {
            //TODO Change the hardcoded server details
            string baseServerName = System.Configuration.ConfigurationManager.AppSettings["serverName"];

            string info = "{\"message\": \"Annotator Store API\",\"links\": {\"search\": {\"url\": \"" + baseServerName + "api/annotations/search\", \"method\": \"GET\", \"desc\": \"Basic search API\"}, \"annotation\": {\"read\": {\"url\": \"" + baseServerName + "api/annotations/:id\", \"method\": \"GET\", \"desc\": \"Get an existing annotation\"}, \"create\": {\"url\": \"" + baseServerName + "api/annotations\", \"method\": \"POST\", \"desc\": \"Create a new annotation\"}, \"update\": {\"url\": \"" + baseServerName + "api/annotations/:id\", \"method\": \"PUT\", \"desc\": \"Update an existing annotation\"}, \"delete\": {\"url\": \"" + baseServerName + "api/annotations/:id\", \"method\": \"DELETE\", \"desc\": \"Delete an annotation\"}}}}";
            JObject json = JObject.Parse(info);
            return json;
        }

        [HttpGet]
        public ExtensionSearchResponse Search(int limit, int offset, string order, string sort, string uri)
        {
            return new AnnotationHelper().GetAnnotationsForPage(Utility.GetCurrentUserID(), uri);
        }

        [HttpPost]
        public ExtensionAnnotationData annotations(ExtensionAnnotationData extAnnotation)
        {

            return new AnnotationHelper().AddAnnotation(extAnnotation, Utility.GetCurrentUserName(), Utility.GetCurrentUserID());
        }


        [HttpGet]
        public ExtensionAnnotationData annotations(long id)
        {
            return new AnnotationHelper().GetAnnotation(id);
           
        }


        [System.Web.Http.HttpPut]
        //[Route("api/Annotation/annotations/{id}")]
        public ExtensionAnnotationData annotations(long id, ExtensionAnnotationData extAnnotation)
        {

            return new AnnotationHelper().UpdateAnnotation(id, extAnnotation, Utility.GetCurrentUserID());
            
        }


        [System.Web.Http.HttpDelete]
        //[Route("api/Annotation/annotations/{id}")]
        public JObject annotationsDelete(long id)
        {
            AnnotationRepository annotationRespository = new AnnotationRepository();
            string successResult = "{\"deleted\": true, \"id\": \""+id+"\"}";
            string failedResult = "{\"deleted\": false, \"id\": \""+id+"\"}";

            if(new AnnotationHelper().DeleteAnnotation(id, Utility.GetCurrentUserID()))
                return JObject.Parse(successResult);
            else
                return JObject.Parse(failedResult);

        }

      }
}
