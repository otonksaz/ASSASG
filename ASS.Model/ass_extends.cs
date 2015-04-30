using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ASS.Model
{
    public partial class ass_complaint_dtl
    {
        public List<ass_status> nextStatus
        {
            get {
                return ass_status.status_flow_current.OrderBy(a => a.step_no).Select(a => a.ass_status_next).ToList();
            }
        }

        public string currentStatus
        {
            get { return curr_status_id == null ? "Input" : ass_status.name; }
        }

        public DateTime currentStatusDate
        {
            get {
                return ass_status_update.Count() == 0 ? 
                    (ass_complaint == null ? DateTime.Now : ass_complaint.check_date) 
                    : ass_status_update.OrderByDescending(x => x.status_date).FirstOrDefault().status_date;
            }
        }

        public string currentRemark
        {
            get
            {
                return ass_status_update.Count() == 0 ? ""
                    : ass_status_update.OrderByDescending(x => x.status_date).FirstOrDefault().remark;
            }
        }

        public int dueDateDiff
        {
            get { return (DateTime.Today - due_date).Days; }
        }
    }

    public partial class ass_reminder
    {
        public bool isOnlyMe { get; set; }
        public bool getIsOnlyMe { get { return owner_user == null ? false : true; } }
    }

    public partial class ass_email_sender
    {
        [AllowHtml]
        public string tempTemplate { get; set; }
    }

    public partial class ass_complaint
    {
        public string lastStatus { get {
            return ass_complaint_dtl.SelectMany(x => x.ass_status_update).Count() == 0 ?
                "INPUT" :
                ass_complaint_dtl.SelectMany(x => x.ass_status_update).OrderByDescending(x => x.status_date).FirstOrDefault().ass_status.name;
        } }
        public DateTime lastUpdate { get {
            return ass_complaint_dtl.SelectMany(x => x.ass_status_update).Count() == 0 ?
                    input_date :
                    ass_complaint_dtl.SelectMany(x => x.ass_status_update).OrderByDescending(x => x.status_date).FirstOrDefault().status_date;
        } }
    }
}
