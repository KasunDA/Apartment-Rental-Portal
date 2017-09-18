using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BootstrapBundleProject.Startup))]
namespace BootstrapBundleProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
