using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IP3Latest.Startup))]
namespace IP3Latest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
