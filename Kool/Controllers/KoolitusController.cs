using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kool.Models;
using Microsoft.AspNet.Identity;

namespace Kool.Controllers
{
    public class KoolitusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Koolitus
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Koolitused.ToList());
        }

        // GET: Koolitus/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null)
            {
                return HttpNotFound();
            }
            return View(koolitus);
        }

        // GET: Koolitus/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [AllowAnonymous] // чтобы могли смотреть все
        public ActionResult ByKeelekursus(int id)
        {
            var list = db.Koolitused
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .Where(k => k.KeelekursusId == id)
                .ToList();

            return View("IndexK", list); // или "Index", если хочешь использовать Index.cshtml
        }

        // POST: Koolitus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,KeelekursusId,OpetajaId,AlgusKuupaev,LoppKuupaev,Hind,MaxOsalejaid")] Koolitus koolitus)
        {
            if (ModelState.IsValid)
            {
                db.Koolitused.Add(koolitus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(koolitus);
        }

        // GET: Koolitus/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null)
            {
                return HttpNotFound();
            }
            return View(koolitus);
        }

        // POST: Koolitus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,KeelekursusId,OpetajaId,AlgusKuupaev,LoppKuupaev,Hind,MaxOsalejaid")] Koolitus koolitus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(koolitus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(koolitus);
        }

        // GET: Koolitus/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null)
            {
                return HttpNotFound();
            }
            return View(koolitus);
        }
        [Authorize(Roles = "Opetaja")]
        public ActionResult MinuKoolitused()
        {
            string userId = User.Identity.GetUserId();

            var minu = db.Koolitused
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .Where(k => k.Opetaja.ApplicationUserId == userId)
                .OrderByDescending(k => k.AlgusKuupaev)
                .ToList();

            return View(minu);
        }

        // POST: Koolitus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var koolitus = db.Koolitused.Find(id);
            if (koolitus == null) return HttpNotFound();

            db.Koolitused.Remove(koolitus);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public ActionResult IndexK()
        {
            var list = db.Koolitused
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .ToList();

            return View("indexK", list); 
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
