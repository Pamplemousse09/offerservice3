using System.Text.RegularExpressions;

namespace Kikai.Internal.Contracts.Objects
{
    public class AuthenticationObject
    {
        public string AuthenticationType { get; private set; }
        public string ApiUser { get; private set; }
        public string SharedSecret { get; private set; }

        /// <summary>
        /// Default constructor with empty values
        /// </summary>
        public AuthenticationObject()
        {
            this.AuthenticationType = string.Empty;
            this.ApiUser = string.Empty;
            this.SharedSecret = string.Empty;
        }

        /// <summary>
        /// Contructor that takes the authentication type and parameters and saves them in an Authentication Object
        /// </summary>
        /// <param name="AuthenticationType"></param>
        /// <param name="AuthenticationParameters"></param>
        public AuthenticationObject(string AuthenticationType, string AuthenticationParameters)
        {
            this.AuthenticationType = AuthenticationType;
            Regex r = new Regex(@"ApiUser=(?<user>\w+), SharedSecret=(?<secret>\w+)", RegexOptions.IgnoreCase);
            Match m = r.Match(AuthenticationParameters);
            if (m.Success)
            {
                this.ApiUser = r.Match(AuthenticationParameters).Result("${user}");
                this.SharedSecret = r.Match(AuthenticationParameters).Result("${secret}");
            }
        }
    }
}
