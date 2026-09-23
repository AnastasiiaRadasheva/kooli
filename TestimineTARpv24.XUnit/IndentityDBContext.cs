using Kool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;


namespace TestimineTARpv24.XUnit
{
    public class ApplicationUser : IdentityUser
    {
        public async Task GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    public class AdaptedDbContext : IdentityDbContext<ApplicationUser>
    {
        public AdaptedDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static AdaptedDbContext Create()
        {
            return new AdaptedDbContext();
        }
    }

    
        public DbSet<Keelekursus> Keelekursused { get; set; }
        public DbSet<Opetaja> Opetajad { get; set; }
        public DbSet<Koolitus> Koolitused { get; set; }
        public DbSet<Registreerimine> Registreerimised { get; set; }
    }
