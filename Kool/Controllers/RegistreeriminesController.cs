using Kool.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Kool.Controllers
{
    public class RegistreeriminesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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

        [Authorize(Roles = "Opilane")]
        public ActionResult Create(int? koolitusId)
        {
            if (koolitusId == null)
                return RedirectToAction("Index", "Koolitus");

            var koolitus = db.Koolitused
                .Include(k => k.Registreerimised)
                .FirstOrDefault(k => k.Id == koolitusId.Value);

            if (koolitus == null) return HttpNotFound();

            int approved = koolitus.Registreerimised.Count(r => r.Staatus == RegistreerimineStaatus.Approved);
            if (approved >= koolitus.MaxOsalejaid)
            {
                TempData["msg"] = "GRUPP TÄIS. Registreerimine pole võimalik.";
                return RedirectToAction("Index", "Koolitus");
            }

            return View(new Registreerimine { KoolitusId = koolitusId.Value });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Opilane")]
        public ActionResult Create(int koolitusId)
        {
            var koolitus = db.Koolitused
                .Include(k => k.Registreerimised)
                .FirstOrDefault(k => k.Id == koolitusId);

            if (koolitus == null)
                return HttpNotFound();

            string userId = User.Identity.GetUserId();

            bool already = db.Registreerimised
                .Any(r => r.KoolitusId == koolitusId && r.ApplicationUserId == userId);

            if (already)
            {
                TempData["Msg"] = "Sa oled juba selle kursuse jaoks registreerinud.";
                return RedirectToAction("Details", "Koolitus", new { id = koolitusId });
            }

            int approved = db.Registreerimised
                .Count(r => r.KoolitusId == koolitusId &&
                            r.Staatus == RegistreerimineStaatus.Approved);

            if (approved >= koolitus.MaxOsalejaid)
            {
                TempData["Msg"] = "GRUPP TÄIS. Registreerimine pole võimalik.";
                return RedirectToAction("Details", "Koolitus", new { id = koolitusId });
            }

            var reg = new Registreerimine
            {
                KoolitusId = koolitusId,
                ApplicationUserId = userId,
                Staatus = RegistreerimineStaatus.Pending
            };

            db.Registreerimised.Add(reg);
            db.SaveChanges();

            TempData["Msg"] = "Registreerimine saadetud. Ootab kinnitamist.";

            return RedirectToAction("MinuKoolitused", "Koolitus");
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Opetaja")]

        public ActionResult Approve(int id)
        {
            var reg = db.Registreerimised
                .Include(r => r.Koolitus)
                .Include(r => r.Koolitus.Keelekursus)
                .Include(r => r.ApplicationUser)
                .FirstOrDefault(r => r.Id == id);

            if (reg == null) return HttpNotFound();

            if (reg.Staatus == RegistreerimineStaatus.Pending)
            {
                reg.Staatus = RegistreerimineStaatus.Approved;
                db.SaveChanges();

                try
                {
                    WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "eha20082@gmail.com";
                    WebMail.Password = "iakc rgui tmxd erwf";
                    WebMail.From = "eha20082@gmail.com";

                    string sisu = $@"
                <h2>Tere, {reg.ApplicationUser.UserName}!</h2>
                <p>Teid on vastu võetud kursusele: <b>{reg.Koolitus.Keelekursus.Nimetus}</b>.</p>
                <p>Kursus algab: {reg.Koolitus.AlgusKuupaev.ToShortDateString()}</p>
                <br/>
                <p>Parimate soovidega, Kooli administratsioon</p>";

                    WebMail.Send(
                        to: reg.ApplicationUser.Email,
                        subject: "Kinnitus: " + reg.Koolitus.Keelekursus.Nimetus,
                        body: sisu,
                        isBodyHtml: true
                    );

                    TempData["msg"] = "Kasutaja on kinnitatud ja e-kiri saadetud!";
                }
                catch (System.Exception ex)
                {
                    TempData["msg"] = "Kinnitatud, kuid e-kirja viga: " + ex.Message;
                }
            }

            return RedirectToAction("Registrations", "Koolitus", new { id = reg.KoolitusId });
        }

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

            return RedirectToAction("Registrations", "Koolitus", new { id = reg.KoolitusId });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
