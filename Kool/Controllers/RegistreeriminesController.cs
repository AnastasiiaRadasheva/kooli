using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kool.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
namespace Kool.Controllers
{
    public class RegistreeriminesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Registreerimines
        // Можно оставить для Admin, чтобы видеть все регистрации
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Registreerimised.ToList());
        }

        // GET: Registreerimines/Details/5
        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var registreerimine = db.Registreerimised.Find(id);
            if (registreerimine == null) return HttpNotFound();

            return View(registreerimine);
        }

        // GET: Registreerimines/Create?koolitusId=5
        // Õpilane нажимает "Registreeru"
        [Authorize(Roles = "Opilane")]

        [Authorize(Roles = "Admin")]
        
        [Authorize(Roles = "Opetaja")]
        public ActionResult Create(int? koolitusId)
        {
            if (koolitusId == null)
                return RedirectToAction("Index", "Koolitus");

            var koolitus = db.Koolitused.Find(koolitusId.Value);
            if (koolitus == null)
                return HttpNotFound();

            return View(new Registreerimine { KoolitusId = koolitusId.Value });
        }

        [Authorize(Roles = "Opilane")]
        public ActionResult MinuKoolitused()
        {
            string userId = User.Identity.GetUserId();

            var minu = db.Registreerimised
                .Where(r => r.ApplicationUserId == userId)
                .Include(r => r.Koolitus)
                .Include(r => r.Koolitus.Keelekursus)
                .Include(r => r.Koolitus.Opetaja)
                .OrderByDescending(r => r.Id)
                .ToList();

            return View(minu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Opilane")]
        public ActionResult Create(int koolitusId)
        {
            var koolitus = db.Koolitused.Find(koolitusId);
            if (koolitus == null) return HttpNotFound();

            int count = db.Registreerimised.Count(r => r.KoolitusId == koolitusId);
            if (count >= koolitus.MaxOsalejaid)
            {
                ModelState.AddModelError("", "GRUPP TÄIS. Registreerimine pole võimalik.");
                return View(new Registreerimine { KoolitusId = koolitusId });
            }

            string userId = User.Identity.GetUserId();

            var reg = new Registreerimine
            {
                KoolitusId = koolitusId,
                ApplicationUserId = userId,
                Staatus = RegistreerimineStaatus.Pending
            };

            db.Registreerimised.Add(reg);
            db.SaveChanges();

            return RedirectToAction("MinuKoolitused");
        }


        // GET: Registreerimines/Edit/5
        // редактирование статуса - только Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var registreerimine = db.Registreerimised.Find(id);
            if (registreerimine == null) return HttpNotFound();

            return View(registreerimine);
        }

        // POST: Registreerimines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,KoolitusId,ApplicationUserId,Staatus")] Registreerimine registreerimine)
        {
            if (!ModelState.IsValid) return View(registreerimine);

            db.Entry(registreerimine).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Registreerimines/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var registreerimine = db.Registreerimised.Find(id);
            if (registreerimine == null) return HttpNotFound();

            return View(registreerimine);
        }

        // POST: Registreerimines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var registreerimine = db.Registreerimised.Find(id);
            if (registreerimine == null) return HttpNotFound();

            db.Registreerimised.Remove(registreerimine);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
