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
    
    public partial class DeleteSourceUser_Result
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public string Summary { get; set; }
        public Nullable<bool> Privacy { get; set; }
        public Nullable<int> Rating { get; set; }
        public Nullable<System.DateTime> ModifiedAt { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string thumbnailText { get; set; }
        public Nullable<long> FolderID { get; set; }
        public int noteCount { get; set; }
        public Nullable<long> SourceID { get; set; }
        public Nullable<bool> PrivacyOverride { get; set; }
        public Nullable<int> PrivateNoteCount { get; set; }
    }
}
