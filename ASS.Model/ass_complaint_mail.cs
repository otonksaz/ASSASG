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
    
    public partial class ass_complaint_mail
    {
        public int id { get; set; }
        public int comp_id { get; set; }
        public string mail_no { get; set; }
        public System.DateTime mail_date { get; set; }
        public int user_id { get; set; }
        public System.DateTime audit_date { get; set; }
        public string email_to { get; set; }
    
        public virtual ass_complaint ass_complaint { get; set; }
        public virtual ass_user ass_user { get; set; }
    }
}
