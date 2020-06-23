using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(urban_archive.Startup))]
namespace urban_archive
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
