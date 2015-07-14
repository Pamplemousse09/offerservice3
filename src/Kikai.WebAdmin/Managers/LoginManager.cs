using Kikai.WebAdmin.IManagers;
using System.Web.Mvc;
using System.Web.Security;

namespace Kikai.WebAdmin.Managers
{
    public class LoginManager : ILoginManager
    {
        public void Login(string username, string password, Controller controller)
        {
            if (FormsAuthentication.Authenticate(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                controller.HttpContext.Response.Redirect("../Offer");
            }
            else
            {
                controller.TempData["invalidLogin"] = true;
                controller.Response.Redirect("./");
            }
        }

        public void Logout(Controller controller)
        {
            controller.Session.Clear();
            controller.Session.Abandon();
            controller.Session.RemoveAll();
            FormsAuthentication.SignOut();
        }
    }
}