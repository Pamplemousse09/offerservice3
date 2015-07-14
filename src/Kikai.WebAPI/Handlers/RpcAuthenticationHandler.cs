using Kikai.Domain.Common;
using Kikai.Logging.DTO;
using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.Managers;
using Kikai.WebApi.Decorators;
using Kikai.Logging.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Kikai.WebApi.Extensions;
using Kikai.Internal.IManagers;
using Kikai.Common.Extensions;

namespace Kikai.WebApi.Handlers
{
    public class RpcAuthenticationHandler : DelegatingHandler
    {
        #region Constants
        private readonly string offerServiceAdminResource = ConfigurationManager.AppSettings["WS_USER_RESOURCE"];
        private readonly string OperationType = operationType.Hub.ToString();
        private readonly string OperationName = operationName.Authentication.ToString();
        #endregion

        #region Private Variables
        private bool xunit = false;
        private IPmp IPmp;
        #endregion

        #region Constructors
        public RpcAuthenticationHandler()
        {

        }

        public RpcAuthenticationHandler(IPmp IPmp)
        {
            this.xunit = true;
            this.IPmp = IPmp;
        }
        #endregion

        #region WebServiceCalls
        private IPmp PMP()
        {
            if (xunit)
                return IPmp;
            else
                return new Pmp();
        }
        #endregion

        #region Methods

        protected override Task<HttpResponseMessage> SendAsync(
                 HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            string _requestId = request.Properties["requestId"].ToString();
            ErrorUtil errorUtil = new ErrorUtil();
            RpcAuthenticationResponse _responseEnvelope = new RpcAuthenticationResponse(_requestId);
            LogObject logObject = new LogObject();
            var errorResponse = new HttpResponseMessage();
            var tsc = new TaskCompletionSource<HttpResponseMessage>();

            //Initialize the error to 0 to be able to check later if there is any error in the request
            int Error = 0;

            if (new CommonMethods().IsLegacyFormat(request.RequestUri.OriginalString))
                request = new CommonMethods().CombobulateRequest(request);

            AuthenticationObject authenticationObject = new CommonMethods().GetAuthenticationHeader(request);
            if (authenticationObject == null || authenticationObject.AuthenticationType == null || authenticationObject.ApiUser == null || authenticationObject.SharedSecret == null)
            {
                Error = ErrorKey.ERR_HUB_AUTHENTICATION_FAILED;
                authenticationObject = new AuthenticationObject();
            }

            //Authenticate with pmp
            else
            {
                Error = PmpAuth(authenticationObject);
            }

            //The following logic has been added to enforce xml output by default if no or incompatible application value is specified in the request header
            var responseType = request.GetRequestContext().Url.Request.Headers.Accept.LastOrDefault();
            if (responseType != null && responseType.ToString().ToLower().Equals("application/json"))
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            else
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            if (Error != 0)
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject(Error));
                errorResponse = new MethodsApi().FormatRpcErrorResponse(request, HttpStatusCode.Unauthorized, _responseEnvelope);
                //R185 Modification
                logObject = new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, _responseEnvelope.Data.Errors);
                log.InfoJson(logObject);
                tsc.SetResult(errorResponse);
                return tsc.Task;
            }

            //Authentication succeeded...continue with the request
            else
            {
                // This piece of code has been added to unit test this handler
                // In case of success we are returning an empty RpcAuthenticationResponse
                if (xunit)
                {
                    tsc.SetResult(new MethodsApi().FormatRpcErrorResponse(request, HttpStatusCode.OK, _responseEnvelope));
                    return tsc.Task;
                }

                logObject = new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, _responseEnvelope.Data.Errors);
                log.InfoJson(logObject);
                var rpcResponse = base.SendAsync(request, cancellationToken);

                //Check if request has missing parameters (e.g. does not specify the term id or the offer id in the url)
                if (rpcResponse.Result.StatusCode == HttpStatusCode.NotFound)
                {
                    _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_HUB_MISSING_DATA_ARGUMENTS));
                    //Log the error
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("URL", rpcResponse.Result.RequestMessage.RequestUri.PathAndQuery);
                    log.InfoJson(new Methods().Error_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, operationName.ApiCall.ToString(), parameters, _responseEnvelope.Data.Errors));
                    errorResponse = new MethodsApi().FormatRpcErrorResponse(request, HttpStatusCode.NotFound, _responseEnvelope);
                    tsc.SetResult(errorResponse);
                    return tsc.Task;
                }

                return rpcResponse;
            }
        }

        private int PmpAuth(AuthenticationObject authenticationObject)
        {
            int error = 0;
            try
            {
                IPmp Pmp = PMP();
                //If Pmp failed to authenticated the request then set the error to authentication failed
                if (!Pmp.Authenticate(authenticationObject.ApiUser, authenticationObject.SharedSecret))
                {
                    error = ErrorKey.ERR_HUB_AUTHENTICATION_FAILED;
                }
            }
            catch
            {
                //Something bad happened (E.g. connection to pmp failed)
                error = ErrorKey.ERR_HUB_AUTHENTICATION_FAILED;
            }
            return error;
        }

        #endregion
    }
}
