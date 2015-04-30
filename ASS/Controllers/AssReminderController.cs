using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASS.Model;
using ASS.Models;
using System.Data.Entity.SqlServer;
using System.Net;

namespace ASS.Controllers
{
    public class AssReminderController : Controller
    {
        POM_DasanaEntities db = new POM_DasanaEntities();
        // GET: AssReminder
        public ActionResult Index()
        {
            List<ViewReminder> reminderProgress = db.ass_complaint_dtl
                .Where(a => a.curr_status_id == null || (!a.ass_status.name.Contains("CLOSED") &&!a.ass_status.name.Contains("CANCEL")))
                .GroupBy(a => new
                {
                    statusId = a.curr_status_id,
                    statusDescs = a.curr_status_id == null ? "INPUT" : a.ass_status.name
                })
                .Select(a => new ViewReminder
                {
                    progressId = a.Key.statusId.Value,
                    descs = a.Key.statusDescs,
                    countUnit = a.Select(x => x.ass_complaint.lot_no + x.ass_complaint.project_no).Distinct().Count(),
                    countProgress = a.Count()
                }).ToList();

            List<ViewReminderDaysHead> reminderDaysHead = new List<ViewReminderDaysHead>();
            List<ViewReminderDays> reminderDays = new List<ViewReminderDays>();
            reminderDays = this.ProgressDays(null);
            if (reminderDays.Count() != 0)
            {
                reminderDaysHead.Add(new ViewReminderDaysHead
                {
                    title = "Reminder",
                    reminderDays = reminderDays
                });
            }

            reminderDays = this.ProgressDays(int.Parse(User.Identity.Name));
            if (reminderDays.Count() != 0)
            {
                reminderDaysHead.Add(new ViewReminderDaysHead
                {
                    title = "Reminder Me",
                    reminderDays = reminderDays
                });
            }

            ViewReminderBase result = new ViewReminderBase
            {
                reminderProgress = reminderProgress,
                reminderDaysHead = reminderDaysHead
            };
            return View(result);
        }

        public ActionResult ReminderUnitProgress(int? idProgress)
        {
            List<ViewReminderUnitProgress> result = new List<ViewReminderUnitProgress>();

            string progressName = "";
            if (idProgress == null)
                progressName = "Input";
            else if (idProgress == 0)
            {
                progressName = "Over Due";
            }
            else
                progressName = db.ass_status.Where(a => a.id == idProgress).FirstOrDefault().name;

            if (idProgress != 0) { 
                result = db.ass_complaint_dtl
                    .Where(a => idProgress == null ? a.curr_status_id == null : a.curr_status_id == idProgress)
                    .GroupBy(a => new {
                        entCd = a.ass_complaint.entity_cd,
                        projectNo = a.ass_complaint.project_no,
                        lotNo = a.ass_complaint.lot_no
                    })
                    .Select(a => new ViewReminderUnitProgress{
                        progrssId = idProgress.Value,
                        progressName = progressName,
                        entCd = a.Key.entCd,
                        projectNo = a.Key.projectNo,
                        lotNo = a.Key.lotNo,
                        checkDate = a.OrderBy(x => x.ass_complaint.check_date).FirstOrDefault().ass_complaint.check_date,
                        countProgress = a.Count()
                    }).ToList();
            }
            else
            {
                result = db.ass_complaint_dtl
                    .Where(a => (a.curr_status_id == null || !a.ass_status.name.Contains("CLOSED")) && a.due_date < DateTime.Today)
                    .GroupBy(a => new
                    {
                        entCd = a.ass_complaint.entity_cd,
                        projectNo = a.ass_complaint.project_no,
                        lotNo = a.ass_complaint.lot_no
                    })
                    .Select(a => new ViewReminderUnitProgress
                    {
                        progrssId = idProgress.Value,
                        progressName = progressName,
                        entCd = a.Key.entCd,
                        projectNo = a.Key.projectNo,
                        lotNo = a.Key.lotNo,
                        checkDate = a.OrderBy(x => x.ass_complaint.check_date).FirstOrDefault().ass_complaint.check_date,
                        countProgress = a.Count()
                    }).ToList();
            }
            return View(result);

        }

        public ActionResult ProgressComplainByUnit(string entCd, string projectNo, string lotNo)
        {
            List<ass_complaint> result = db.ass_complaint
                .Where(a => a.entity_cd == entCd && a.project_no == projectNo && a.lot_no == lotNo)
                .OrderBy(a => a.check_date)
                .ToList();
            return View(result);
        }

        public ActionResult ProgressByComplaint(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_complaint ass_complaint = db.ass_complaint.Find(id);
            if (ass_complaint == null)
            {
                return HttpNotFound();
            }

            List<ass_status> statusLs = db.ass_status.ToList();
            ViewProgressDtl result = new ViewProgressDtl
            {
                ass_complaint = ass_complaint,
                status = statusLs
            };

            return View(result);
        }

        public ActionResult ProgressByUnit(string entCd, string projectNo, string lotNo)
        {
            List<ass_complaint_dtl> result = db.ass_complaint_dtl
                .Where(a => (a.curr_status_id == null 
                    || (!a.ass_status.name.Contains("CLOSED") 
                    && !a.ass_status.name.Contains("CANCEL")))
                    && a.ass_complaint.entity_cd == entCd && a.ass_complaint.project_no == projectNo && a.ass_complaint.lot_no == lotNo)
                .OrderBy(a => a.due_date)
                .ToList();
            return View(result);
        }

        public ActionResult ReminderUnit(int remId)
        {
            ass_reminder reminder = db.ass_reminder
                .Where(a => a.id == remId)
                .FirstOrDefault();

            List<ass_complaint_dtl> dtlLs = db.ass_complaint_dtl
                .Where(a => a.curr_status_id == null || !a.ass_status.name.Contains("CLOSED"))
                .ToList();

            List<ViewReminderUnitProgress> result = new List<ViewReminderUnitProgress>();
            if (reminder.start_day == null)
                dtlLs = dtlLs.Where(a => a.dueDateDiff <= reminder.end_day).ToList();
            else if (reminder.end_day == null)
                dtlLs = dtlLs.Where(a => a.dueDateDiff >= reminder.start_day).ToList();
            else
                dtlLs = dtlLs.Where(a => a.dueDateDiff >= reminder.start_day && a.dueDateDiff <= reminder.end_day).ToList();

            result = dtlLs
                .GroupBy(a => new
                {
                    entCd = a.ass_complaint.entity_cd,
                    projectNo = a.ass_complaint.project_no,
                    lotNo = a.ass_complaint.lot_no
                })
                .Select(a => new ViewReminderUnitProgress
                {
                    progrssId = remId,
                    progressName = reminder.descs,
                    entCd = a.Key.entCd,
                    projectNo = a.Key.projectNo,
                    lotNo = a.Key.lotNo,
                    checkDate = a.OrderBy(x => x.ass_complaint.check_date).FirstOrDefault().ass_complaint.check_date,
                    countProgress = a.Count()
                }).ToList();
            return View("ReminderUnitProgress", result);
        }

        private List<ViewReminderDays> ProgressDays(int? userId)
        {
            List<ViewReminderDays> result = new List<ViewReminderDays>();
            List<ass_complaint_dtl> dtlLs = db.ass_complaint_dtl
                .Where(a => a.curr_status_id == null || !a.ass_status.name.Contains("CLOSED"))
                .ToList();

            List<ass_reminder> reminderLs = db.ass_reminder
                .OrderBy(a => a.start_day == null ? 0 : 1)
                .ThenBy(a => a.start_day)
                .ToList();

            if (userId == null)
                reminderLs = reminderLs.Where(a => a.owner_user == null).ToList();
            else
                reminderLs = reminderLs.Where(a => a.owner_user == userId).ToList();

            foreach (ass_reminder rem in reminderLs)
            {
                ViewReminderDays add = new ViewReminderDays
                {
                    remId = rem.id,
                    descs = rem.descs,                    
                    countUnit = dtlLs
                        .Where(x => rem.start_day == null ? 
                            x.dueDateDiff <= rem.end_day : 
                            rem.end_day == null ? 
                                x.dueDateDiff >= rem.start_day :
                                x.dueDateDiff >= rem.start_day && x.dueDateDiff <= rem.end_day)
                        .Select(x => x.ass_complaint.lot_no + x.ass_complaint.project_no)
                        .Distinct()
                        .Count(),
                    countComplaint = dtlLs
                        .Where(x => rem.start_day == null ?
                            x.dueDateDiff <= rem.end_day :
                            rem.end_day == null ?
                                x.dueDateDiff >= rem.start_day :
                                x.dueDateDiff >= rem.start_day && x.dueDateDiff <= rem.end_day)
                        .Count(),
                    classes = rem.clases
                };
                result.Add(add);
            }
            return result;
        }
    }
}