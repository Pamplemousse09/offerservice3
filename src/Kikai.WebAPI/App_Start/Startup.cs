using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Kikai.WebApi.Startup))]

namespace Kikai.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}