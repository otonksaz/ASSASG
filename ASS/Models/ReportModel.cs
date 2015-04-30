using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASS.Models
{
    public class PrintOut
    {
        public int no { get; set; }
        public string ownerName { get; set; }
        public string telp { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public DateTime bast { get; set; }
        public DateTime checkListDate { get; set; }
        public DateTime finishDate { get; set; }
        public string contractorName { get; set; }
        public string cpContractor { get; set; }
        public string headDescs { get; set; }
        public string descs { get; set; }        
    }

    public class RekapItemKomplain
    {
        public string cluster { get; set; }
        public string catGroup { get; set; }
        public string catName { get; set; }
        public int catCount { get; set; }
    }

    public class SimpleClass
    {
        public string value { get; set; }
        public string description { get; set; }
    }

    public class RekapKomplainKontraktor
    {
        public DateTime sDate { get; set; }
        public DateTime eDate { get; set; }
        public string clusterName { get; set; }
        public string contractorName { get; set; }
        public int complaintCount { get; set; }
        public int complaintClosed { get; set; }
        public int complainSum
        {
            get
            {
                return complaintCount + complaintClosed;
            }
        }
        public int unitCount { get; set; }
        public decimal complainPersen
        {
            get
            {
                return (complaintCount + complaintClosed) == 0 || unitCount == 0 ? 0 : (complaintCount + complaintClosed) / unitCount * 100;
            }
        }
    }

    public class ComplainDtl
    {
        public int no {get;set;}
        public string unit { get; set; }
        public string nama { get; set; }
        public string telp { get; set; }
        public DateTime checkListDate { get; set; }
        public string descs { get; set; }
        public string bastDate { get; set; }
        public DateTime targetDate { get; set; }
        public string contractorName { get; set; }
        public string pic { get; set; }
        public string compAction { get; set; }
    }
}