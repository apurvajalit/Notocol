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
    
    public partial class Follow
    {
        public long ID { get; set; }
        public long follower { get; set; }
        public long followee { get; set; }
        public System.DateTime lastUpdated { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
