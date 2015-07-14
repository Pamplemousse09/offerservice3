using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Kikai.WebAdmin.Startup))]

namespace Kikai.WebAdmin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}