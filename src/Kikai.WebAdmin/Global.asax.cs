using Kikai.Domain.Common;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using OSUser = Kikai.WebAdmin.Managers.User;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace Kikai.WebAdmin
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            log4net.Config.XmlConfigurator.Configure();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        public void Application_AuthenticateRequest(Object src, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.AuthenticationType == "Forms")
                {
                    System.Web.Security.FormsIdentity id;
                    id = (System.Web.Security.FormsIdentity)HttpContext.Current.User.Identity;
                    String[] myRoles = new String[1];
                    myRoles[0] = UserPermission.OFFER_ADMIN;
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(id, myRoles);
                }
            }
        }

        protected void Application_AcquireRequestState()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated && 
                ((FormsIdentity)HttpContext.Current.User.Identity).Ticket.UserData == Constants.HummingbirdUser)
            {
                if (Context.Session != null && HttpContext.Current.Session["Permissions"] != null)
                {
                        System.Web.Security.FormsIdentity id;
                        id = (System.Web.Security.FormsIdentity)HttpContext.Current.User.Identity;
                        String[] myRoles;
                        myRoles = OSUser.Permissions();
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(id, myRoles);
                }
            }
        }

        void Application_End(object sender, EventArgs e)
        {
        }
    }
}
