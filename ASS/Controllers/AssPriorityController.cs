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
    public class AssPriorityController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        // GET: AssPriority
        public ActionResult Index()
        {
            var ass_priority = db.ass_priority.Include(a => a.ass_user);
            return View(ass_priority.ToList());
        }

        // GET: AssPriority/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_priority ass_priority = db.ass_priority.Find(id);
            if (ass_priority == null)
            {
                return HttpNotFound();
            }
            return View(ass_priority);
        }

        // GET: AssPriority/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.ass_user, "id", "username");
            return View();
        }

        // POST: AssPriority/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,descs,user_id,audit_date")] ass_priority ass_priority)
        {
            if (ModelState.IsValid)
            {
                ass_priority.user_id = int.Parse(User.Identity.Name);
                ass_priority.audit_date = DateTime.Now;
                db.ass_priority.Add(ass_priority);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_priority);
        }

        // GET: AssPriority/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_priority ass_priority = db.ass_priority.Find(id);
            if (ass_priority == null)
            {
                return HttpNotFound();
            }
            return View(ass_priority);
        }

        // POST: AssPriority/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,descs,user_id,audit_date")] ass_priority ass_priority)
        {
            if (ModelState.IsValid)
            {
                ass_priority.user_id = int.Parse(User.Identity.Name);
                ass_priority.audit_date = DateTime.Now;
                db.Entry(ass_priority).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_priority);
        }

        // GET: AssPriority/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_priority ass_priority = db.ass_priority.Find(id);
            if (ass_priority == null)
            {
                return HttpNotFound();
            }
            return View(ass_priority);
        }

        // POST: AssPriority/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ass_priority ass_priority = db.ass_priority.Find(id);
            db.ass_priority.Remove(ass_priority);
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
