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
    public class AssEmailSenderController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        // GET: AssEmailSender
        public ActionResult Index()
        {
            return View(db.ass_email_sender.ToList());
        }

        // GET: AssEmailSender/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_email_sender ass_email_sender = db.ass_email_sender.Find(id);
            if (ass_email_sender == null)
            {
                return HttpNotFound();
            }
            return View(ass_email_sender);
        }

        // GET: AssEmailSender/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssEmailSender/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ent_cd,email,password,url_logo,ent_descs,ent_address,ttd_name,ttd_title,email_template,tempTemplate")] ass_email_sender ass_email_sender)
        {
            if (ModelState.IsValid)
            {
                ass_email_sender.password = EncrypHelpers.Encrypt(ass_email_sender.password);
                ass_email_sender.email_template = ass_email_sender.tempTemplate;
                db.ass_email_sender.Add(ass_email_sender);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_email_sender);
        }

        // GET: AssEmailSender/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_email_sender ass_email_sender = db.ass_email_sender.Find(id);
            if (ass_email_sender == null)
            {
                return HttpNotFound();
            }
            ass_email_sender.password = EncrypHelpers.Decrypt(ass_email_sender.password);
            ass_email_sender.tempTemplate = ass_email_sender.email_template;
            return View(ass_email_sender);
        }

        // POST: AssEmailSender/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ent_cd,email,password,url_logo,ent_descs,ent_address,ttd_name,ttd_title,email_template,tempTemplate")] ass_email_sender ass_email_sender)
        {
            if (ModelState.IsValid)
            {
                ass_email_sender.password = EncrypHelpers.Encrypt(ass_email_sender.password);
                ass_email_sender.email_template = ass_email_sender.tempTemplate;
                db.Entry(ass_email_sender).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_email_sender);
        }

        // GET: AssEmailSender/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_email_sender ass_email_sender = db.ass_email_sender.Find(id);
            if (ass_email_sender == null)
            {
                return HttpNotFound();
            }
            return View(ass_email_sender);
        }

        // POST: AssEmailSender/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ass_email_sender ass_email_sender = db.ass_email_sender.Find(id);
            db.ass_email_sender.Remove(ass_email_sender);
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
