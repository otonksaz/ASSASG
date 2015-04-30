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
    
    public partial class ass_complaint_dtl
    {
        public ass_complaint_dtl()
        {
            this.ass_status_update = new HashSet<ass_status_update>();
        }
    
        public int id { get; set; }
        public int comp_id { get; set; }
        public int cat_id { get; set; }
        public int location_id { get; set; }
        public string descs { get; set; }
        public System.DateTime due_date { get; set; }
        public byte[] attach_file { get; set; }
        public Nullable<int> curr_status_id { get; set; }
    
        public virtual ass_category ass_category { get; set; }
        public virtual ass_complaint ass_complaint { get; set; }
        public virtual ass_location ass_location { get; set; }
        public virtual ICollection<ass_status_update> ass_status_update { get; set; }
        public virtual ass_status ass_status { get; set; }
    }
}