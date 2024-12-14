using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IR_Admin.Db;
using IR_Admin.Models;

namespace IR_Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountryBasicProjectDatasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CountryBasicProjectDatas
        public ActionResult Index()
        {
            return View(db.CountryBasicProjectDatas.ToList());
        }

        // GET: CountryBasicProjectDatas/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CountryBasicProjectData countryBasicProjectData = db.CountryBasicProjectDatas.Find(id);
            if (countryBasicProjectData == null)
            {
                return HttpNotFound();
            }
            return View(countryBasicProjectData);
        }

        // GET: CountryBasicProjectDatas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CountryBasicProjectDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,Roll,EmailAddress,Country,ProjectLocation,TypeOfProject,SeasonalProjectType,Name,IsOneDonar,IRWProjectPinCode,Donar_Partner,Duration,ImplementationPeriodFrom,ImplementationPeriodTo,Region,CreateDate")] CountryBasicProjectData countryBasicProjectData)
        {
            if (ModelState.IsValid)
            {
                countryBasicProjectData.Id = Guid.NewGuid();
                db.CountryBasicProjectDatas.Add(countryBasicProjectData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(countryBasicProjectData);
        }

        // GET: CountryBasicProjectDatas/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CountryBasicProjectData countryBasicProjectData = db.CountryBasicProjectDatas.Find(id);
            if (countryBasicProjectData == null)
            {
                return HttpNotFound();
            }
            return View(countryBasicProjectData);
        }

        // POST: CountryBasicProjectDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Roll,EmailAddress,Country,ProjectLocation,TypeOfProject,SeasonalProjectType,Name,IsOneDonar,IRWProjectPinCode,Donar_Partner,Duration,ImplementationPeriodFrom,ImplementationPeriodTo,Region,CreateDate")] CountryBasicProjectData countryBasicProjectData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(countryBasicProjectData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(countryBasicProjectData);
        }

        // GET: CountryBasicProjectDatas/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CountryBasicProjectData countryBasicProjectData = db.CountryBasicProjectDatas.Find(id);
            if (countryBasicProjectData == null)
            {
                return HttpNotFound();
            }
            return View(countryBasicProjectData);
        }

        // POST: CountryBasicProjectDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            CountryBasicProjectData countryBasicProjectData = db.CountryBasicProjectDatas.Find(id);
            db.CountryBasicProjectDatas.Remove(countryBasicProjectData);
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
