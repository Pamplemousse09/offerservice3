using Kikai.Logging.DTO;
using Kikai.Internal.Contracts.Objects;
using Kikai.WebApi.Decorators;
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
using Kikai.WebApi.Extensions.IExtensions;

namespace Kikai.WebApi.Extensions
{
    public class MethodsApi : IMethodsApi
    {
        //Edit for R184
        public LogObject AuthenticationItem_ToLogObject(string RequestId, string User, string Type, string Name, AuthenticationObject Parameters, ErrorObject WebServiceResponse = null)
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
            if (WebServiceResponse == null)
            {
                response.Add("REQUEST_SUCCESSFUL", "The request was successful.");
            }
            else
            {
                response.Add(WebServiceResponse.ErrorKey, WebServiceResponse.Message);
            }
            logObject.Response = response;
            return logObject;
        }/// <summary>

        /// <summary>
        /// Function used to detect the desired response format and return the provider error response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="code"></param>
        /// <param name="_responseEnvelope"></param>
        /// <returns></returns>
        public HttpResponseMessage FormatProviderErrorResponse(HttpRequestMessage request, HttpStatusCode code, ProviderAuthenticationResponse _responseEnvelope)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            MediaTypeWithQualityHeaderValue responseType = null;

            //Check the requested response format
            if (request.GetRequestContext() != null && request.GetRequestContext().Url != null && request.GetRequestContext().Url.Request != null && request.GetRequestContext().Url.Request.Headers != null)
                responseType = request.GetRequestContext().Url.Request.Headers.Accept.LastOrDefault();


            if (responseType != null && responseType.ToString().ToLower().Equals("application/xml"))
                response.Content = new ObjectContent<ProviderAuthenticationResponse>(_responseEnvelope, new XmlMediaTypeFormatter());
            else
                response.Content = new ObjectContent<ProviderAuthenticationResponse>(_responseEnvelope, new JsonMediaTypeFormatter());

            return response;
        }

        /// <summary>
        /// Function used to detect the desired response format and return the rpc error response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="code"></param>
        /// <param name="_responseEnvelope"></param>
        /// <returns></returns>
        public HttpResponseMessage FormatRpcErrorResponse(HttpRequestMessage request, HttpStatusCode code, RpcAuthenticationResponse _responseEnvelope)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            var responseType = request.GetRequestContext().Url.Request.Headers.Accept.LastOrDefault();
            //Check the requested response format

            if (responseType != null && responseType.ToString().ToLower().Equals("application/xml"))
                response.Content = new ObjectContent<RpcAuthenticationResponse>(_responseEnvelope, new XmlMediaTypeFormatter());
            else
                response.Content = new ObjectContent<RpcAuthenticationResponse>(_responseEnvelope, new JsonMediaTypeFormatter());

            return response;
        }
    }
}
