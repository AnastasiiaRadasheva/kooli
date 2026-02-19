using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kool.Models;
using Microsoft.AspNet.Identity;

namespace Kool.Controllers
{
    public class KoolitusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // -----------------------------
        // ADMIN: список курсов (Index)
        // -----------------------------
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var list = db.Koolitused.ToList();

            // pending заявки по каждому курсу
            var pendingCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Pending)
                .GroupBy(r => r.KoolitusId)
                .Select(g => new { KoolitusId = g.Key, Cnt = g.Count() })
                .ToList()
                .ToDictionary(x => x.KoolitusId, x => x.Cnt);

            ViewBag.PendingCounts = pendingCounts;

            return View(list);
        }

        // --------------------------------------
        // PUBLIC: список курсов (IndexK) - у тебя
        // --------------------------------------
        [AllowAnonymous]
        public ActionResult IndexK()
        {
            var list = db.Koolitused
                .Include(k => k.Keelekursus)
                .Include(k => k.Opetaja)
                .ToList();

            // pending заявки по каждому курсу (нужно для "Заявки: X" у Admin)
            var pendingCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Pending)
                .GroupBy(r => r.KoolitusId)
                .Select(g => new { KoolitusId = g.Key, Cnt = g.Count() })
                .ToList()
                .ToDictionary(x => x.KoolitusId, x => x.Cnt);

            ViewBag.PendingCounts = pendingCounts;

            return View("IndexK", list);
        }

        // GET: Koolitus/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null) return HttpNotFound();

            return View(koolitus);
        }

        // ---------------------------------------
        // ADMIN: список желающих на курс (Pending)
        // ---------------------------------------
        [Authorize(Roles = "Admin")]
        public ActionResult Registrations(int id) // id = KoolitusId
        {
            var regs = db.Registreerimised
                .Include(r => r.ApplicationUser)
                .Where(r => r.KoolitusId == id && r.Staatus == RegistreerimineStaatus.Pending)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.KoolitusId = id;
            return View(regs); // Views/Koolitus/Registrations.cshtml
        }

        // ---------------------------------------
        // USER: отправить заявку на курс (Pending)
        // ---------------------------------------
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

        // GET: Koolitus/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
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

            // чтобы "Заявки: X" тоже работало, если Admin зайдет сюда
            var pendingCounts = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Pending)
                .GroupBy(r => r.KoolitusId)
                .Select(g => new { KoolitusId = g.Key, Cnt = g.Count() })
                .ToList()
                .ToDictionary(x => x.KoolitusId, x => x.Cnt);

            ViewBag.PendingCounts = pendingCounts;

            return View("IndexK", list);
        }

        // POST: Koolitus/Create
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
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null) return HttpNotFound();

            return View(koolitus);
        }

        // POST: Koolitus/Edit/5
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

        [Authorize(Roles = "Opetaja")]
        public ActionResult Osalejad(int id) // id = KoolitusId
        {
            string userId = User.Identity.GetUserId();

            // Проверяем, что этот курс реально принадлежит этому учителю
            var koolitus = db.Koolitused
                .Include(k => k.Opetaja)
                .FirstOrDefault(k => k.Id == id);

            if (koolitus == null) return HttpNotFound();

            if (koolitus.Opetaja == null || koolitus.Opetaja.ApplicationUserId != userId)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // Берем только Approved участников
            var osalejad = db.Registreerimised
                .Include(r => r.ApplicationUser)
                .Where(r => r.KoolitusId == id && r.Staatus == RegistreerimineStaatus.Approved)
                .OrderBy(r => r.ApplicationUser.UserName)
                .ToList();

            ViewBag.KoolitusId = id;
            return View(osalejad);
        }

        // POST: Koolitus/ApproveRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult ApproveRegistration(int id) // id = Registreerimine.Id
        {
            var reg = db.Registreerimised.FirstOrDefault(r => r.Id == id);
            if (reg == null) return HttpNotFound();

            reg.Staatus = RegistreerimineStaatus.Approved;
            db.SaveChanges();

            TempData["Msg"] = "Approved!";
            return RedirectToAction("Registrations", new { id = reg.KoolitusId });
        }

        // POST: Koolitus/RejectRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult RejectRegistration(int id) // id = Registreerimine.Id
        {
            var reg = db.Registreerimised.FirstOrDefault(r => r.Id == id);
            if (reg == null) return HttpNotFound();

            reg.Staatus = RegistreerimineStaatus.Rejected;
            db.SaveChanges();

            TempData["Msg"] = "Rejected!";
            return RedirectToAction("Registrations", new { id = reg.KoolitusId });
        }


        // GET: Koolitus/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Koolitus koolitus = db.Koolitused.Find(id);
            if (koolitus == null) return HttpNotFound();

            return View(koolitus);
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

        [Authorize(Roles = "Opetaja")]
        [Authorize(Roles = "Opetaja,Opilane")]
        public ActionResult MinuKoolitused()
        {
            string userId = User.Identity.GetUserId();

            // --- Õpetaja: показываем курсы учителя ---
            if (User.IsInRole("Opetaja"))
            {
                var minu = db.Koolitused
                    .Include(k => k.Keelekursus)
                    .Include(k => k.Opetaja)
                    .Where(k => k.Opetaja.ApplicationUserId == userId)
                    .OrderByDescending(k => k.AlgusKuupaev)
                    .ToList();

                // (не обязательно, но удобно) сколько Approved участников на каждый курс
                var approvedCounts = db.Registreerimised
                    .Where(r => r.Staatus == RegistreerimineStaatus.Approved)
                    .GroupBy(r => r.KoolitusId)
                    .Select(g => new { KoolitusId = g.Key, Cnt = g.Count() })
                    .ToList()
                    .ToDictionary(x => x.KoolitusId, x => x.Cnt);

                ViewBag.ApprovedCounts = approvedCounts;

                return View(minu);
            }

            // --- Õpilane: показываем курсы, куда ученик регался ---
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

            // статус по каждому курсу (если вдруг несколько записей — берем самый важный)
            // Priority: Approved > Pending > Rejected
            var statuses = db.Registreerimised
                .Where(r => r.ApplicationUserId == userId)
                .ToList()
                .GroupBy(r => r.KoolitusId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        if (g.Any(x => x.Staatus == RegistreerimineStaatus.Approved)) return RegistreerimineStaatus.Approved;
                        if (g.Any(x => x.Staatus == RegistreerimineStaatus.Pending)) return RegistreerimineStaatus.Pending;
                        return RegistreerimineStaatus.Rejected;
                    }
                );

            ViewBag.MyStatuses = statuses;

            return View(listOpilane);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
