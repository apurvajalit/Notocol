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
    
    public partial class NotificationTemp
    {
        public long ID { get; set; }
        public bool ReadStatus { get; set; }
        public int Type { get; set; }
        public long Receiver { get; set; }
        public Nullable<long> SecondaryUser { get; set; }
        public System.DateTime Created { get; set; }
        public Nullable<long> SourceUserID { get; set; }
        public int ReasonCode { get; set; }
        public long SourceID { get; set; }
        public string AdditionalText { get; set; }
        public string tags { get; set; }
        public string note { get; set; }
    }
}