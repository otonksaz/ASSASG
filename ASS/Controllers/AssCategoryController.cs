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
    public class AssCategoryController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        // GET: AssCategory
        public ActionResult Index()
        {
            var ass_category = db.ass_category.Include(a => a.parent);
            return View(ass_category.ToList());
        }       

        public JsonResult GetCategory()
        {
            var categories = db.ass_category.Select(a => new
            {
                a.id,
                a.name,
                a.descs,
            });
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkDurationById(int? id)
        {
            var result = db.ass_category
                .Find(id);
            return Json(new { success = 1, data = result.work_duration, ex = "" }, JsonRequestBehavior.AllowGet);
        }

        // GET: AssCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_category ass_category = db.ass_category.Find(id);
            if (ass_category == null)
            {
                return HttpNotFound();
            }
            return View(ass_category);
        }

        // GET: AssCategory/Create
        public ActionResult Create()
        {
            ViewBag.parent_id = new SelectList(db.ass_category, "id", "name");
            return View();
        }

        // POST: AssCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,parent_id,name,descs,work_duration")] ass_category ass_category)
        {
            if (ModelState.IsValid)
            {
                ass_category.audit_date = DateTime.Now;
                ass_category.user_id = int.Parse(User.Identity.Name);

                db.ass_category.Add(ass_category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.parent_id = new SelectList(db.ass_category, "id", "name", ass_category.parent_id);
            return View(ass_category);
        }

        // GET: AssCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_category ass_category = db.ass_category.Find(id);
            if (ass_category == null)
            {
                return HttpNotFound();
            }
            ViewBag.parent_id = new SelectList(db.ass_category, "id", "name", ass_category.parent_id);
            return View(ass_category);
        }

        // POST: AssCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,parent_id,name,descs,work_duration")] ass_category ass_category)
        {
            if (ModelState.IsValid)
            {
                ass_category.audit_date = DateTime.Now;
                ass_category.user_id = int.Parse(User.Identity.Name);

                db.Entry(ass_category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.parent_id = new SelectList(db.ass_category, "id", "name", ass_category.parent_id);
            return View(ass_category);
        }

        // GET: AssCategory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_category ass_category = db.ass_category.Find(id);
            if (ass_category == null)
            {
                return HttpNotFound();
            }
            return View(ass_category);
        }

        // POST: AssCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ass_category ass_category = db.ass_category.Find(id);
            db.ass_category.Remove(ass_category);
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
