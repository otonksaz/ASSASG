using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASS.Model;
using ASS.Models;
using ASS.Helpers;
using System.IO;
using System.Net.Mail;

namespace ASS.Controllers
{
    public class AssStatusUpdateController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        public ActionResult GetComplaintDtlByUnit(string entCd, string projectNo, string lotNo)
        {
            List<ass_complaint_dtl> result = db.ass_complaint_dtl
                .Where(a => a.curr_status_id != null && !a.ass_status.name.Contains("CLOSED") && !a.ass_status.name.Contains("CANCEL")
                    && a.ass_complaint.entity_cd == entCd && a.ass_complaint.project_no == projectNo && a.ass_complaint.lot_no == lotNo)
                .OrderBy(a => a.due_date)
                .ToList();
            return PartialView("_tableDetail", result);
        }

        public JsonResult GetLotComplaint()
        {
            var tenans = db.ass_complaint
                .Where(a => a.ass_complaint_dtl.Where(x => x.curr_status_id != null
                    && !x.ass_status.name.Contains("CLOSED") && (!x.ass_status.name.Contains("CANCEL"))).Any())
                .Join(db.cf_business, a => a.business_id, b => b.business_id, (a, b) => new { a, b })
                .Select(a => new
                {
                    entCd = a.a.entity_cd,
                    projectNo = a.a.project_no,
                    lotNo = a.a.lot_no,
                    busId = a.a.business_id,
                    busName = a.b.name,
                    busTelp = a.b.hand_phone,
                    email = a.b.designation
                }).Distinct();
            return Json(tenans, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateJson(AddStatus status)
        {
            string messageResult = "Progress has been updated";
            using (var dbTrans = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (AddStatusDtl dtl in status.dtl)
                    {
                        ass_complaint_dtl compDtl = db.ass_complaint_dtl.Where(a => a.id == dtl.compDtlId).FirstOrDefault();
                        if (compDtl != null)
                        {
                            compDtl.curr_status_id = dtl.statusId;
                            ass_status_update up = new ass_status_update
                            {
                                compdtl_id = dtl.compDtlId,
                                status_id = dtl.statusId,
                                status_date = DateTime.Today,
                                remark = dtl.remark,
                                user_id = int.Parse(User.Identity.Name)
                            };
                            db.ass_status_update.Add(up);
                            db.SaveChanges();

                            ass_complaint complaint = db.ass_complaint
                                .Where(a => a.id == compDtl.comp_id)
                                .FirstOrDefault();
                            if (!complaint.ass_complaint_dtl.Where(x => x.curr_status_id == null || !x.ass_status.name.Contains("CLOSED")).Any())
                                complaint.closing_date = DateTime.Today;

                            bool cancel = true;
                            foreach (ass_complaint_dtl itemDtl in complaint.ass_complaint_dtl)
                            {
                                if (!itemDtl.currentStatus.Contains("CANCEL"))
                                    cancel = false;
                            }

                            if (cancel)
                                complaint.cancel_date = DateTime.Now;

                            ass_status currentStatus = db.ass_status.Where(a => a.id == dtl.statusId).FirstOrDefault();
                            if (currentStatus != null && currentStatus.name.Contains("DONE") && complaint.cp_email != "")
                            {
                                if (!complaint.ass_complaint_dtl.Where(x => x.curr_status_id == null || (!x.ass_status.name.Contains("CLOSED") && !x.ass_status.name.Contains("DONE"))).Any())
                                {
                                    //#GET SENDER EMAIL
                                    ass_email_sender mailSender = db.ass_email_sender.FirstOrDefault();
                                    string templateBody = mailSender.email_template;                                    

                                    //#SEND EMAIL
                                    DateTime today = DateTime.Today;
                                    string mailNo = today.Day.ToString() + "/ASS-GLC/" + IntExtensions.ToRomanNumeral(today.Month) + "/" + today.Year;
                                    string templateHTML;
                                    //Read template file from the App_Data folder
                                    using (var sr = new StreamReader(Server.MapPath("\\App_Data\\Templates\\") + "MailAssDone.html"))
                                    {
                                        templateHTML = sr.ReadToEnd();
                                    }
                                    MailMessage mail = new MailMessage();
                                    mail.To.Add(complaint.cp_email);
                                    //mail.To.Add("otonksaz@gmail.com");
                                    //mail.From = new MailAddress("aftersalesservice.gl@agungsedayu.com");
                                    mail.From = new MailAddress(mailSender.email);
                                    mail.Subject = "Surat Pemberitahuan Komplain Telah Selesai";
                                    string messageBody = @"
                                    <table style='width:100%'>
                                        <tr>
                                            <th>#</th>
                                            <th>Uraian Checklist</th>
                                            <th>Proses</th>
                                            <th>Keterangan</th>
                                        </tr>";

                                    int x = 1;
                                    foreach (ass_complaint_dtl ass_complaint_dtl in complaint.ass_complaint_dtl)
                                    {
                                        string descs = ass_complaint_dtl.ass_category.name + " "
                                            + ass_complaint_dtl.ass_location.name + " "
                                            + ass_complaint_dtl.descs;
                                        string contentDtl = "<tr>";
                                        contentDtl += "<td>" + x.ToString() + "</td>";
                                        contentDtl += "<td>" + descs + "</td>";
                                        contentDtl += "<td>" + ass_complaint_dtl.currentStatus + "</td>";
                                        contentDtl += "<td>" + ass_complaint_dtl.currentRemark + "</td>";
                                        contentDtl += "</tr>";
                                        messageBody += contentDtl;
                                        x++;
                                    }
                                    messageBody += "</table>";

                                    string Body = templateBody.Replace("{emailNo}", mailNo)
                                        .Replace("{cluster}", complaint.pm_tenancy.pl_project.descs)
                                        .Replace("{lotNo}", complaint.lot_no)
                                        .Replace("{date}", DateTime.Today.ToString("dd MMMM yyyy"))
                                        .Replace("{complaintDetail}", messageBody)
                                        .Replace("{complaintDate}", complaint.check_date.ToString("dd MMM yyyy"));
                                    Body = templateHTML.Replace("{body}", Body);
                                    mail.Body = Body;
                                    mail.IsBodyHtml = true;
                                    SmtpClient smtp = new SmtpClient();
                                    smtp.Host = "mx.agungsedayu.com";
                                    smtp.Port = 25;
                                    smtp.UseDefaultCredentials = false;
                                    //smtp.Credentials = new System.Net.NetworkCredential
                                    //("ass.agungsedayu@gmail.com", "aqswde123");// Enter seders User name and password
                                    smtp.Credentials = new System.Net.NetworkCredential
                                    (mailSender.email, EncrypHelpers.Decrypt(mailSender.password));// Enter seders User name and password
                                    smtp.EnableSsl = true;
                                    smtp.Send(mail);

                                    ass_complaint_mail compMail = new ass_complaint_mail
                                    {
                                        mail_no = mailNo,
                                        email_to = complaint.cp_email,
                                        mail_date = today,
                                        comp_id = complaint.id,
                                        user_id = int.Parse(User.Identity.Name),
                                        audit_date = DateTime.Now
                                    };
                                    db.ass_complaint_mail.Add(compMail);
                                    db.SaveChanges();
                                    //#END SEND EMAIL
                                    messageResult = "Progress has been updated and Email has been sent.";
                                }
                            }
                                
                            db.SaveChanges();
                        }
                    }
                    dbTrans.Commit();
                    return Json(new { success = 1, ex = messageResult });
                }
                catch (Exception ex)
                {
                    dbTrans.Rollback();
                    return Json(new { success = 0, ex = ex.ToString() });
                }
            }
        }

        // GET: AssStatusUpdate/Create
        public ActionResult Create()
        {
            ViewBag.compdtl_id = new SelectList(db.ass_complaint_dtl, "id", "descs");
            ViewBag.status_id = new SelectList(db.ass_status, "id", "name");
            return View();
        }

        [HttpPost]
        public JsonResult SetOpen(int comp_id)
        {
            try
            {
                ass_complaint comp = db.ass_complaint
                    .Where(a => a.id == comp_id).FirstOrDefault();

                foreach (ass_complaint_dtl dtl in comp.ass_complaint_dtl.ToList())
                {
                    dtl.curr_status_id = 1;
                    ass_status_update up = new ass_status_update
                    {
                        compdtl_id = dtl.id,
                        status_id = 1,
                        status_date = DateTime.Today,
                        user_id = int.Parse(User.Identity.Name)
                    };
                    db.ass_status_update.Add(up);
                }
                db.SaveChanges();

                return Json(new { success = 1, comp_id = comp_id, Exception = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, comp_id = comp_id, Exception = ex.ToString() });
            }
        }

        public ActionResult ViewComplainByProgress()
        {
            List<ass_status> statusLs = new List<ass_status>();                
            statusLs.Add(new ass_status { id = 0, name = "ANY" });
            statusLs.AddRange(db.ass_status.ToList());

            ViewBag.progressFrom = new SelectList(statusLs, "id", "name");
            ViewBag.progressTo = new SelectList(statusLs, "id", "name");
            
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
