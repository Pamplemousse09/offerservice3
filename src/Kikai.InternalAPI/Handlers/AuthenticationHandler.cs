using Kikai.Common.Extensions;
using Kikai.Domain.Common;
using Kikai.Internal.Contracts.Objects;
using Kikai.Logging.DTO;
using Kikai.Logging.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LSR.Security;
using Kikai.InternalAPI.Decorators;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Security.Principal;

namespace Kikai.InternalAPI.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        #region Constants

        private readonly string AdminUser = ConfigurationManager.AppSettings["AdminUser"];
        private readonly string AdminPassword = ConfigurationManager.AppSettings["AdminPassword"];
        private readonly string AdminKey = ConfigurationManager.AppSettings["AdminKey"];
        private const string AuthenticationType = "LSR-DIGEST";
        private readonly string OperationType = operationType.InternalAPI.ToString();
        private readonly string OperationName = operationName.Authentication.ToString();

        #endregion

        #region Methods

        protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            ErrorUtil errorUtil = new ErrorUtil();
            LogObject logObject = new LogObject();
            int Error = 0;
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            string _requestId;

            var QueryValues = request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
            QueryValues.TryGetValue("requestId", out _requestId);

            if (String.IsNullOrEmpty(_requestId))
                _requestId = Guid.NewGuid().ToString();

            log.ProcessingDebug(_requestId, "Full request URL:" + request.RequestUri.AbsoluteUri);
            request.Properties.Add("requestId", _requestId);
            AuthenticationResponse _responseEnvelope = new AuthenticationResponse(_requestId);

            if (new CommonMethods().IsArgumentFormat(request.RequestUri.OriginalString))
                request = new CommonMethods().CombobulateArgumentRequest(request);

            if (new CommonMethods().IsLegacyFormat(request.RequestUri.OriginalString))
                request = new CommonMethods().CombobulateRequest(request);

            AuthenticationObject authenticationObject = new CommonMethods().GetAuthenticationHeader(request);

            //If header does not contain the required credentials then add an error to the response envelope else 
            if (authenticationObject == null || authenticationObject.AuthenticationType == null || authenticationObject.ApiUser == null || authenticationObject.SharedSecret == null)
            {
                Error = ErrorKey.ERR_HUB_AUTHENTICATION_FAILED;
                authenticationObject = new AuthenticationObject();
            }

            //Check authentication
            else
            {
                Error = Authenticate(authenticationObject, _requestId);
            }

            if (Error != 0)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("ApiUser", authenticationObject.ApiUser);
                _responseEnvelope.Data.Errors.Add(new ErrorObject(Error, parameters));
                logObject = new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, _responseEnvelope.Data.Errors);
                log.InfoJson(logObject);
                var response = FormatErrorResponse(request, HttpStatusCode.OK, _responseEnvelope);
                tsc.SetResult(response);

                return tsc.Task;
            }
            else
            {
                logObject = new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, _responseEnvelope.Data.Errors);
                log.InfoJson(logObject);
                var Response = base.SendAsync(request, cancellationToken);
                return Response;
            }
        }

        private int Authenticate(AuthenticationObject authenticationObject, string _requestId)
        {
            HourlyDigest hourlyDigest = new HourlyDigest(AdminUser, AdminPassword, AdminKey);
            string AdminSharedSecret = hourlyDigest.CalculateDigest(DateTime.UtcNow);
            if (authenticationObject.ApiUser == AdminUser && authenticationObject.SharedSecret == AdminSharedSecret)
            {
                HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(authenticationObject.ApiUser, authenticationObject.AuthenticationType), new String[] { "Admin" });
                return 0;
            }

            else
                return ErrorKey.ERR_HUB_AUTHENTICATION_FAILED;
        }

        /// <summary>
        /// Function used to detect the desired response format and return the provider error response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="code"></param>
        /// <param name="_responseEnvelope"></param>
        /// <returns></returns>
        public HttpResponseMessage FormatErrorResponse(HttpRequestMessage request, HttpStatusCode code, AuthenticationResponse _responseEnvelope)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            MediaTypeWithQualityHeaderValue responseType = null;

            //Check the requested response format
            if (request.GetRequestContext() != null && request.GetRequestContext().Url != null && request.GetRequestContext().Url.Request != null && request.GetRequestContext().Url.Request.Headers != null)
                responseType = request.GetRequestContext().Url.Request.Headers.Accept.LastOrDefault();

            if (responseType != null && responseType.ToString().ToLower().Equals("application/json"))
                response.Content = new ObjectContent<AuthenticationResponse>(_responseEnvelope, new JsonMediaTypeFormatter());
            else
                response.Content = new ObjectContent<AuthenticationResponse>(_responseEnvelope, new XmlMediaTypeFormatter());

            return response;
        }

        #endregion
    }
}