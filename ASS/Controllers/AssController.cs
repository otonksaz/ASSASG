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
using Microsoft.AspNet.Identity;

namespace ASS.Controllers
{
    public class AssController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        // GET: Ass
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("Account/Login");
            }
            return View(db.ass_complaint.Where(a => a.ass_complaint_dtl
                .Where(x => x.curr_status_id == null).Any() || 
                    (a.input_date.Year == DateTime.Today.Year && a.input_date.Month == DateTime.Today.Month && a.input_date.Day == DateTime.Today.Day))
                .ToList()
            );
        }
        public ActionResult ViewAll()
        {
            return View();
        }

        public JsonResult GetAllComplaints()
        {
            var tenans = db.ass_complaint
                .Select(a => new
                {
                    id = a.id,
                    entCd = a.entity_cd,
                    projectNo = a.project_no,
                    lotNo = a.lot_no,
                    busId = a.business_id,
                    busName = a.cp_name,
                    busTelp = a.cp_telp,
                    email = a.cp_email,
                    descs = a.descs,
                    checkDate = a.check_date,
                    contractorName = a.cm_contractor.trade
                }).Distinct();
            var data = new { data = tenans };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: Ass/Details/5
        public ActionResult Details(int? id)
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

        public JsonResult GetComplainDetails(int? comp_id)
        {
            var result = db.ass_complaint_dtl
                .Where(a => a.comp_id == comp_id)
                .Select(a => new
                {
                    no = a.id,
                    cat_id = a.cat_id,
                    Category = a.ass_category.name,
                    location_id = a.location_id,
                    Location = a.ass_location.name,
                    descs = a.descs,
                    due_date = a.due_date
                });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Ass/Create
        public ActionResult Create()
        {
            List<ass_category> catLs = db.ass_category.ToList();
            catLs.Add(new ass_category { id = 0, name = " " });

            List<ass_location> locLs = db.ass_location.ToList();
            locLs.Add(new ass_location { id = 0, name = " " });

            ViewBag.contractor_id = new SelectList(db.cm_contractor, "id", "trade");
            ViewBag.category_id = new SelectList(catLs.OrderBy(a => a.id == 0 ? 0 : 1).ThenBy(a => a.name).ToList(), "id", "name", new { id = 0 });
            ViewBag.location_id = new SelectList(locLs.OrderBy(a => a.id == 0 ? 0 : 1).ThenBy(a => a.name).ToList(), "id", "name", new { id = 0 });
            return View("Create_");
        }

        // POST: Ass/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,complaint_cd,entity_cd,project_no,lot_no,business_id,cp_name,cp_telp,check_date,contractor_id,descs,input_date,user_id,attach_file")] ass_complaint ass_complaint)
        {
            if (ModelState.IsValid)
            {
                db.ass_complaint.Add(ass_complaint);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ass_complaint);
        }

        [HttpPost]
        public JsonResult CreateJson(ass_complaint ass_complaint)
        {
            try
            {
                ass_complaint curComp = db.ass_complaint
                    .Where(a => a.id == ass_complaint.id)
                    .FirstOrDefault();               

                if (curComp != null)
                {
                    curComp.entity_cd = ass_complaint.entity_cd;
                    curComp.lot_no = ass_complaint.lot_no;
                    curComp.project_no = ass_complaint.project_no;
                    curComp.business_id = ass_complaint.business_id;
                    curComp.cp_name = ass_complaint.cp_name;
                    curComp.cp_telp = ass_complaint.cp_telp;
                    curComp.cp_email = ass_complaint.cp_email;
                    curComp.check_date = ass_complaint.check_date;
                    curComp.contractor_id = ass_complaint.contractor_id;

                    curComp.ass_complaint_dtl.ToList().ForEach(a => db.ass_complaint_dtl.Remove(a));

                    foreach (ass_complaint_dtl dtl in ass_complaint.ass_complaint_dtl)
                        db.ass_complaint_dtl.Add(dtl);
                }
                else
                {
                    ass_complaint.input_date = DateTime.Now;
                    ass_complaint.complaint_cd = "";
                    ass_complaint.user_id = int.Parse(User.Identity.Name);

                    db.ass_complaint.Add(ass_complaint);
                }
                db.SaveChanges();
                return Json(new { success = 1, comp_id = ass_complaint.id, ex = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, comp_id = ass_complaint.id, ex = ex.ToString() });
            }            
        }

        // GET: Ass/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_complaint ass_complaint = db.ass_complaint.Find(id);
            if (ass_complaint == null ||
                ((ass_complaint.input_date.Year != DateTime.Today.Year 
                || ass_complaint.input_date.Month != DateTime.Today.Month 
                || ass_complaint.input_date.Day != DateTime.Today.Day) && !ass_complaint.lastStatus.Contains("INPUT")))
            {
                return HttpNotFound();
            }
            ViewBag.contractor_id = new SelectList(db.cm_contractor, "id", "trade", ass_complaint.contractor_id);
            ViewBag.categories = db.ass_category.ToList();
            ViewBag.locations = db.ass_location.ToList();
            return View("Edit_",ass_complaint);
        }

        // POST: Ass/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,complaint_cd,entity_cd,project_no,lot_no,business_id,cp_name,cp_telp,check_date,contractor_id,descs,input_date,user_id,attach_file")] ass_complaint ass_complaint)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ass_complaint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_complaint);
        }

        // GET: Ass/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Ass/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ass_complaint ass_complaint = db.ass_complaint.Find(id);
            db.ass_complaint.Remove(ass_complaint);
            db.SaveChanges();
            return RedirectToAction("Index");
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
