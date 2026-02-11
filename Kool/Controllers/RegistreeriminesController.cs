using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kool.Models;
using Microsoft.AspNet.Identity;

namespace Kool.Controllers
{
    public class RegistreeriminesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Admin: все заявки
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var all = db.Registreerimised
                .Include(r => r.Koolitus)
                .Include(r => r.Koolitus.Keelekursus)
                .Include(r => r.ApplicationUser)
                .OrderByDescending(r => r.Id)
                .ToList();

            return View(all);
        }

        // Admin/Opetaja: детали заявки
        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var reg = db.Registreerimised
                .Include(r => r.Koolitus)
                .Include(r => r.Koolitus.Keelekursus)
                .Include(r => r.ApplicationUser)
                .FirstOrDefault(r => r.Id == id.Value);

            if (reg == null) return HttpNotFound();

            return View(reg);
        }

        // Opilane: форма подтверждения "хочу записаться"
        [Authorize(Roles = "Opilane")]
        public ActionResult Create(int? koolitusId)
        {
            if (koolitusId == null)
                return RedirectToAction("Index", "Koolitus");

            var koolitus = db.Koolitused
                .Include(k => k.Registreerimised)
                .FirstOrDefault(k => k.Id == koolitusId.Value);

            if (koolitus == null) return HttpNotFound();

            // только проверка мест по APPROVED
            int approved = koolitus.Registreerimised.Count(r => r.Staatus == RegistreerimineStaatus.Approved);
            if (approved >= koolitus.MaxOsalejaid)
            {
                TempData["msg"] = "GRUPP TÄIS. Registreerimine pole võimalik.";
                return RedirectToAction("Index", "Koolitus");
            }

            return View(new Registreerimine { KoolitusId = koolitusId.Value });
        }

        // Opilane: мои заявки/курсы
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

        // Opilane: отправить заявку (Pending)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Opilane")]
        public ActionResult Create(int koolitusId)
        {
            var koolitus = db.Koolitused
                .Include(k => k.Registreerimised)
                .FirstOrDefault(k => k.Id == koolitusId);

            if (koolitus == null) return HttpNotFound();

            string userId = User.Identity.GetUserId();

            // 1) Запрет на повторную запись
            bool already = db.Registreerimised.Any(r => r.KoolitusId == koolitusId && r.ApplicationUserId == userId);
            if (already)
            {
                ModelState.AddModelError("", "Sa oled juba selle kursuse jaoks registreerinud.");
                return View(new Registreerimine { KoolitusId = koolitusId });
            }

            // 2) Проверка мест ТОЛЬКО по Approved
            int approved = koolitus.Registreerimised.Count(r => r.Staatus == RegistreerimineStaatus.Approved);
            if (approved >= koolitus.MaxOsalejaid)
            {
                ModelState.AddModelError("", "GRUPP TÄIS. Registreerimine pole võimalik.");
                return View(new Registreerimine { KoolitusId = koolitusId });
            }

            var reg = new Registreerimine
            {
                KoolitusId = koolitusId,
                ApplicationUserId = userId,
                Staatus = RegistreerimineStaatus.Pending
            };

            db.Registreerimised.Add(reg);

            // если есть уникальный индекс — SaveChanges может бросить исключение на дубль
            db.SaveChanges();

            TempData["msg"] = "Registreerimine saadetud. Ootab kinnitamist.";
            return RedirectToAction("MinuKoolitused");
        }

        // Admin/Opetaja: список Pending (все)
        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult Pending()
        {
            var pending = db.Registreerimised
                .Where(r => r.Staatus == RegistreerimineStaatus.Pending)
                .Include(r => r.Koolitus)
                .Include(r => r.Koolitus.Keelekursus)
                .Include(r => r.ApplicationUser)
                .OrderByDescending(r => r.Id)
                .ToList();

            return View(pending);
        }

        // Admin/Opetaja: подтвердить (Approved) — место займется только сейчас
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult Approve(int id)
        {
            var reg = db.Registreerimised
                .Include(r => r.Koolitus)
                .Include(r => r.Koolitus.Registreerimised)
                .FirstOrDefault(r => r.Id == id);

            if (reg == null) return HttpNotFound();

            // если уже обработано — просто назад
            if (reg.Staatus != RegistreerimineStaatus.Pending)
                return RedirectToAction("Pending");

            // проверить места по Approved
            int approved = reg.Koolitus.Registreerimised.Count(r => r.Staatus == RegistreerimineStaatus.Approved);
            if (approved >= reg.Koolitus.MaxOsalejaid)
            {
                TempData["msg"] = "Ei saa kinnitada: grupp on täis.";
                return RedirectToAction("Pending");
            }

            reg.Staatus = RegistreerimineStaatus.Approved;
            db.SaveChanges();

            return RedirectToAction("Pending");
        }

        // Admin/Opetaja: отклонить
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Opetaja")]
        public ActionResult Reject(int id)
        {
            var reg = db.Registreerimised.Find(id);
            if (reg == null) return HttpNotFound();

            if (reg.Staatus == RegistreerimineStaatus.Pending)
            {
                reg.Staatus = RegistreerimineStaatus.Rejected;
                db.SaveChanges();
            }

            return RedirectToAction("Pending");
        }

        // Admin: Edit (оставим, но лучше пользоваться Approve/Reject)
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var reg = db.Registreerimised.Find(id);
            if (reg == null) return HttpNotFound();

            return View(reg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,KoolitusId,ApplicationUserId,Staatus")] Registreerimine reg)
        {
            if (!ModelState.IsValid) return View(reg);

            db.Entry(reg).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Admin: Delete
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var reg = db.Registreerimised.Find(id);
            if (reg == null) return HttpNotFound();

            return View(reg);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var reg = db.Registreerimised.Find(id);
            if (reg == null) return HttpNotFound();

            db.Registreerimised.Remove(reg);
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
