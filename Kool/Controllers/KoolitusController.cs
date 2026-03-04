using Kool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Kool.Controllers
{
    public class KoolitusController : Controller
    {

        private void LoadCounts()
        {
            ViewBag.PendingCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Pending)
                .GroupBy(r => r.KoolitusId)
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.ApprovedCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Approved)
                .GroupBy(r => r.KoolitusId)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "Admin")]
        public ActionResult ByOpetaja(int id)
        {
            var list = db.Koolitused
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .Where(k => k.OpetajaId == id)
                .ToList();

            ViewBag.PendingCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Pending)
                .GroupBy(r => r.KoolitusId)
                .ToDictionary(g => g.Key, g => g.Count());

            LoadCounts();
            return View("IndexK", list); 
        }

        [AllowAnonymous]
        public ActionResult IndexK(string keel, string nimi)
        {
            var andmed = db.Koolitused
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keel))
                andmed = andmed.Where(x => x.Keelekursus.Keel.Contains(keel));

            if (!string.IsNullOrEmpty(nimi))
                andmed = andmed.Where(x => x.Opetaja.Nimi.Contains(nimi));

            ViewBag.PendingCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Pending)
                .GroupBy(r => r.KoolitusId)
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.ApprovedCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Approved)
                .GroupBy(r => r.KoolitusId)
                .ToDictionary(g => g.Key, g => g.Count());

            return View(andmed.ToList());
        }

        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null) return HttpNotFound();

            return View(koolitus);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Registrations(int id) 
        {
            var regs = db.Registreerimised
                .Include(r => r.ApplicationUser)
                .Where(r => r.KoolitusId == id && r.Staatus == RegistreerimineStaatus.Pending)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.KoolitusId = id;
            return View(regs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Register(int koolitusId)
        {
            string userId = User.Identity.GetUserId();

            bool exists = db.Registreerimised.Any(r =>
                r.KoolitusId == koolitusId && r.ApplicationUserId == userId);

            if (!exists)
            {
                db.Registreerimised.Add(new Registreerimine
                {
                    KoolitusId = koolitusId,
                    ApplicationUserId = userId,
                    Staatus = RegistreerimineStaatus.Pending
                });

                db.SaveChanges();
                TempData["Msg"] = "Taotlus saadetud. Oota Admin kinnitust.";
            }
            else
            {
                TempData["Msg"] = "Sul on juba olemas taotlus (või oled juba registreeritud).";
            }

            return RedirectToAction("Details", new { id = koolitusId });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.OpetajaId = new SelectList(
                db.Opetajad,
                "Id",
                "Nimi"
            );

            ViewBag.KeelekursusId = new SelectList(
                db.Keelekursused,
                "Id",
                "Nimetus"
            );

            return View();
        }

        [AllowAnonymous]
        public ActionResult ByKeelekursus(int id)
        {
            var list = db.Koolitused
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .Where(k => k.KeelekursusId == id)
                .ToList();

            var pendingCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Pending)
                .GroupBy(r => r.KoolitusId)
                .Select(g => new { KoolitusId = g.Key, Cnt = g.Count() })
                .ToList()
                .ToDictionary(x => x.KoolitusId, x => x.Cnt);

            ViewBag.PendingCounts = pendingCounts;
            LoadCounts();
            return View("IndexK", list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,KeelekursusId,OpetajaId,AlgusKuupaev,LoppKuupaev,Hind,MaxOsalejaid")] Koolitus koolitus)
        {
            if (ModelState.IsValid)
            {
                db.Koolitused.Add(koolitus);
                db.SaveChanges();
                return RedirectToAction("IndexK");
            }

            ViewBag.OpetajaId = new SelectList(db.Opetajad, "Id", "Nimi", koolitus.OpetajaId);
            ViewBag.KeelekursusId = new SelectList(db.Keelekursused, "Id", "Nimetus", koolitus.KeelekursusId);

            return View(koolitus);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null) return HttpNotFound();

            return View(koolitus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,KeelekursusId,OpetajaId,AlgusKuupaev,LoppKuupaev,Hind,MaxOsalejaid")] Koolitus koolitus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(koolitus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexK");
            }
            return View(koolitus);
        }

        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult Osalejad(int id)
        {
            string userId = User.Identity.GetUserId();

            var koolitus = db.Koolitused
                .Include(k => k.Opetaja)
                .FirstOrDefault(k => k.Id == id);

            if (koolitus == null)
                return HttpNotFound();

            if (User.IsInRole("Opetaja"))
            {
                if (koolitus.Opetaja == null || koolitus.Opetaja.ApplicationUserId != userId)
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            IQueryable<Registreerimine> query = db.Registreerimised
                .Include(r => r.ApplicationUser)
                .Where(r => r.KoolitusId == id);

            if (User.IsInRole("Opetaja"))
            {
                query = query.Where(r => r.Staatus == RegistreerimineStaatus.Approved);
            }


            var osalejad = query
                .OrderBy(r => r.ApplicationUser.UserName)
                .ToList();

            ViewBag.KoolitusId = id;

            return View(osalejad);
        }


        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult WriteToStudent(string email)
        {
            ViewBag.Email = email;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult SendPersonalEmail(string email, string subject, string message)
        {
            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "eha20082@gmail.com";
                WebMail.Password = "iakc rgui tmxd erwf";

                WebMail.Send(
                    to: email,
                    subject: subject,
                    body: message
                );

                TempData["Msg"] = "Kiri saadetud kasutajale " + email;
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Viga: " + ex.Message;
            }

            return RedirectToAction("IndexK");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null) return HttpNotFound();

            return View(koolitus);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var koolitus = db.Koolitused.Find(id);
            if (koolitus == null) return HttpNotFound();

            db.Koolitused.Remove(koolitus);
            db.SaveChanges();
            return RedirectToAction("IndexK");
        }

        [Authorize(Roles = "Admin,Opetaja,Opilane")]
        public ActionResult MinuKoolitused()
        {
            string userId = User.Identity.GetUserId();

            if (User.IsInRole("Admin"))
            {
                var all = db.Koolitused
                    .Include(k => k.Keelekursus)
                    .Include(k => k.Opetaja)
                    .OrderByDescending(k => k.AlgusKuupaev)
                    .ToList();

                var approvedCounts = db.Registreerimised
                    .Where(r => r.Staatus == RegistreerimineStaatus.Approved)
                    .GroupBy(r => r.KoolitusId)
                    .ToDictionary(g => g.Key, g => g.Count());

                ViewBag.ApprovedCounts = approvedCounts;

                return View(all);
            }

            if (User.IsInRole("Opetaja"))
            {
                var minu = db.Koolitused
                    .Include(k => k.Keelekursus)
                    .Include(k => k.Opetaja)
                    .Where(k => k.Opetaja.ApplicationUserId == userId)
                    .OrderByDescending(k => k.AlgusKuupaev)
                    .ToList();

                var approvedCounts = db.Registreerimised
                    .Where(r => r.Staatus == RegistreerimineStaatus.Approved)
                    .GroupBy(r => r.KoolitusId)
                    .ToDictionary(g => g.Key, g => g.Count());

                ViewBag.ApprovedCounts = approvedCounts;

                return View(minu);
            }

            var koolitusIds = db.Registreerimised
                .Where(r => r.ApplicationUserId == userId)
                .Select(r => r.KoolitusId)
                .Distinct()
                .ToList();

            var listOpilane = db.Koolitused
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .Where(k => koolitusIds.Contains(k.Id))
                .OrderByDescending(k => k.AlgusKuupaev)
                .ToList();

            var statuses = db.Registreerimised
                .Where(r => r.ApplicationUserId == userId)
                .ToList()
                .GroupBy(r => r.KoolitusId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        if (g.Any(x => x.Staatus == RegistreerimineStaatus.Approved))
                            return RegistreerimineStaatus.Approved;
                        if (g.Any(x => x.Staatus == RegistreerimineStaatus.Pending))
                            return RegistreerimineStaatus.Pending;
                        return RegistreerimineStaatus.Rejected;
                    }
                );

            ViewBag.MyStatuses = statuses;

            return View(listOpilane);
        }
        [HttpGet]
        public ActionResult RemoveParticipant()
        {
            return Content("GET METHOD");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult RemoveParticipant(int id)
        {

            var reg = db.Registreerimised.Find(id);

            if (reg == null)
                return HttpNotFound();

            int koolitusId = reg.KoolitusId;

            db.Registreerimised.Remove(reg);
            db.SaveChanges();

            TempData["Msg"] = "Osaleja eemaldatud kursuselt.";

            return RedirectToAction("Osalejad", new { id = koolitusId });
        }

        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult WriteToGroup(int id)
        {
            ViewBag.KoolitusId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult SendGroupEmail(int koolitusId, string subject, string message)
        {
            var emails = db.Registreerimised
                .Where(r => r.KoolitusId == koolitusId && r.Staatus == RegistreerimineStaatus.Approved)
                .Select(r => r.ApplicationUser.Email)
                .ToList();

            try
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "eha20082@gmail.com";
                WebMail.Password = "iakc rgui tmxd erwf";

                foreach (var email in emails)
                {
                    WebMail.Send(
                        to: email,
                        subject: subject,
                        body: message
                    );
                }

                TempData["Msg"] = "Kiri saadetud kogu grupile!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Viga: " + ex.Message;
            }

            return RedirectToAction("Osalejad", new { id = koolitusId });
        }
        [AllowAnonymous]
        public ActionResult ByOpetaja1(int id)
        {
            var list = db.Koolitused
        .Include(k => k.Keelekursus)
        .Include(k => k.Opetaja)
        .Where(k => k.OpetajaId == id)
        .ToList();
            LoadCounts();
            return View("IndexK", list);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
