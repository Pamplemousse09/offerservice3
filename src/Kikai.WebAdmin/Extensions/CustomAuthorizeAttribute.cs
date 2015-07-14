using System;
using System.Web;
using System.Web.Mvc;
using Kikai.WebAdmin.Managers;

namespace Kikai.WebAdmin.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {



        public CustomAuthorizeAttribute(params string[] roles)
            : base()
        {
            Roles = string.Join(",", roles);
        }

        private bool _isAuthorized;

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            _isAuthorized = base.AuthorizeCore(httpContext);
            return _isAuthorized;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.Request.Cookies["PHPSESSID"] != null)
                {
                    filterContext.Result = (new RedirectController()).RedirectToHBError();
                }
                else
                {
                    filterContext.Result = (new RedirectController()).RedirectToError();
                }
            }

            if (User.IsAuthenticatedHummingbirdUser() && User.IsLoggedoutFromHummingbird())
            {
                new HBUserManager().ResetUserState();
                filterContext.Result = (new RedirectController()).RedirectToHBError();
            }


        }

        private class RedirectController : Controller
        {
            public ActionResult RedirectToError()
            {
                return RedirectToAction("Error", "Home");
            }

            public ActionResult RedirectToHBError()
            {
                return RedirectToAction("HBError", "Home");
            }
        }
    }
}