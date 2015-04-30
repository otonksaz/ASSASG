using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASS.Model;
using ASS.Helpers;

namespace ASS.Controllers
{
    public class AssStatusController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        // GET: AssStatus
        public ActionResult Index()
        {
            return View(db.ass_status.ToList());
        }

        // GET: AssStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_status ass_status = db.ass_status.Find(id);
            if (ass_status == null)
            {
                return HttpNotFound();
            }
            return View(ass_status);
        }

        // GET: AssStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,descs")] ass_status ass_status)
        {
            if (ModelState.IsValid)
            {
                db.ass_status.Add(ass_status);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ass_status);
        }

        // GET: AssStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_status ass_status = db.ass_status.Find(id);
            if (ass_status == null)
            {
                return HttpNotFound();
            }
            return View(ass_status);
        }

        // POST: AssStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,descs")] ass_status ass_status)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ass_status).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_status);
        }

        // GET: AssStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_status ass_status = db.ass_status.Find(id);
            if (ass_status == null)
            {
                return HttpNotFound();
            }
            return View(ass_status);
        }

        // POST: AssStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ass_status ass_status = db.ass_status.Find(id);
            db.ass_status.Remove(ass_status);
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
