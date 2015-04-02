using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    public class AnnotationPermissions
    {
        
            public IList<String> read { get; set; }
            public IList<String> admin { get; set; }
            public IList<String> update { get; set; }
            public IList<String> delete { get; set; }
            public AnnotationPermissions()
            {
                string permission = "group:__world__";
                read = new List<string>();
                read.Add(permission);

                admin = new List<string>();
                admin.Add(permission);

                delete = new List<string>();
                delete.Add(permission);

                update = new List<string>();
                update.Add(permission);
            }
            public static string AnnotationPermissionsToString(AnnotationPermissions permissions)
            {

                return JsonConvert.SerializeObject(permissions);

            }

            public static AnnotationPermissions AnnotationPermissionsFromString(string annotationPermissionsString)
            {
                return JsonConvert.DeserializeObject<AnnotationPermissions>(annotationPermissionsString);
            }
    }
}