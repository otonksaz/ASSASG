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
    
    public partial class ass_status_flow
    {
        public int id { get; set; }
        public int current_status { get; set; }
        public int next_status { get; set; }
        public Nullable<int> step_no { get; set; }
    
        public virtual ass_status ass_status_current { get; set; }
        public virtual ass_status ass_status_next { get; set; }
    }
}