using Kikai.Logging.DTO;
using Kikai.Internal.Contracts.Objects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Kikai.Common.Extensions.IExtensions;
using System.Web;

namespace Kikai.Common.Extensions
{
    public class CommonMethods : ICommonMethods
    {
        /// <summary>
        /// Converts input variables into log object
        /// </summary>
        /// <param name="TimeStamp"></param>
        /// <param name="RequestId"></param>
        /// <param name="User"></param>
        /// <param name="Type"></param>
        /// <param name="Name"></param>
        /// <param name="Parameters"></param>
        /// <param name="WebServiceResponse"></param>
        /// <returns></returns>
        public LogObject Authentication_ToLogObject(string RequestId, string User, string Type, string Name, AuthenticationObject Parameters, List<ErrorObject> WebServiceResponse = null)
        {
            LogObject logObject = new LogObject();
            Dictionary<string, string> response = new Dictionary<string, string>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            logObject.TimeStamp = DateTime.Now;
            logObject.RequestId = RequestId;
            logObject.User = User;
            logObject.Type = Type;
            logObject.Name = Name;
            parameters.Add("Authentication Type", Parameters.AuthenticationType);
            parameters.Add("ApiUser", Parameters.ApiUser);
            parameters.Add("SharedSecret", Parameters.SharedSecret);
            logObject.Parameters = parameters;
            if (WebServiceResponse.Count == 0)
            {
                response.Add("REQUEST_SUCCESSFUL", "The request was successful.");
            }
            else
            {
                foreach (var error in WebServiceResponse)
                {
                    response.Add(error.ErrorKey, error.Message);
                }
            }
            logObject.Response = response;
            return logObject;
        }

        /// Function that gets the authentication type, apiuser and sharedSecret from the authentication header and saves them in an AuthenticationObject
        /// </summary>
        /// <param name="message"></param>
        /// <returns>AuthenticationObject</returns>
        public AuthenticationObject GetAuthenticationHeader(HttpRequestMessage request)
        {
            string authenticationType = "LSR-DIGEST";
            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme.Equals(authenticationType, StringComparison.OrdinalIgnoreCase))
            {
                return new AuthenticationObject(request.Headers.Authorization.Scheme, request.Headers.Authorization.Parameter);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Function that puts the authentication parameters from the request url in the request authentication header
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpRequestMessage CombobulateRequest(HttpRequestMessage request)
        {
            NameValueCollection requestParams = request.RequestUri.ParseQueryString();

            // Derive Authorization header from parameter
            if (request.Headers.Authorization == null)
            {
                string authHeader = String.Format("ApiUser={0}, SharedSecret={1}", requestParams["username"], requestParams["sharedSecret"]);
                request.Headers.Authorization = new AuthenticationHeaderValue("LSR-DIGEST", authHeader);
            }

            // Build new uri without method and data array
            StringBuilder newPath = new StringBuilder(request.RequestUri.Scheme + "://");
            newPath.Append(request.RequestUri.Authority);
            newPath.Append(request.RequestUri.AbsolutePath);
            newPath.Append("/" + requestParams["method"] + "/");

            //// Find the data[], aka the function parameters
            NameValueCollection functionParams = new NameValueCollection();
            Regex r = new Regex(@"data\[(?<parm>\w+)\]", RegexOptions.IgnoreCase);
            foreach (String s in requestParams.AllKeys)
            {
                Match m = r.Match(s);
                if (m.Success)
                    functionParams.Add(r.Match(s).Result("${parm}"), requestParams[s]);
            }

            if (functionParams.HasKeys())
            {
                newPath.Append("?");
                foreach (String s in functionParams.AllKeys)
                {
                    newPath.Append("&" + s + "=" + functionParams[s]);
                }
            }

            request.RequestUri = new Uri(newPath.ToString());

            return request;
        }

        /// <summary>
        /// Function that puts the authentication parameters from the request url in the request authentication header and parses the arguments as parameters for the controller
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpRequestMessage CombobulateArgumentRequest(HttpRequestMessage request)
        {
            NameValueCollection requestParams = request.RequestUri.ParseQueryString();

            // Derive Authorization header from parameter
            if (request.Headers.Authorization == null)
            {
                string test = requestParams["arg0"];
                string test2 = requestParams["arg1"];
                string authHeader = String.Format("ApiUser={0}, SharedSecret={1}", requestParams["arg0"], requestParams["arg1"]);
                request.Headers.Authorization = new AuthenticationHeaderValue("LSR-DIGEST", authHeader);
            }

            // Build new uri without method and data array
            StringBuilder newPath = new StringBuilder(request.RequestUri.Scheme + "://");
            newPath.Append(request.RequestUri.Authority);
            newPath.Append(request.RequestUri.AbsolutePath);
            newPath.Append("/" + requestParams["method"] + "/");

            //// Find the data[], aka the function parameters
            NameValueCollection functionParams = new NameValueCollection();
            Regex r = new Regex(@"arg[0-9]+\[(?<parm>\w+)\]", RegexOptions.IgnoreCase);
            foreach (String s in requestParams.AllKeys)
            {
                Match m = r.Match(s);
                if (m.Success)
                    functionParams.Add(r.Match(s).Result("${parm}"), requestParams[s]);
            }

            if (functionParams.HasKeys())
            {
                newPath.Append("?");
                foreach (String s in functionParams.AllKeys)
                {
                    newPath.Append("&" + s + "=" + functionParams[s]);
                }
            }

            request.RequestUri = new Uri(newPath.ToString());

            return request;
        }

        /// <summary>
        /// Function that will check if the authentication parameters are passed in the URL
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool IsLegacyFormat(string uri)
        {
            Regex r = new Regex(@"method|username|sharedSecret", RegexOptions.IgnoreCase);
            return r.Match(uri).Success;
        }

        /// <summary>
        /// Function that will check if the authentication parameters are passed as arguments in the URL
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool IsArgumentFormat(string uri)
        {
            Regex r = new Regex(@"arg0|arg1|arg2", RegexOptions.IgnoreCase);
            return r.Match(uri).Success;
        }

        //Remove data[] from input strings
        private string cleanInputString(string wsParameter, HttpContext context)
        {
            string cleanString = context.Request.Params[wsParameter];

            if (String.IsNullOrEmpty(cleanString))
            {
                cleanString = context.Request.Params["data[" + wsParameter + "]"];
            }
            return cleanString;
        }
    }
}
