using Kikai.Logging.Utils;
using Kikai.Internal.IManagers;
using log4net;
using LSR.Security;
using System.Configuration;

namespace Kikai.Internal.Managers
{
    public class Pmp : IPmp
    {

        public Pmp()
        {
        }

        /// <summary>
        /// Creates a PmpClient that will call the Pmp service
        /// </summary>
        /// <returns></returns>
        internal static SecurityClient GetPmpClient()
        {
            SecurityClient client = new SecurityClient();
            CredentialConfiguration wscredentials = (CredentialConfiguration)ConfigurationManager.GetSection("WsAuth");
            client.SetCredentialStoreFromConfig(wscredentials);
            return client;
        }

        /// <summary>
        /// Calls Pmp and checks if provided user and sharedSecret are authenticated
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sharedSecret"></param>
        /// <returns>Boolean</returns>
        public bool Authenticate(string user, string sharedSecret)
        {
            LoggerFactory.GetLogger().Debug("Checking authentication for User: " + user + ", with SharedSecret: " + sharedSecret);
            return GetPmpClient().Authenticate(user, sharedSecret);
        }

        /// <summary>
        /// Calls Pmp and check if provided user is authorized
        /// </summary>
        /// <param name="user"></param>
        /// <param name="resource"></param>
        /// <returns>Boolean</returns>
        public bool Authorize(string user, string resource)
        {
            LoggerFactory.GetLogger().Debug("Checking authorization for User: " + user + ", with Resource: " + resource);
            return GetPmpClient().CheckAuthorization(user, resource);
        }
    }
}
