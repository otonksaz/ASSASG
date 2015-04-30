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
    public class AssLocationController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        // GET: AssLocation
        public ActionResult Index()
        {
            return View(db.ass_location.ToList());
        }

        public JsonResult getLocation()
        {
            var loactions = db.ass_location.Select(a => new
            {
                a.id,
                a.name,
                a.descs
            });
            return Json(loactions, JsonRequestBehavior.AllowGet);
        }

        // GET: AssLocation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_location ass_location = db.ass_location.Find(id);
            if (ass_location == null)
            {
                return HttpNotFound();
            }
            return View(ass_location);
        }

        // GET: AssLocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssLocation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,descs")] ass_location ass_location)
        {
            if (ModelState.IsValid)
            {
                ass_location.audit_date = DateTime.Now;
                ass_location.user_id = int.Parse(User.Identity.Name);

                db.ass_location.Add(ass_location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ass_location);
        }

        // GET: AssLocation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_location ass_location = db.ass_location.Find(id);
            if (ass_location == null)
            {
                return HttpNotFound();
            }
            return View(ass_location);
        }

        // POST: AssLocation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,descs")] ass_location ass_location)
        {
            if (ModelState.IsValid)
            {
                ass_location.audit_date = DateTime.Now;
                ass_location.user_id = int.Parse(User.Identity.Name);

                db.Entry(ass_location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_location);
        }

        // GET: AssLocation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_location ass_location = db.ass_location.Find(id);
            if (ass_location == null)
            {
                return HttpNotFound();
            }
            return View(ass_location);
        }

        // POST: AssLocation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ass_location ass_location = db.ass_location.Find(id);
            db.ass_location.Remove(ass_location);
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
