using Kikai.BL.Concrete;
using Kikai.Domain.Common;
using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.Managers;
using Kikai.WebApi.Decorators;
using Kikai.Logging.DTO;
using Kikai.Logging.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Kikai.WebApi.Extensions;
using Kikai.BL.IRepository;
using Kikai.Internal.IManagers;
using Kikai.BL.DTO;
using Kikai.Common.Extensions;

namespace Kikai.WebApi.Handlers
{
    public class ProviderAuthenticationHandler : DelegatingHandler
    {
        #region Constants
        private readonly string offerServiceUserResource = ConfigurationManager.AppSettings["WS_USER_RESOURCE"];
        private const string authenticationType = "LSR-DIGEST";
        private readonly string OperationType = operationType.WS.ToString();
        private readonly string OperationName = operationName.Authentication.ToString();
        #endregion

        #region Private Variables
        private bool xunit = false;
        private IProviderRepository IProviderRepository;
        private IPmp IPmp;
        private ICSSProviders ICSSProviders;
        #endregion

        #region Constructors
        public ProviderAuthenticationHandler()
        {

        }

        public ProviderAuthenticationHandler(IProviderRepository IProviderRepository, IPmp IPmp, ICSSProviders ICSSProviders)
        {
            this.xunit = true;
            this.IProviderRepository = IProviderRepository;
            this.IPmp = IPmp;
            this.ICSSProviders = ICSSProviders;
        }
        #endregion

        #region Repositorys
        private IProviderRepository ProviderRepository()
        {
            if (xunit)
                return IProviderRepository;
            else
                return new ProviderRepository();
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

        private ICSSProviders CSSProviders()
        {
            if (xunit)
                return ICSSProviders;
            else
                return new CSSProviders();
        }
        #endregion

        #region Methods

        protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            ErrorUtil errorUtil = new ErrorUtil();
            LogObject logObject = new LogObject();
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            string _requestId = request.Properties["requestId"].ToString();
            ProviderAuthenticationResponse _responseEnvelope = new ProviderAuthenticationResponse(_requestId);
            //Initialize the error to 0 to be able to check later if there is any error in the request
            int Error = 0;

            //Get the authentication credentials
            AuthenticationObject authenticationObject = new CommonMethods().GetAuthenticationHeader(request);

            //If header does not contain the required credentials then add an error to the response envelope else 
            if (authenticationObject == null || authenticationObject.AuthenticationType == null || authenticationObject.ApiUser == null || authenticationObject.SharedSecret == null)
            {
                Error = ErrorKey.ERR_PROVIDER_AUTHENTICATION_FAILED;
                authenticationObject = new AuthenticationObject();
            }

            //Authenticate with pmp
            else
            {
                Error = PmpAuth(authenticationObject, _requestId);
            }

            //Error different then 0, send back the request with an error message
            if (Error != 0)
            {

                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters.Add("ApiUser", authenticationObject.ApiUser);

                //Add the error in the response envelope
                //Edit for R184
                _responseEnvelope.Data.Errors.Add(new ErrorObject(Error, parameters));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(Error, parameters));

                //Log the error
                //Edit for R184
                logObject = new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, _responseEnvelope.Data.Errors);
                //R185 Modification
                //logObject = new Methods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, _responseEnvelope.Data.Errors);
                log.InfoJson(logObject);

                var response = new MethodsApi().FormatProviderErrorResponse(request, HttpStatusCode.OK, _responseEnvelope);

                //Return back the results
                tsc.SetResult(response);

                return tsc.Task;
            }

            //Authentication succeeded...continue with the request
            else
            {
                //Authentication and authorization were successful, log the request and  continue processing
                //Edit for R184
                logObject = new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, _responseEnvelope.Data.Errors);
                //R185 Modification
                //logObject = new Methods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, _responseEnvelope.Data.Errors);
                log.InfoJson(logObject);

                // This piece of code has been added to unit test this handler
                // In case of success we are returning an empty ProviderAuthenticationResponse
                if (xunit)
                {
                    tsc.SetResult(new MethodsApi().FormatProviderErrorResponse(request, HttpStatusCode.OK, _responseEnvelope));
                    return tsc.Task;
                }

                var providerResponse = base.SendAsync(request, cancellationToken);

                //Check if request has missing action name (e.g. does not specify the action for liveoffers)
                if (providerResponse.Result.StatusCode == HttpStatusCode.InternalServerError)
                {
                    _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_MISSING_ACTION));
                    var response = new MethodsApi().FormatProviderErrorResponse(request, HttpStatusCode.OK, _responseEnvelope);
                    //Log the error
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("URL", providerResponse.Result.RequestMessage.RequestUri.AbsolutePath);
                    log.InfoJson(new Methods().Error_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, operationName.ApiCall.ToString(), parameters, _responseEnvelope.Data.Errors));
                    tsc.SetResult(response);
                    return tsc.Task;
                }

                return providerResponse;
            }
        }

        /// <summary>
        /// Method that will contact pmp and check if the request sent is authenticated.
        /// If it is authenticated it will check if the provider is authorized to use the offer service
        /// </summary>
        /// <param name="message"></param>
        /// <param name="Error"></param>
        private int PmpAuth(AuthenticationObject authenticationObject, string _requestId)
        {
            bool isPmpAuthenticated = false;
            bool isAuthorized = false;
            int error = 0;

            try
            {
                IPmp Pmp = PMP();
                //Communicate with pmp the provider apiUser and SharedSecret

                isPmpAuthenticated = Pmp.Authenticate(authenticationObject.ApiUser, authenticationObject.SharedSecret);
                if (isPmpAuthenticated)
                {
                    isAuthorized = Pmp.Authorize(authenticationObject.ApiUser, offerServiceUserResource);

                    if (isAuthorized)
                    {
                        //Provider is authorized, mark the request as authorized and continue processing
                        if (HttpContext.Current != null)
                        {
                            HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(authenticationObject.ApiUser, authenticationObject.AuthenticationType), new String[] { "Provider" });
                        }

                        var localProvider = ProviderRepository().SelectByProviderId(authenticationObject.ApiUser);
                        //Checking local database for provider URL Code
                        if (localProvider == null || (DateTime.Now - localProvider.Update_Date).TotalMinutes > Convert.ToInt32(ConfigurationManager.AppSettings["PROVIDER_MANAGEMENT_CHECK_INTERVAL"]))
                        {
                            LoggerFactory.GetLogger().Debug("The provider is either not up to date or does not exist in Offer Service database. Calling the showMainstreamProviders service to get the updated information.");
                            var providerExist = true;
                            var providerEnabled = true;//Enabled status in this context means that he is enabled + has at least 1 welcomeUrlCode

                            //Call the netmr web service
                            var showMainstreamProviderResponse = CSSProviders().GetMainstreamProviderInfo(authenticationObject.ApiUser);
                            if (showMainstreamProviderResponse.Errors != null && showMainstreamProviderResponse.MainstreamProviderObject == null)
                            {
                                providerExist = false;
                                LoggerFactory.GetLogger().ErrorJson(new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, new List<ErrorObject>() { new ErrorObject() { ErrorKey = "ERR_GTM_PROVIDER_NOT_FOUND", Message = "Provider was not found in GTM database." } }));
                                if (localProvider != null)
                                {
                                    ProviderRepository().Delete(localProvider.Id);
                                }
                            }
                            else
                            {
                                if (!showMainstreamProviderResponse.MainstreamProviderObject.Exists(msp => msp.Enabled == true))
                                {
                                    providerEnabled = false;
                                }
                            }

                            if (providerExist)
                            {
                                var providerFromServiceResponse = showMainstreamProviderResponse.MainstreamProviderObject.Find(msp => msp.ProviderId == authenticationObject.ApiUser);

                                //Initializing the new provider information
                                ProviderObject provider = new ProviderObject();
                                if (providerFromServiceResponse != null)
                                {
                                    provider.ProviderId = providerFromServiceResponse.ProviderId;
                                    provider.WelcomeURLCode = providerFromServiceResponse.WelcomeUrlCode;
                                    provider.Enabled = providerFromServiceResponse.Enabled;
                                }

                                if (localProvider != null)
                                {
                                    //The provider exists in the database. Updating the database row.
                                    provider.Id = localProvider.Id;
                                    ProviderRepository().Update(provider);
                                }
                                else
                                {
                                    //The provider does not exist in the database. Inserting the provider to the database.
                                    ProviderRepository().Insert(provider);
                                }

                                if (!providerEnabled)
                                {
                                    error = ErrorKey.ERR_PROVIDER_DEACTIVATED;
                                    LoggerFactory.GetLogger().ErrorJson(new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, new List<ErrorObject>() { new ErrorObject() { ErrorKey = "ERR_GTM_PROVIDER_NOTENABLED", Message = "Provider was not found as enabled in the GTM database. Please make sure that he has the status enabled and has at least one enabled welcome URL." } }));
                                }
                            }
                            else
                            {
                                //return error not exist in hummingbird
                                error = ErrorKey.ERR_PROVIDER_NOT_FOUND;
                            }
                        }
                        //Exists in database AND updated in the last 30 minutes
                        else
                        {
                            LoggerFactory.GetLogger().Debug("Provider is found in the database and is up to date.");
                            //If provider is enabled
                            if (!localProvider.Enabled)
                            {
                                error = ErrorKey.ERR_PROVIDER_DEACTIVATED;
                                LoggerFactory.GetLogger().ErrorJson(new CommonMethods().Authentication_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, authenticationObject, new List<ErrorObject>() { new ErrorObject() { ErrorKey = "ERR_OS_PROVIDER_NOTENABLED", Message = "Provider was not found as enabled in the Offer Service database." } }));
                            }
                        }
                    }
                    else
                    {
                        //Provider authorization failed on pmp
                        error = ErrorKey.ERR_PROVIDER_AUTHORIZATION_FAILED;
                    }

                }
                else
                {
                    //Provider authentication failed on pmp
                    error = ErrorKey.ERR_PROVIDER_AUTHENTICATION_FAILED;
                }
            }
            catch (Exception e)
            {
                //Something bad happened (E.g. connection to pmp failed)
                error = ErrorKey.ERR_PROVIDER_AUTHENTICATION_FAILED;
                LoggerFactory.GetLogger().ErrorJson(new Methods().Exception_ToLogObject(_requestId, authenticationObject.ApiUser, OperationType, OperationName, e));
            }
            return error;
        }
        #endregion
    }
}
