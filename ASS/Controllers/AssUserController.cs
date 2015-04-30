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
using System.Web.Security;

namespace ASS.Controllers
{
    public class AssUserController : Controller
    {
        private POM_DasanaEntities db = new POM_DasanaEntities();

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel userLogin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string encPass = EncrypHelpers.Encrypt(userLogin.password);
                    ass_user user = db.ass_user
                        .Where(a => a.username.Equals(userLogin.username) && a.password.Equals(encPass))
                        .FirstOrDefault();
                    if (user != null)
                    {
                        Session["userid"] = user.id;
                        Session["username"] = user.username;
                        Session["fullname"] = user.fullname;
                        FormsAuthentication.SetAuthCookie(user.id.ToString(), false);
                        return RedirectToAction("Index", "AssReminder");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login data is incorrect!");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.ToString());
                }
            }
            //return RedirectToAction("AfterLogin");
            return View(userLogin);
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "AssUser");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword cp)
        {
            cp.msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    int userId = int.Parse(User.Identity.Name);
                    string encPass = EncrypHelpers.Encrypt(cp.OldPassword);
                    ass_user user = db.ass_user
                        .Where(a => a.id == userId && a.password.Equals(encPass))
                        .FirstOrDefault();
                    if (user != null)
                    {
                        user.password = EncrypHelpers.Encrypt(cp.NewPassword);
                        db.SaveChanges();
                        cp.msg = "Password Has Been Changed.";
                    }
                    else
                    {
                        ModelState.AddModelError("", "OldPassword is incorrect!");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.ToString());
                }
            }
            //return RedirectToAction("AfterLogin");
            return View(cp);
        }

        [HttpPost]
        public JsonResult ResetPassword(LoginModel dataChange)
        {
            try
            {
                if (Session["username"] == null || Session["username"].ToString().ToLower() != "admin")
                {
                    return Json(new { success = 0, username = dataChange.username, ex = "You don't have a privillages to reset password." });
                }

                ass_user user = db.ass_user
                        .Where(a => a.username == dataChange.username)
                        .FirstOrDefault();
                if (user == null) 
                    return Json(new { success = 0, username = dataChange.username, ex = "User Not Found." });

                user.password = EncrypHelpers.Encrypt("123456");
                db.SaveChanges();
                return Json(new { success = 1, username = dataChange.username, ex = "Password has been reset." });
            }
            catch (Exception ex)
            {
                return Json(new { success = 0, username = dataChange.username, ex = ex.ToString() });
            }
        }


        // GET: AssUser        
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["username"].ToString().ToLower() != "admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View(db.ass_user.ToList());
        }

        // GET: AssUser/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_user ass_user = db.ass_user.Find(id);
            if (ass_user == null)
            {
                return HttpNotFound();
            }
            return View(ass_user);
        }

        // GET: AssUser/Create
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["username"].ToString().ToLower() != "admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View();
        }

        // POST: AssUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,username,fullname")] ass_user ass_user)
        {
            if (Session["username"] == null || Session["username"].ToString().ToLower() != "admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (ModelState.IsValid)
            {
                ass_user.password = EncrypHelpers.Encrypt("123456");
                ass_user.audit_date = DateTime.Now;
                ass_user.user_id = int.Parse(User.Identity.Name);

                db.ass_user.Add(ass_user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ass_user);
        }

        // GET: AssUser/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["username"] == null || Session["username"].ToString().ToLower() != "admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_user ass_user = db.ass_user.Find(id);
            if (ass_user == null)
            {
                return HttpNotFound();
            }
            return View(ass_user);
        }

        // POST: AssUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,username,fullname")] ass_user ass_user)
        {
            if (Session["username"] == null || Session["username"].ToString().ToLower() != "admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (ModelState.IsValid)
            {
                ass_user.audit_date = DateTime.Now;
                ass_user.user_id = int.Parse(User.Identity.Name);

                db.Entry(ass_user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ass_user);
        }

        // GET: AssUser/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["username"] == null || Session["username"].ToString().ToLower() != "admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ass_user ass_user = db.ass_user.Find(id);
            if (ass_user == null)
            {
                return HttpNotFound();
            }
            return View(ass_user);
        }

        // POST: AssUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["username"] == null || Session["username"].ToString().ToLower() != "admin")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            ass_user ass_user = db.ass_user.Find(id);
            db.ass_user.Remove(ass_user);
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
