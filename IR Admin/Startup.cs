using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IR_Admin.Startup))]
namespace IR_Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
