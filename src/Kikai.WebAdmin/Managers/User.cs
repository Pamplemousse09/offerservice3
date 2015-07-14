using System.Web;
using Kikai.Domain.Common;
using System.Web.Security;
using System;
using Kikai.Internal.Contracts.Objects;
using System.Collections.Generic;

namespace Kikai.WebAdmin.Managers
{
    public static class User
    {
        
        public static bool CanAddSamples()
        {
            return HttpContext.Current.User.IsInRole(UserPermission.OFFER_CAN_ADD_SAMPLES) ||
                HttpContext.Current.User.IsInRole(UserPermission.OFFER_ADMIN);
        }

        public static bool CanEditOffers()
        {
            return HttpContext.Current.User.IsInRole(UserPermission.OFFER_CAN_EDIT_OFFERS) ||
                HttpContext.Current.User.IsInRole(UserPermission.OFFER_ADMIN); 
        }

        public static bool IsLoggedoutFromHummingbird()
        {
            return
            (HttpContext.Current.Session["PHPSESSID"] == null ||
                HttpContext.Current.Session["Permissions"] == null ||
                HttpContext.Current.Request.Cookies["PHPSESSID"] == null ||
                HttpContext.Current.Session["PHPSESSID"].ToString() != HttpContext.Current.Request.Cookies["PHPSESSID"].Value);
            
        }

        public static bool IsAuthenticatedHummingbirdUser()
        {
            return (HttpContext.Current.User.Identity.IsAuthenticated
                && ((FormsIdentity)HttpContext.Current.User.Identity).Ticket.UserData == Constants.HummingbirdUser);
        }


        public static String[] Permissions()
        {
            return HttpContext.Current.Session["Permissions"].ToString().Split(',');
        }

        public static bool CanViewOfferService(List<HBUserRightsItemObject> userRights)
        {
            return (userRights.Exists(x => x.UserRightsItem == UserPermission.OFFER_CAN_SEE_DASHBOARD));
        }

        public static List<string> GetPermissions(List<HBUserRightsItemObject> userRights)
        {
            List<string> permissionList = new List<string>();
            foreach (var permission in typeof(UserPermission).GetFields())
            {
                if (userRights.Exists(x => x.UserRightsItem == permission.Name))
                {
                    permissionList.Add(permission.GetRawConstantValue().ToString());
                }
            }

            return permissionList;
        }
            
    }
}