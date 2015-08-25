//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Web;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
//using System.Web.Mvc;

//namespace Notocol.Controllers.Api
//{
//    public class CustomAuthFilter : AuthorizationFilterAttribute
//    {
//        private long getUserID(string tokenValue)
//        {
//            long userID = 0;
//            if(tokenValue != "")
//                 userID = Convert.ToInt64(tokenValue);

//            return userID;
//        }

//        public override void OnAuthorization(HttpActionContext actionContext)
//        {
//            long userID;
//            try { 
//              string token = actionContext.Request.Headers.GetValues("X-Notocol-Token").FirstOrDefault();
                
//                if ((userID = getUserID(token)) == 0) {
//                    actionContext.Response = new HttpResponseMessage();
//                    return;
//                }
//                actionContext.Request.Properties["userID"] = userID;
//             }catch(Exception e){
//                 throw;
//             }finally{

//            }
//        }
        
//    }
    
//}