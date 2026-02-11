using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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

            userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(db)
            );

            roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(db)
            );
        }

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
            return View(new OpetajaCreateVM());
        }

        // POST: Opetajas/Create  (ТОЛЬКО Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(OpetajaCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // 1) создаём пользователя Identity
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

            // 2) выдаём роль Opetaja
            if (!roleManager.RoleExists("Opetaja"))
                roleManager.Create(new IdentityRole("Opetaja"));

            userManager.AddToRole(user.Id, "Opetaja");

            // 3) создаём профиль Opetaja и связываем с ApplicationUser
            var opetaja = new Opetaja
            {
                Nimi = vm.Nimi,
                Kvalifikatsioon = vm.Kvalifikatsioon,
                FotoPath = vm.FotoPath,
                ApplicationUserId = user.Id
            };

            db.Opetajad.Add(opetaja);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Opetajas/Edit/5  (ТОЛЬКО Admin)
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var opetaja = db.Opetajad.Find(id);
            if (opetaja == null) return HttpNotFound();

            var user = userManager.FindById(opetaja.ApplicationUserId);
            if (user == null) return HttpNotFound();

            var vm = new OpetajaEditVM
            {
                Id = opetaja.Id,
                Nimi = opetaja.Nimi,
                Kvalifikatsioon = opetaja.Kvalifikatsioon,
                FotoPath = opetaja.FotoPath,
                Email = user.Email,
                ApplicationUserId = opetaja.ApplicationUserId
            };

            return View(vm);
        }

        // POST: Opetajas/Edit/5  (ТОЛЬКО Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(OpetajaEditVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var opetaja = db.Opetajad.Find(vm.Id);
            if (opetaja == null) return HttpNotFound();

            var user = userManager.FindById(vm.ApplicationUserId);
            if (user == null) return HttpNotFound();

            // 1) обновляем данные учителя
            opetaja.Nimi = vm.Nimi;
            opetaja.Kvalifikatsioon = vm.Kvalifikatsioon;
            opetaja.FotoPath = vm.FotoPath;

            // 2) обновляем email
            if (user.Email != vm.Email)
            {
                // (простая проверка, чтобы не было дубля)
                var exists = db.Users.Any(u => u.Email == vm.Email && u.Id != user.Id);
                if (exists)
                {
                    ModelState.AddModelError("Email", "See email on juba kasutusel.");
                    return View(vm);
                }

                user.Email = vm.Email;
                user.UserName = vm.Email;
            }

            // 3) обновляем пароль (если ввели новый)
            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                var token = userManager.GeneratePasswordResetToken(user.Id);
                var passResult = userManager.ResetPassword(user.Id, token, vm.NewPassword);

                if (!passResult.Succeeded)
                {
                    foreach (var err in passResult.Errors)
                        ModelState.AddModelError("", err);

                    return View(vm);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
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

            // Если хочешь удалять и пользователя — раскомментируй:
            // if (!string.IsNullOrEmpty(opetaja.ApplicationUserId))
            // {
            //     var user = userManager.FindById(opetaja.ApplicationUserId);
            //     if (user != null) userManager.Delete(user);
            // }

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
