//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASS.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class ass_reminder
    {
        public int id { get; set; }
        public string descs { get; set; }
        public string clases { get; set; }
        public Nullable<int> start_day { get; set; }
        public Nullable<int> end_day { get; set; }
        public int user_id { get; set; }
        public System.DateTime audit_date { get; set; }
        public Nullable<int> owner_user { get; set; }
    
        public virtual ass_user ass_user { get; set; }
        public virtual ass_user ass_user_owner { get; set; }
    }
}
