using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASS.Startup))]
namespace ASS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
