using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASS.Model;

namespace ASS.Controllers
{
    public class PMTenancyController : Controller
    {
        POM_DasanaEntities db = new POM_DasanaEntities();
        // GET: PMTenancy
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getLot()
        {
            var tenans = db.pm_tenancy
                .Join(db.cf_business, a => a.business_id, b => b.business_id, (a, b) => new { a, b })
                .Select(a => new
            {
                entCd = a.a.entity_cd,
                projectNo = a.a.project_no,
                lotNo = a.a.tenant_no,
                busId = a.a.business_id,
                busName = a.b.name,
                busTelp = a.b.hand_phone,
                bast = a.a.solicitor_ref,
                email = a.b.designation
            });
            var data = new { data = tenans };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getLotDetails(string entCd, string lotNo, string projectNo)
        {
            var tenans = db.pm_tenancy
                .Where(a => a.entity_cd == entCd && a.tenant_no == lotNo && a.project_no == projectNo)
                .Join(db.cf_business, a => a.business_id, b => b.business_id, (a, b) => new { a, b })                
                .Select(a => new
                {
                    entCd = a.a.entity_cd,
                    projectNo = a.a.project_no,
                    lotNo = a.a.tenant_no,
                    busId = a.a.business_id,
                    busName = a.b.name,
                    busTelp = a.b.hand_phone,
                    bast = a.a.solicitor_ref,
                    email = a.b.designation
                });
            return Json(tenans, JsonRequestBehavior.AllowGet);
            //return Json(new { success = 0, comp_id = 1, Exception = "hello" }, JsonRequestBehavior.AllowGet);
        }
    }
}