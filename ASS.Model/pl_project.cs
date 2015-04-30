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
    
    public partial class pl_project
    {
        public pl_project()
        {
            this.pm_lot = new HashSet<pm_lot>();
            this.pm_tenancy = new HashSet<pm_tenancy>();
            this.ass_migrate_lastmonth = new HashSet<ass_migrate_lastmonth>();
        }
    
        public string entity_cd { get; set; }
        public string project_no { get; set; }
        public Nullable<System.DateTime> project_date { get; set; }
        public string project_ref { get; set; }
        public string debtor_acct { get; set; }
        public string debtor_type { get; set; }
        public string owner_type { get; set; }
        public string location { get; set; }
        public string project_type { get; set; }
        public string descs { get; set; }
        public Nullable<System.DateTime> award_date { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> completion_date { get; set; }
        public string currency_cd { get; set; }
        public decimal contract_amt { get; set; }
        public decimal auth_vo { get; set; }
        public decimal claim_todate { get; set; }
        public decimal ret_todate { get; set; }
        public decimal ret_limit { get; set; }
        public decimal ret_percent { get; set; }
        public Nullable<decimal> max_ret_percent { get; set; }
        public Nullable<System.DateTime> ret_release_date { get; set; }
        public decimal penalty { get; set; }
        public Nullable<decimal> min_claim_amt { get; set; }
        public Nullable<decimal> claim_interval { get; set; }
        public string ret_acct { get; set; }
        public string profit_acct { get; set; }
        public string wip_acct { get; set; }
        public string revenue_acct { get; set; }
        public string memo_acct { get; set; }
        public string project_rev_cd { get; set; }
        public string ret_cd { get; set; }
        public string memo_cd { get; set; }
        public Nullable<decimal> est_cost { get; set; }
        public Nullable<decimal> prev_recog_profit { get; set; }
        public string project_status { get; set; }
        public string remarks { get; set; }
        public string audit_user { get; set; }
        public System.DateTime audit_date { get; set; }
        public Nullable<decimal> project_factor { get; set; }
        public Nullable<decimal> doc_no { get; set; }
        public string claim_method { get; set; }
        public Nullable<System.DateTime> act_complete_dt { get; set; }
        public Nullable<System.DateTime> last_bill_date { get; set; }
        public string div_cd { get; set; }
        public string dept_cd { get; set; }
        public string activity_cd { get; set; }
        public string wstatus { get; set; }
        public string contact_person { get; set; }
        public string contact_telno { get; set; }
        public string contact_email { get; set; }
        public string designation { get; set; }
        public string retention_activity_cd { get; set; }
    
        public virtual ICollection<pm_lot> pm_lot { get; set; }
        public virtual ICollection<pm_tenancy> pm_tenancy { get; set; }
        public virtual ICollection<ass_migrate_lastmonth> ass_migrate_lastmonth { get; set; }
    }
}
