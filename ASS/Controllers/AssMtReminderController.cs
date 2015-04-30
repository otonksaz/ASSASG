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

namespace ASS.Controllers
{
    public class AssMtReminderController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        // GET: AssMtReminder
        public ActionResult Index()
        {
            int userId = int.Parse(User.Identity.Name);
            return View(
                db.ass_reminder
                    .Where(a => a.owner_user == null || a.owner_user == userId)
                    .ToList()
                );
        }

        // GET: AssMtReminder/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_reminder ass_reminder = db.ass_reminder.Find(id);
            if (ass_reminder == null)
            {
                return HttpNotFound();
            }
            return View(ass_reminder);
        }

        // GET: AssMtReminder/Create
        public ActionResult Create()
        {
            List<SimpleClass> sc = new List<SimpleClass>();
            sc.Add(new SimpleClass { 
                value = "",
                description = "Normal"
            });
            sc.Add(new SimpleClass
            {
                value = "alert-warning",
                description = "Warning"
            });
            sc.Add(new SimpleClass
            {
                value = "alert-danger",
                description = "Danger"
            });
            ViewBag.clases = new SelectList(sc, "value", "description");
            return View();
        }

        // POST: AssMtReminder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descs,clases,start_day,end_day,isOnlyMe")] ass_reminder ass_reminder)
        {
            if (ModelState.IsValid)
            {
                ass_reminder.audit_date = DateTime.Now;
                ass_reminder.user_id = int.Parse(User.Identity.Name);

                if (ass_reminder.isOnlyMe)
                    ass_reminder.owner_user = int.Parse(User.Identity.Name);
                else
                    ass_reminder.owner_user = null;

                db.ass_reminder.Add(ass_reminder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ass_reminder);
        }

        // GET: AssMtReminder/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_reminder ass_reminder = db.ass_reminder.Find(id);
            if (ass_reminder == null)
            {
                return HttpNotFound();
            }
            ass_reminder.isOnlyMe = ass_reminder.getIsOnlyMe;
            return View(ass_reminder);
        }

        // POST: AssMtReminder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descs,clases,start_day,end_day,isOnlyMe")] ass_reminder ass_reminder)
        {
            if (ModelState.IsValid)
            {
                ass_reminder.audit_date = DateTime.Now;
                ass_reminder.user_id = int.Parse(User.Identity.Name);

                if (ass_reminder.isOnlyMe)
                    ass_reminder.owner_user = int.Parse(User.Identity.Name);
                else
                    ass_reminder.owner_user = null;

                db.Entry(ass_reminder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_reminder);
        }

        // GET: AssMtReminder/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_reminder ass_reminder = db.ass_reminder.Find(id);
            if (ass_reminder == null)
            {
                return HttpNotFound();
            }
            return View(ass_reminder);
        }

        // POST: AssMtReminder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ass_reminder ass_reminder = db.ass_reminder.Find(id);
            db.ass_reminder.Remove(ass_reminder);
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
