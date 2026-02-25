using Kool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kool.Controllers
{
    public class OpetajasController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public OpetajasController()
        {
            db = new ApplicationDbContext();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        }

        // ---------------- INDEX ----------------
        public ActionResult Index()
        {
            return View(db.Opetajad.ToList());
        }

        // ---------------- DETAILS ----------------
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();
            return View(opetaja);
        }

        // ---------------- CREATE (GET) ----------------
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View(new OpetajaCreateVM());
        }

        // ---------------- CREATE (POST) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(OpetajaCreateVM vm, HttpPostedFileBase foto)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // 1. Создаём пользователя
            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email
            };

            var result = userManager.Create(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err);
                return View(vm);
            }

            if (!roleManager.RoleExists("Opetaja"))
                roleManager.Create(new IdentityRole("Opetaja"));

            userManager.AddToRole(user.Id, "Opetaja");

            // 2. Фото
            string fileName = "default.png";
            if (foto != null && foto.ContentLength > 0)
            {
                var ext = Path.GetExtension(foto.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("", "Ainult JPG ja PNG.");
                    return View(vm);
                }

                fileName = Guid.NewGuid() + ext;
                var path = Server.MapPath("~/Content/uploads/opetajad/");
                Directory.CreateDirectory(path);
                foto.SaveAs(Path.Combine(path, fileName));
            }

            // 3. Учитель
            var opetaja = new Opetaja
            {
                Nimi = vm.Nimi,
                Kvalifikatsioon = vm.Kvalifikatsioon,
                FotoPath = fileName,
                ApplicationUserId = user.Id
            };

            db.Opetajad.Add(opetaja);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ---------------- EDIT (GET) ----------------
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            var user = userManager.FindById(opetaja.ApplicationUserId);
            if (user == null) return HttpNotFound();

            return View(new OpetajaEditVM
            {
                Id = opetaja.Id,
                Nimi = opetaja.Nimi,
                Kvalifikatsioon = opetaja.Kvalifikatsioon,
                FotoPath = opetaja.FotoPath,
                Email = user.Email,
                ApplicationUserId = opetaja.ApplicationUserId
            });
        }

        // ---------------- EDIT (POST) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(OpetajaEditVM vm, HttpPostedFileBase foto)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var opetaja = db.Opetajad.Find(vm.Id);
            var user = userManager.FindById(vm.ApplicationUserId);
            if (opetaja == null || user == null) return HttpNotFound();

            opetaja.Nimi = vm.Nimi;
            opetaja.Kvalifikatsioon = vm.Kvalifikatsioon;

            // Фото
            if (foto != null && foto.ContentLength > 0)
            {
                var ext = Path.GetExtension(foto.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("", "Ainult JPG ja PNG.");
                    return View(vm);
                }

                if (!string.IsNullOrEmpty(opetaja.FotoPath) && opetaja.FotoPath != "default.png")
                {
                    var old = Server.MapPath("~/Content/uploads/opetajad/" + opetaja.FotoPath);
                    if (System.IO.File.Exists(old))
                        System.IO.File.Delete(old);
                }

                var newName = Guid.NewGuid() + ext;
                foto.SaveAs(Server.MapPath("~/Content/uploads/opetajad/" + newName));
                opetaja.FotoPath = newName;
            }

            // Email
            if (user.Email != vm.Email)
            {
                user.Email = vm.Email;
                user.UserName = vm.Email;
            }

            // Пароль
            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                var token = userManager.GeneratePasswordResetToken(user.Id);
                userManager.ResetPassword(user.Id, token, vm.NewPassword);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ---------------- DELETE ----------------
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();
            return View(opetaja);
        }

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