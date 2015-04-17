using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QueueBackgroundWorkItemTest.Startup))]
namespace QueueBackgroundWorkItemTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
