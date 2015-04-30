using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASS.Model;
using ASS.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Globalization;
using ASS.Helpers;

namespace ASS.Controllers
{
    public class AssReportController : Controller
    {
        // GET: AssReport
        POM_DasanaEntities db = new POM_DasanaEntities();

        public ActionResult AssCetak(int? id)
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
            return View(ass_complaint);
        }

        public ActionResult printOut(int? id)
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
            List<PrintOut> poLs = new List<PrintOut>();
            poLs = ass_complaint.ass_complaint_dtl
                .Select(a => new PrintOut
                {
                    no = a.ass_complaint.id,
                    ownerName = a.ass_complaint.cp_name,
                    telp = a.ass_complaint.cp_telp,
                    address = a.ass_complaint.lot_no,
                    email = a.ass_complaint.cp_email,
                    bast = a.ass_complaint.pm_tenancy.commence_date,
                    checkListDate = a.ass_complaint.check_date,
                    finishDate = a.ass_complaint.ass_complaint_dtl.OrderByDescending(x => x.due_date).FirstOrDefault().due_date,
                    contractorName = a.ass_complaint.cm_contractor.trade,
                    cpContractor = a.ass_complaint.controller_name,
                    headDescs = a.ass_complaint.descs,
                    descs = a.ass_category.name + " " + a.ass_location.name + " " + a.descs
                }).ToList();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "AssPrintOut.rpt"));
            rd.SetDataSource(poLs);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                //return File(stream, "application/pdf", id.ToString() + ".pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception ex) { throw; }
        }

        public ActionResult RekapItemKomplainView()
        {
            ViewBag.monthId = new SelectList(
                Enumerable.Range(1, 12)
                .Select(r => new
                {
                    Text = new DateTime(2000, r, 1).ToString("MMMM"),
                    Value = r.ToString()
                }), "Value", "Text", DateTime.Today.Month);
            return View();
        }

        public ActionResult RekapItemKomplainPrint(string stSDate, string stEDate)
        {
            DateTime sDate = new DateTime();
            DateTime eDate = new DateTime();
            DateTime sPastMontDate = new DateTime();
            if (stSDate == "")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            sDate = DateTime.Parse(stSDate);
            sPastMontDate = sDate.AddMonths(-1);
            if (stEDate == "")
                eDate = sDate.AddMonths(1);
            else
                eDate = DateTime.Parse(stEDate);

            List<RekapItemKomplain> poLs = new List<RekapItemKomplain>();
            //#DETAIL CATEGORY BULAN INI
            poLs = db.ass_category
                .Join(db.pl_project, a => 1, b => 1, (a, b) => new { a, b })
                .Select(x => new RekapItemKomplain
                {
                    cluster = x.b.descs.Replace("cluster ", ""),
                    catGroup = "6",
                    catName = x.a.name,
                    catCount = x.a.ass_complaint_dtl
                        .Where(z => z.ass_complaint.check_date >= sDate && z.ass_complaint.check_date <= eDate
                            && z.ass_complaint.project_no == x.b.project_no && z.ass_complaint.cancel_date == null)
                        .GroupBy(z => new { cat = z.ass_category.name, entCd = z.ass_complaint.entity_cd, projectNo = z.ass_complaint.project_no, z.ass_complaint.lot_no })
                        .Select(z => new { z.Key.cat, z.Key.entCd, z.Key.projectNo, z.Key.lot_no })
                        .GroupBy(z => new { z.cat, z.entCd, z.projectNo })
                        .Select(a => a.Key.cat)
                        .Count()
                }).ToList();

            //#ON PROGRESS BULAN LALU
            poLs.AddRange(
                db.ass_complaint
                    .Where(a => a.check_date < sDate
                        && (a.closing_date == null || (a.closing_date.Value) >= sDate))
                    .Join(db.pl_project, a => new { a.entity_cd, a.project_no }, b => new { b.entity_cd, b.project_no }, (a, b) => new { a, b })
                     .GroupBy(a => new { a.a.entity_cd, a.a.project_no, cluster = a.b.descs.Replace("cluster ", "") })
                    .Select(a => new RekapItemKomplain
                    {
                        cluster = a.Key.cluster,
                        catGroup = "1",
                        catName = "BULAN LALU",
                        catCount = a.Count()
                    })
                    .ToList().Union(
                        db.ass_migrate_lastmonth
                        .Select(x => new RekapItemKomplain
                        {
                            cluster = x.pl_project.descs,
                            catGroup = "1",
                            catName = "BULAN LALU",
                            catCount = x.on_progress
                        }).ToList()
                    ).Union(
                        db.ass_migrate_closing
                        .Where(x => x.closing_date < sDate)
                        .Select(x => new RekapItemKomplain
                        {
                            cluster = x.ass_migrate_lastmonth.pl_project.descs,
                            catGroup = "1",
                            catName = "BULAN LALU",
                            catCount = x.closing * -1
                        }).ToList()
                    ).GroupBy(x => new { cluster = x.cluster, catGroup = x.catGroup, catName = x.catName })
                    .Select(x => new RekapItemKomplain { 
                        cluster = x.Key.cluster,
                        catGroup = "1",
                        catName = "BULAN LALU",
                        catCount = x.Select(y => y.catCount).DefaultIfEmpty().Sum()
                    }).ToList()
                );

            //#CLOSING BULAN LALU
            poLs.AddRange(
                db.ass_complaint
                    .Where(a => a.closing_date != null && (a.closing_date.Value >= sPastMontDate && a.closing_date.Value < sDate))
                    .Join(db.pl_project, a => new { a.entity_cd, a.project_no }, b => new { b.entity_cd, b.project_no }, (a, b) => new { a, b })
                    .GroupBy(a => new { a.a.entity_cd, a.a.project_no, cluster = a.b.descs.Replace("cluster ", "") })
                    .Select(a => new RekapItemKomplain
                    {
                        cluster = a.Key.cluster,
                        catGroup = "2",
                        catName = "CLOSING",
                        catCount = a.Count()
                    }).ToList().Union(
                        db.ass_migrate_closing
                        .Where(x => x.closing_date < sDate)
                        .Select(x => new RekapItemKomplain
                        {
                            cluster = x.ass_migrate_lastmonth.pl_project.descs,
                            catGroup = "2",
                            catName = "CLOSING",
                            catCount = x.closing * -1
                        }).ToList()
                    ).GroupBy(x => new { cluster = x.cluster, catGroup = x.catGroup, catName = x.catName })
                    .Select(x => new RekapItemKomplain
                    {
                        cluster = x.Key.cluster,
                        catGroup = "2",
                        catName = "CLOSING",
                        catCount = x.Select(y => y.catCount).DefaultIfEmpty().Sum()
                    }).ToList()
                );

            //#COMPLAINT BULAN  INI
            poLs.AddRange(
                db.ass_complaint
                    .Where(a => a.check_date >= sDate && a.check_date <= eDate && a.cancel_date == null)
                    .Join(db.pl_project, a => new { a.entity_cd, a.project_no }, b => new { b.entity_cd, b.project_no }, (a, b) => new { a, b })
                     .GroupBy(a => new { a.a.entity_cd, a.a.project_no, cluster = a.b.descs.Replace("cluster ", ""), lotno = a.a.lot_no })
                     .Select(a => new { a.Key.entity_cd, a.Key.project_no, a.Key.cluster, a.Key.lotno })
                     .GroupBy(a => new { a.entity_cd, a.project_no, a.cluster })
                    .Select(a => new RekapItemKomplain
                    {
                        cluster = a.Key.cluster,
                        catGroup = "3",
                        catName = "BULAN INI",
                        catCount = a.Count()
                    })
                    .ToList()
                );

            //#CLOSING BULAN INI
            poLs.AddRange(
                db.ass_complaint
                    .Where(a => a.closing_date != null && (a.closing_date.Value >= sDate && a.closing_date <= eDate) && a.cancel_date == null)
                    .Join(db.pl_project, a => new { a.entity_cd, a.project_no }, b => new { b.entity_cd, b.project_no }, (a, b) => new { a, b })
                    .GroupBy(a => new { a.a.entity_cd, a.a.project_no, cluster = a.b.descs.Replace("cluster ", ""), lotno = a.a.lot_no })
                    .Select(a => new { a.Key.entity_cd, a.Key.project_no, a.Key.cluster, a.Key.lotno })
                    .GroupBy(a => new { a.entity_cd, a.project_no, a.cluster })
                    .Select(a => new RekapItemKomplain
                    {
                        cluster = a.Key.cluster,
                        catGroup = "4",
                        catName = "CLOSING",
                        catCount = a.Count()
                    })
                    .ToList()
                );
            //#ON PROGRESS SAMPAI BULAN INI
            poLs.AddRange(
                db.ass_complaint
                    .Where(a => a.check_date <= eDate
                        && (a.closing_date == null || (a.closing_date.Value > eDate)) && a.cancel_date == null)
                    .Join(db.pl_project, a => new { a.entity_cd, a.project_no }, b => new { b.entity_cd, b.project_no }, (a, b) => new { a, b })
                    .GroupBy(a => new { a.a.entity_cd, a.a.project_no, cluster = a.b.descs.Replace("cluster ", ""), lotno = a.a.lot_no })
                    .Select(a => new { a.Key.entity_cd, a.Key.project_no, a.Key.cluster, a.Key.lotno })
                    .GroupBy(a => new { a.entity_cd, a.project_no, a.cluster })
                    .Select(a => new RekapItemKomplain
                    {
                        cluster = a.Key.cluster,
                        catGroup = "5",
                        catName = "ON PROGRESS",
                        catCount = a.Count()
                    })
                    .ToList()
                );

            //#Sub Report
            List<int> catIdLs = new List<int>();
            List<SimpleClass> subLs = new List<SimpleClass>();
            subLs = db.ass_complaint_dtl
                .Where(a => a.ass_complaint.check_date >= sDate && a.ass_complaint.check_date <= eDate)
                .GroupBy(a => new { a.cat_id, a.ass_category.name, a.ass_complaint.entity_cd, a.ass_complaint.project_no, a.ass_complaint.lot_no })
                .Select(a => new { catId = a.Key.cat_id, catName = a.Key.name, count = 1 })
                .GroupBy(a => new { a.catId, a.catName})
                .Select(a => new { catId = a.Key.catId, a.Key.catName, count = a.Count() })
                .OrderByDescending(a => a.count)
                .Select(a => new SimpleClass { 
                    value = a.catName,
                    description = ""
                }).Take(3).ToList();

            //subLs = db.ass_complaint_dtl
            //    .Where(a => a.ass_complaint.check_date.Year * 100 + a.ass_complaint.check_date.Month == period
            //        && catIdLs.Contains(a.cat_id))
            //    .GroupBy(a => new { cat = a.ass_category.name, loc = a.ass_location.name })
            //    .Select(a => new { cat = a.Key.cat, loc = a.Key.loc, count = a.Count() })
            //    .ToList()
            //    .Partition(
            //        a => a.cat,
            //        a => a.OrderByDescending(x => x.count),
            //        (a, rn) => new { rn = rn, a = a }
            //    ).ToList()
            //    .Where(a => a.rn == 0 || a.rn == 1)
            //    .Select(a => new SimpleClass
            //    {
            //        value = a.a.cat,
            //        description = a.a.loc
            //    }).ToList();

            //subLs = db.ass_complaint_dtl
            //    .Where(a => a.ass_complaint.check_date >= sDate && a.ass_complaint.check_date <= eDate
            //        && catIdLs.Contains(a.cat_id))
            //    .GroupBy(a => new { cat = a.ass_category.name })
            //    .Select(a => new SimpleClass
            //    {
            //        value = a.Key.cat,
            //        description = ""
            //    }).ToList();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptRekapItemKomplain.rpt"));
            rd.SetDataSource(poLs);
            rd.Subreports[0].SetDataSource(subLs);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                //return File(stream, "application/pdf", id.ToString() + ".pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception ex) { throw; }
        }

        public ActionResult RekapKomplainKontraktorView()
        {
            return View();
        }

        public ActionResult RekapKomplainKontraktorPrint(string stSDate, string stEDate)
        {
            DateTime sDate = DateTime.Parse(stSDate);
            DateTime eDate = DateTime.Parse(stEDate);

            List<RekapKomplainKontraktor> rs = db.ass_complaint
                .Where(a => a.check_date >= sDate && a.check_date <= eDate)
                .GroupBy(a => new { entCd = a.pm_tenancy.pl_project.entity_cd, projectNo = a.pm_tenancy.pl_project.project_no, clusterName = a.pm_tenancy.pl_project.descs, contractorName = a.cm_contractor.trade })
                .Select(a => new RekapKomplainKontraktor
                {
                    sDate = sDate,
                    eDate = eDate,
                    clusterName = a.Key.clusterName,    
                    contractorName = a.Key.contractorName,
                    complaintCount = a.Where(x => x.ass_complaint_dtl.Where(z => !z.ass_status.name.Contains("CLOSED")).Any()).Count(),
                    complaintClosed = a.Where(x => x.ass_complaint_dtl.Where(z => z.ass_status.name.Contains("CLOSED")).Count() == x.ass_complaint_dtl.Count()).Count(),
                    unitCount = a.FirstOrDefault().cm_contractor.pm_lot_contractor.Where(x => x.ent_cd == a.Key.entCd && x.project_no == a.Key.projectNo).Count()
                }).ToList();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "RptRekapKomplainKontraktor.rpt"));
            rd.SetDataSource(rs);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                //return File(stream, "application/pdf", id.ToString() + ".pdf");
                return File(stream, "application/pdf");
            }
            catch (Exception ex) { throw; }
        }
    }    
}