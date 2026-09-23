using Kool.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Kool.Models;
using Kool; 

namespace TestimineTARpv24.XUnit
{
    public abstract class TestBase
    {
        protected IServiceProvider serviceProvider { get; set; }

        protected TestBase()
        {
            var services = new ServiceCollection();
            SetupServices(services);

            serviceProvider = services.BuildServiceProvider();
        }

        protected T Svc<T>()
        {
            return serviceProvider.GetService<T>();
        }
        //seame üles testide läbiviimiseks vajalikud kontrollerid mujalt projektist
        //see meetod annab ka mäluspleva andmebaasi misa testideks kasutada 
        //Viper tüüpi projektides toimib kui "program.cs" analoog, end lühidat kujul
        //param name sevises kollektor kuhu aseme kontroll´ri instantsid
        public virtual void SetupServices(IServiceCollection services)
        {
            services.AddScoped<KoolitusController>();
            services.AddScoped<IHostEnvironment, MockIHostEnvironment>();

            services.AddDbContext<TestimineTARpv24Context>(x =>
            {
                x.UseInMemoryDatabase("TEST");
                x.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            RegisterMacros(services);
        }

      //Leia üles kindel teenus teenusepakkujakt
      //serviceProvider omab kontrollireti instantse ning GetService hangib selle X tüüpi kontrolleri
        private static void RegisterMacros(IServiceCollection services)
        {
         
            var macroBaseType = typeof(IMacros);

            var macros = macroBaseType
                .Assembly
                .GetTypes()
                .Where(t =>
                    macroBaseType.IsAssignableFrom(t) &&
                    !t.IsInterface &&
                    !t.IsAbstract);

            foreach (var macro in macros)
            {
                services.AddSingleton(macro);
            }
        }
    }
}
