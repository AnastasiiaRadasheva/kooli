using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Kool.Startup))]
namespace Kool
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
