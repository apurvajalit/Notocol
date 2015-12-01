using Model.Extended.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extended
{
    public class UserKey
    {
        private long userID;
        private string userName;

        public long UserID { get { return userID; } set { userID = value; } }
        public string UserName { get { return userName; } set { userName = value; } }

        public UserKey(long id, string name)
        {
            userID = id;
            userName = name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UserKey)) return false;

            return ((UserKey)obj).UserID == this.UserID && ((UserKey)obj).UserName == this.UserName;
        }

        public override int GetHashCode()
        {
            return (this.UserID + this.UserName).GetHashCode();
        }
    }
    public class SourceInfoWithUserNotes
    {
        public SourceUser sourceUser { get; set; }
        public Source source { get; set; }
        
        public Dictionary<UserKey, List<NoteData>> userNotes { get; set; }

        public List<SourceTagData> tags { get; set; }
    }
}
