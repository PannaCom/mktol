using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MarketingOnline.Startup))]
namespace MarketingOnline
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
