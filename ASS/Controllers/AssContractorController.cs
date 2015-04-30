using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASS.Model;

namespace ASS.Controllers
{
    public class AssContractorController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        // GET: AssContractor
        public ActionResult Index()
        {
            var cm_contractor = db.cm_contractor.Include(c => c.ass_user);
            return View(cm_contractor.ToList());
        }

        // GET: AssContractor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cm_contractor cm_contractor = db.cm_contractor.Find(id);
            if (cm_contractor == null)
            {
                return HttpNotFound();
            }
            return View(cm_contractor);
        }

        // GET: AssContractor/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.ass_user, "id", "username");
            return View();
        }

        // POST: AssContractor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,creditor_acct,trade,address,telp,cp_name,user_id,audit_date")] cm_contractor cm_contractor)
        {
            if (ModelState.IsValid)
            {
                cm_contractor.audit_date = DateTime.Now;
                cm_contractor.user_id = int.Parse(User.Identity.Name);

                db.cm_contractor.Add(cm_contractor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.ass_user, "id", "username", cm_contractor.user_id);
            return View(cm_contractor);
        }

        // GET: AssContractor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cm_contractor cm_contractor = db.cm_contractor.Find(id);
            if (cm_contractor == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.ass_user, "id", "username", cm_contractor.user_id);
            return View(cm_contractor);
        }

        // POST: AssContractor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,creditor_acct,trade,address,telp,cp_name,user_id,audit_date")] cm_contractor cm_contractor)
        {
            if (ModelState.IsValid)
            {
                cm_contractor.audit_date = DateTime.Now;
                cm_contractor.user_id = int.Parse(User.Identity.Name);

                db.Entry(cm_contractor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.ass_user, "id", "username", cm_contractor.user_id);
            return View(cm_contractor);
        }

        // GET: AssContractor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cm_contractor cm_contractor = db.cm_contractor.Find(id);
            if (cm_contractor == null)
            {
                return HttpNotFound();
            }
            return View(cm_contractor);
        }

        // POST: AssContractor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cm_contractor cm_contractor = db.cm_contractor.Find(id);
            db.cm_contractor.Remove(cm_contractor);
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
