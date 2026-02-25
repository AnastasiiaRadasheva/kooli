namespace Kool.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Kool.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<Kool.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Kool.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            string[] rollid = { "Admin", "Opetaja", "Opilane" };
            foreach (var roll in rollid)
            {
                if (!context.Roles.Any(r => r.Name == roll))
                {
                    roleManager.Create(new IdentityRole(roll));
                }
            }

            var adminEmail = "admin@kool.ee";
            var admin = context.Users.FirstOrDefault(u => u.Email == adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                userManager.Create(admin, "Admin123!"); 
            }

            if (!userManager.IsInRole(admin.Id, "Admin"))
                userManager.AddToRole(admin.Id, "Admin");

            var opetajaEmail = "opetaja@kool.ee";
            var opetajaUser = context.Users.FirstOrDefault(u => u.Email == opetajaEmail);

            if (opetajaUser == null)
            {
                opetajaUser = new ApplicationUser { UserName = opetajaEmail, Email = opetajaEmail };
                userManager.Create(opetajaUser, "Opetaja123!");
            }

            if (!userManager.IsInRole(opetajaUser.Id, "Opetaja"))
                userManager.AddToRole(opetajaUser.Id, "Opetaja");
            var opetajaProfiil = context.Opetajad.FirstOrDefault(o => o.ApplicationUserId == opetajaUser.Id);
            if (opetajaProfiil == null)
            {
                context.Opetajad.Add(new Opetaja
                {
                    Nimi = "Mari Maasikas",
                    Kvalifikatsioon = "CELTA, 5 aastat kogemust",
                    FotoPath = "mari.jpg",
                    ApplicationUserId = opetajaUser.Id
                });
                context.SaveChanges();
            }

            var opilaneEmail = "opilane@kool.ee";
            var opilaneUser = context.Users.FirstOrDefault(u => u.Email == opilaneEmail);

            if (opilaneUser == null)
            {
                opilaneUser = new ApplicationUser { UserName = opilaneEmail, Email = opilaneEmail };
                userManager.Create(opilaneUser, "opilane@kool.eeopilane@kool.ee");
            }

            if (!userManager.IsInRole(opilaneUser.Id, "Opilane"))
                userManager.AddToRole(opilaneUser.Id, "Opilane");

            if (!context.Keelekursused.Any(k => k.Nimetus == "Saksa keel algajatele"))
            {
                context.Keelekursused.Add(new Keelekursus
                {
                    Nimetus = "Saksa keel algajatele",
                    Keel = "Saksa",
                    Tase = "A1",
                    Kirjeldus = "Algajatele mõeldud kursus."
                });
                context.SaveChanges();
            }

            var kursus = context.Keelekursused.First(k => k.Nimetus == "Saksa keel algajatele");
            var opetaja = context.Opetajad.First(o => o.ApplicationUserId == opetajaUser.Id);

            if (!context.Koolitused.Any(x => x.KeelekursusId == kursus.Id && x.OpetajaId == opetaja.Id))
            {
                context.Koolitused.Add(new Koolitus
                {
                    KeelekursusId = kursus.Id,
                    OpetajaId = opetaja.Id,
                    AlgusKuupaev = System.DateTime.Today.AddDays(7),
                    LoppKuupaev = System.DateTime.Today.AddDays(37),
                    Hind = 199,
                    MaxOsalejaid = 10
                });
                context.SaveChanges();
            }

        }
    }
    }
