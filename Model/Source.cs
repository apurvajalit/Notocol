//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Source
    {
        public Source()
        {
            this.SourceUsers = new HashSet<SourceUser>();
        }
    
        public long ID { get; set; }
        public string url { get; set; }
        public byte[] uriHash { get; set; }
        public string faviconURL { get; set; }
        public string title { get; set; }
        public System.DateTime created { get; set; }
    
        public virtual ICollection<SourceUser> SourceUsers { get; set; }
    }
}
