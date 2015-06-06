using Model;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notocol.Models
{
    public class TagHelper
    {
        public static IList<Tag> GetAllUserTags(long userID)
        {

            return new TagRepository().SearchTags("", userID);
        }
    }
}