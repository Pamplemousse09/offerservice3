using System.Web.Mvc;

namespace Kikai.WebAdmin.IManagers
{
    public interface ILoginManager
    {
        void Login(string username, string password, Controller controller);

        void Logout(Controller controller);
    }
}
