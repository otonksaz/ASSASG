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
    
    public partial class pm_tenancy
    {
        public pm_tenancy()
        {
            this.ass_complaint = new HashSet<ass_complaint>();
        }
    
        public string entity_cd { get; set; }
        public string project_no { get; set; }
        public string trade_name { get; set; }
        public string business_id { get; set; }
        public string tenant_no { get; set; }
        public System.DateTime contract_date { get; set; }
        public System.DateTime commence_date { get; set; }
        public System.DateTime expiry_date { get; set; }
        public string currency_cd { get; set; }
        public Nullable<decimal> apply_no { get; set; }
        public string theme_cd { get; set; }
        public string class_cd { get; set; }
        public string category_cd { get; set; }
        public string staff { get; set; }
        public string cr_terms { get; set; }
        public string statements { get; set; }
        public string reminder { get; set; }
        public string interest { get; set; }
        public string ten_solicitor { get; set; }
        public string ten_solicitor_ref { get; set; }
        public string solicitor { get; set; }
        public string solicitor_ref { get; set; }
        public Nullable<decimal> area { get; set; }
        public string remark { get; set; }
        public string status { get; set; }
        public string reason_cd { get; set; }
        public string audit_user { get; set; }
        public System.DateTime audit_date { get; set; }
        public Nullable<System.DateTime> terminate_date { get; set; }
        public Nullable<decimal> tarrif_percent { get; set; }
        public Nullable<System.DateTime> booking_date { get; set; }
        public string contract_no { get; set; }
        public string rental_basis { get; set; }
        public Nullable<decimal> contract_sum { get; set; }
        public string debtor_type { get; set; }
    
        public virtual cf_business cf_business { get; set; }
        public virtual pl_project pl_project { get; set; }
        public virtual ICollection<ass_complaint> ass_complaint { get; set; }
    }
}
