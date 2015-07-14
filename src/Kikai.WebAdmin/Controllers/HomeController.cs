using Kikai.Logging.Utils;
using Kikai.WebAdmin.Common;
using Kikai.WebAdmin.Managers;
using Kikai.WebAdmin.Utils;
using System;
using System.Web.Mvc;

namespace Kikai.WebAdmin.Controllers
{
    public class HomeController : Controller
    {
        private MessagesUtil msgUtil;

        public HomeController()
        {
            msgUtil = new MessagesUtil();
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Offer Service";
            return View();
        }

        public ActionResult Error()
        {
            ViewBag.Title = "Error";
            return View();
        }

        public ActionResult HBError()
        {
            ViewBag.Title = "HBError";
            return View();
        }

        public void HBRequest()
        {
            try
            {
                new HBUserManager().ValidateRequest();
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(msgUtil.GetMessage(MessageKey.LOG_HBREQUEST_EXCEPTION), e);
            }
        }

        /// <summary>
        /// Login function
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="?"></param>
        public void Authenticate(string username, string password){
            try
            {
                new LoginManager().Login(username, password, this);
            }
            catch(Exception e)
            {
                LoggerFactory.GetLogger().Error(msgUtil.GetMessage(MessageKey.LOG_AUTHENTICATE_EXCEPTION), e);
            }
        }

        /// <summary>
        /// Logout function
        /// </summary>
        public ActionResult Logout(){
            new LoginManager().Logout(this);
            return RedirectToAction("Index");
        }
    }
}
