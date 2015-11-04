using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class FileUploadData
    {
        public string originalFileName { get; set; }
        public string code { get; set; }
        public string password { get; set; }
        public long userID { get; set; }
        public string userName { get; set; }
        public string description { get; set; }
        public string uri { get; set; }
        public string title { get; set; }


    }
}
