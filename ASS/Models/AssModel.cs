using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASS.Model;

namespace ASS.Models
{
    public class ViewReminderBase
    {
        public List<ViewReminder> reminderProgress { get; set; }
        public List<ViewReminderDaysHead> reminderDaysHead { get; set; }
    }
    public class ViewReminder
    {
        public int? progressId { get; set; }
        public string descs { get; set; }
        public int countUnit { get; set; }
        public int countProgress { get; set; }
    }

    public class ViewReminderUnitProgress
    {
        public int? progrssId{get;set;}
        public string progressName { get; set; }
        public string entCd { get; set; }
        public string projectNo { get; set; }
        public string lotNo { get; set; }
        public DateTime checkDate { get; set; }
        public int countProgress { get; set; }
    }

    public class ViewReminderDaysHead
    {
        public string title { get; set; }
        public List<ViewReminderDays> reminderDays { get; set; }
    }
    public class ViewReminderDays
    {
        public int remId { get; set; }
        public int? startDay { get; set; }
        public int? endDay { get; set; }
        public string descs { get; set; }
        public int countUnit { get; set; }
        public int countComplaint { get; set; }
        public string classes { get; set; }
    }

    public class ViewProgressDtl
    {
        public List<ass_status> status { get; set; }
        public ass_complaint ass_complaint { get; set; }
    }
}