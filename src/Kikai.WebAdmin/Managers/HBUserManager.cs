using System;
using System.Web;
using Kikai.WebAdmin.IManagers;
using Kikai.Internal.Managers;
using Kikai.Internal.Contracts.Objects;
using Kikai.Domain.Common;
using System.Web.Security;
using System.Configuration;
using Kikai.Logging.Utils;
using LSR.Security.Utils;
using System.Collections.Generic;

namespace Kikai.WebAdmin.Managers
{
    public class HBUserManager : IHBUserManager
    {
        private readonly string hashSecret = ConfigurationManager.AppSettings["HummingbirdHashSecret"];
        private const string requestUserIdParam = "user_id";
        private const string requestColorScheme = "color_scheme";
        private const string dateFormat = "yyyy-MM-dd HH";

        public void ValidateRequest()
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            string userIdParam, userId, hash, newHash, timeStamp;
            userIdParam = HttpContext.Current.Request.QueryString[requestUserIdParam];
            log.Debug("HBUserManager.ValidateRequest.userIdParam=" + userIdParam);

            if (!(string.IsNullOrEmpty(userIdParam)) && (userIdParam.Length > 32))
            {
                timeStamp = DateTime.UtcNow.ToString(dateFormat);
                log.Debug("HBUserManager.ValidateRequest.timeStamp=" + timeStamp);
                hash = userIdParam.Substring(0, 32);
                log.Debug("HBUserManager.ValidateRequest.SentHash=" + hash);
                userId = userIdParam.Substring(32, userIdParam.Length - hash.Length);
                log.Debug("HBUserManager.ValidateRequest.userId=" + userId);
                //create new hash to compare it with the HB hash
                newHash = new CryptographyUtil().CalculateMD5(userId + ":" + hashSecret + ":" + timeStamp);
                //newHash = new HourlyDigest().CalculateMD5("");
                log.Debug("HBUserManager.ValidateRequest.newHash=" + newHash);
                if (newHash == hash)
                {
                    log.Debug("HBUserManager.ValidateRequest.newHashEqualsOldHash=true");
                    Authenticate(userId);
                }
                else
                {
                    log.Debug("HBUserManager.ValidateRequest.newHashEqualsOldHash=false");
                }
            }

            HttpContext.Current.Response.Redirect("../Offer");
        }

        private bool Authenticate(string userId)
        {
            HBUserPermissionObject hbUserPermissionObject = new HummingbirdUser().GetUserPermissions(Convert.ToInt32(userId));

            if (User.CanViewOfferService(hbUserPermissionObject.UserRightsList))
            {
                CreateUserAuthenticationTicket(userId);
                SetUserSessionData(hbUserPermissionObject, HttpContext.Current.Request.Cookies["PHPSESSID"], HttpContext.Current.Request.QueryString[requestColorScheme]);

                return true;
            }
            else if (User.IsAuthenticatedHummingbirdUser())
            {
                ResetUserState();
            }

            return false;
        }

        private void SetUserSessionData(HBUserPermissionObject hbUserPermissionObject, HttpCookie phpCookie, string colorScheme)
        {
            HttpContext.Current.Session.Add("HTMLHead", hbUserPermissionObject.HtmlHead);
            HttpContext.Current.Session.Add("Header", hbUserPermissionObject.Header);
            HttpContext.Current.Session.Add("Footer", hbUserPermissionObject.Footer);
            HttpContext.Current.Session.Add("ColorScheme", colorScheme);

            string Permissions = String.Join(",", User.GetPermissions(hbUserPermissionObject.UserRightsList).ToArray());
            HttpContext.Current.Session.Add("Permissions", Permissions);
                        
            if (phpCookie != null)
            {
                HttpContext.Current.Session.Add("PHPSESSID", phpCookie.Value);
            }        
        }

        private void CreateUserAuthenticationTicket(string userId)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                    userId,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                    false,
                    Constants.HummingbirdUser,
                    FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // Create the cookie.
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

        }
        
        public void ResetUserState()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.RemoveAll();
            FormsAuthentication.SignOut();
        }

    }
}
