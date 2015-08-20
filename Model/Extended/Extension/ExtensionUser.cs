using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended.Extension
{
    public class ExtensionUser
    {
        public long ID { get; set; }
        public string Username { get; set; }
        public string email { get; set; }
        public string Password { get; set; }
        public string subscriptions { get; set; }
        public string pwd { get; set; }
        public string code { get; set; }
        public string hashkey { get; set; }
     }
}
