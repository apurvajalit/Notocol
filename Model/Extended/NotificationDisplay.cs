using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class NotificationDisplay
    {
        public int reasonCode { get; set; }
        public Boolean readStatus { get; set; }
        public string secondaryUserName { get; set; }
        public long secondaryUserID { get; set; }
        public string sourceTitle { get; set; }
        public string notificationDetailText { get; set; }
        public long sourceID { get; set; }
        public long sourceUserID { get; set; }
        public string tags { get; set; }
        public string note { get; set; }
        public long id { get; set; }
        public DateTime notificationDate { get; set; }
    }
}
