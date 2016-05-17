using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MoneyTree.Startup))]
namespace MoneyTree
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
