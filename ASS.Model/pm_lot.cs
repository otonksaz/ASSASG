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
    
    public partial class pm_lot
    {
        public string entity_cd { get; set; }
        public string project_no { get; set; }
        public string lot_no { get; set; }
        public string property_cd { get; set; }
        public string descs { get; set; }
        public string block_no { get; set; }
        public string type { get; set; }
        public string location_cd { get; set; }
        public string direction_cd { get; set; }
        public string theme_cd { get; set; }
        public string class_cd { get; set; }
        public string category_cd { get; set; }
        public string zone_cd { get; set; }
        public string level_no { get; set; }
        public string area_uom { get; set; }
        public Nullable<decimal> land_area { get; set; }
        public decimal land_rate { get; set; }
        public string land_tax_cd { get; set; }
        public decimal land_tax_amt { get; set; }
        public decimal land_price { get; set; }
        public decimal land_net_price { get; set; }
        public Nullable<decimal> extra_land_area { get; set; }
        public decimal extra_land_rate { get; set; }
        public decimal extra_land_price { get; set; }
        public decimal build_up_area { get; set; }
        public Nullable<decimal> budget_cost { get; set; }
        public string package_cd { get; set; }
        public Nullable<decimal> package_price { get; set; }
        public string package_tax_cd { get; set; }
        public Nullable<decimal> package_tax_amt { get; set; }
        public Nullable<decimal> package_net_price { get; set; }
        public Nullable<decimal> other_amt { get; set; }
        public string other_tax_cd { get; set; }
        public Nullable<decimal> other_tax_amt { get; set; }
        public Nullable<decimal> other_net_amt { get; set; }
        public Nullable<decimal> min_selling_price { get; set; }
        public Nullable<System.DateTime> sold_date { get; set; }
        public string sold_type { get; set; }
        public Nullable<decimal> type_amt { get; set; }
        public Nullable<decimal> rental { get; set; }
        public Nullable<decimal> rent_rate { get; set; }
        public string rate_type { get; set; }
        public string rent_type { get; set; }
        public Nullable<decimal> comm_amt { get; set; }
        public Nullable<decimal> bgt_rate { get; set; }
        public Nullable<decimal> budget_amt { get; set; }
        public Nullable<decimal> share_amt { get; set; }
        public Nullable<decimal> rentable_area { get; set; }
        public Nullable<decimal> tariff_percent { get; set; }
        public string deposit_flag { get; set; }
        public Nullable<decimal> deposit_amt { get; set; }
        public string owner_acct { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string addr3 { get; set; }
        public string post_cd { get; set; }
        public Nullable<decimal> pic_x { get; set; }
        public Nullable<decimal> pic_y { get; set; }
        public string status { get; set; }
        public decimal prev_profit_recog { get; set; }
        public string sales_flag { get; set; }
        public string rent_flag { get; set; }
        public string ref_no { get; set; }
        public string remarks { get; set; }
        public string audit_user { get; set; }
        public System.DateTime audit_date { get; set; }
        public Nullable<decimal> area { get; set; }
        public string rented_status { get; set; }
        public Nullable<decimal> pic_x_all { get; set; }
        public Nullable<decimal> pic_y_all { get; set; }
    
        public virtual pl_project pl_project { get; set; }
    }
}
