using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kool.Models;

namespace Kool.Controllers
{
    public class OpetajasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Opetajas  (ВСЕМ можно смотреть)
        public ActionResult Index()
        {
            return View(db.Opetajad.ToList());
        }

        // GET: Opetajas/Details/5  (ВСЕМ можно смотреть)
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            return View(opetaja);
        }

        // GET: Opetajas/Create  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Opetajas/Create  (ТОЛЬКО Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Nimi,Kvalifikatsioon,FotoPath,ApplicationUserId")] Opetaja opetaja)
        {
            if (ModelState.IsValid)
            {
                db.Opetajad.Add(opetaja);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(opetaja);
        }

        // GET: Opetajas/Edit/5  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            return View(opetaja);
        }

        // POST: Opetajas/Edit/5  (ТОЛЬКО Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Nimi,Kvalifikatsioon,FotoPath,ApplicationUserId")] Opetaja opetaja)
        {
            if (ModelState.IsValid)
            {
                db.Entry(opetaja).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(opetaja);
        }

        // GET: Opetajas/Delete/5  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            return View(opetaja);
        }

        // POST: Opetajas/Delete/5  (ТОЛЬКО Admin)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            db.Opetajad.Remove(opetaja);
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
