using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.IManagers;
using Kikai.Internal.Utils;
using log4net;
using LSR.WebClient;
using LSR.WebClient.Configuration;
using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Kikai.Internal.Managers
{
    public class HummingbirdUser : IHummingbirdUser
    {
        readonly static WebClientUser UserInformation = (WebClientUser)ConfigurationManager.GetSection("WebClientUser");

        public HummingbirdUser()
        {
        }

        /// <summary>
        /// Creates a HummingbirdUserClient that will call the HummingbirdUser service
        /// </summary>
        /// <returns></returns>
        internal static LSR.WebClient.HummingbirdUser.RpcClient GetHummingbirdUserClient()
        {
            var p = new ConfigurationProvider(UserInformation);
            var client = new LSR.WebClient.HummingbirdUser.RpcClient(p);
            return client;
        }

        /// <summary>
        /// Call the Hummingbird User webservice
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>XDocument webservice response</returns>
        private XDocument CallHummingbirdUserService(string method, Hashtable parameters)
        {
            XDocument result = null;
            var hummingbirdUserClient = GetHummingbirdUserClient();

            string service = "GetHummingbirdUserClient";
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                result = new XmlUtil().ParseXmlDocument(hummingbirdUserClient.CallWebservice(method, parameters));
                LogUtil.CallSuccess(service, result.ToString());
            }
            catch (Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }

            return result;
        }

        /// Get the Hummingbird user permissions
        /// </summary>
        /// <param name="userId"></param>
        public HBUserPermissionObject GetUserPermissions(int userId)
        {
            var parameters = new Hashtable { 
            { "data[user_id]", userId }, 
            { "data[module_name]", ConfigurationManager.AppSettings["HummingbirdModuleName"] }, 
            { "data[module_code]", ConfigurationManager.AppSettings["HummingbirdModuleCode"] }, 
            { "data[action]", ConfigurationManager.AppSettings["HummingbirdAction"] } 
            };   
         
            HBUserPermissionObject hbUserPermissionObject = new HBUserPermissionObject();

            string service = "GetHummingbirdUserPermission";
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                var result = CallHummingbirdUserService("getUserPermissions", parameters);
                LogUtil.CallSuccess(service, result.ToString());
                hbUserPermissionObject = ProcessUserPermissionsResponse(result);
            }
            catch (Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }
            return hbUserPermissionObject;
        }

        /// <summary>
        /// Get the HB user rights and html head and footer 
        /// </summary>
        /// <param name="result"></param>
        /// <returns>HBUserPermissionsObject</returns>
        private HBUserPermissionObject ProcessUserPermissionsResponse(XDocument result)
        {
            HBUserPermissionObject hbUserPermissionObject = new HBUserPermissionObject();            

            try
            {

                hbUserPermissionObject = (from userPermissions in result.Descendants("Data")
                                           select new HBUserPermissionObject()
                                           {
                                               HtmlHead = XmlUtil.GetSafeStringNodeValue(userPermissions.Element("HtmlHead")),
                                               Header = XmlUtil.GetSafeStringNodeValue(userPermissions.Element("Header")),
                                               Footer = XmlUtil.GetSafeStringNodeValue(userPermissions.Element("Footer")),
                                               UserRightsList = (from permission in userPermissions.Descendants("Item")
                                                                select new HBUserRightsItemObject()
                                                                 {
                                                                     UserRightsItem = XmlUtil.GetSafeStringNodeValue(permission)
                                                                 }).ToList()


                                           }).FirstOrDefault();

                if (hbUserPermissionObject == null)
                    return new HBUserPermissionObject();

            }
            catch (Exception)
            {
                throw;
            }

            return hbUserPermissionObject;
        }
    }
}
