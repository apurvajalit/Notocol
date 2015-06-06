using AutoMapper;
using Model.Extended.Extension;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{


    public class ObjectTypeConverter
    {
        public class ExtensionUserToUser : ValueResolver<ExtensionUser, Model.User>
        {
            protected override Model.User ResolveCore(ExtensionUser user)
            {
                Model.User userDB = new Model.User();
                userDB.ID = user.ID;
                userDB.Username = user.Username;
                userDB.Email = user.email;
                userDB.Password = user.Password;


                return userDB;
            }

        }
        public class UserToExtensionUser : ValueResolver<Model.User, ExtensionUser>
        {
            protected override ExtensionUser ResolveCore(Model.User user)
            {
                ExtensionUser extUser = new ExtensionUser();
                extUser.ID = user.ID;
                extUser.Username = user.Username;
                extUser.Password = user.Password;
                extUser.email = user.Email;


                return extUser;
            }

        }

        public class ExtensionAnnotationDataToAnnotation : ValueResolver<ExtensionAnnotationData, Annotation>
        {
            protected override Annotation ResolveCore(ExtensionAnnotationData extAnnotation)
            {
                return new Annotation();
            }
        }

        public class AnnotationToExtensionAnnotationData : ValueResolver<Annotation, ExtensionAnnotationData>
        {
            protected override ExtensionAnnotationData ResolveCore(Annotation annotation)
            {
                return new ExtensionAnnotationData();
            }
        }
        
    }
}