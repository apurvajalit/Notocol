using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Extended
{
    public class UserRegistration
    {
        public string username { get; set; }
        public string  name { get; set; }
        public string password { get; set; }
        public string  address { get; set; }
        public char gender { get; set; }
        public DateTime DOB { get; set; }
        public string email { get; set; }
        public string photo { get; set; }
    }
}
