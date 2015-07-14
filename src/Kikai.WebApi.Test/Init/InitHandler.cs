using Kikai.BL.IRepository;
using Kikai.Internal.IManagers;
using Kikai.WebApi.Decorators;
using Kikai.WebApi.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;

namespace Kikai.WebAPI.Test.Init
{
    public class InitHandler
    {
        #region Methods
        public HttpResponseMessage initProviderAuthenticationHandler(IProviderRepository IProviderRepository, IPmp IPmp, ICSSProviders ICSSProviders, string ResponseFormat = null, string Authorization = null)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/public");
            var requestContext = new HttpRequestContext();
            requestContext.Url = new System.Web.Http.Routing.UrlHelper();
            requestContext.Url.Request = httpRequestMessage;
            if (ResponseFormat != null)
                requestContext.Url.Request.Headers.Add("Accept", "Application/Xml");
            httpRequestMessage.Properties[HttpPropertyKeys.RequestContextKey] = requestContext;
            httpRequestMessage.Properties.Add("requestId", new Guid());
            if (Authorization != null)
                httpRequestMessage.Headers.Add("Authorization", Authorization);
            var handler = new ProviderAuthenticationHandler(IProviderRepository, IPmp, ICSSProviders)
            {
                InnerHandler = new ProviderAuthenticationHandler(IProviderRepository, IPmp, ICSSProviders)
            };
            var client = new HttpClient(handler);
            return client.SendAsync(httpRequestMessage).Result;
        }

        public HttpResponseMessage initRpcAuthenticationHandler(IPmp IPmp, string ResponseFormat = null, string Authorization = null, bool isLegacy = false)
        {
            HttpRequestMessage httpRequestMessage;
            if (!isLegacy)
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost/RpcService");
            else
                httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhost/RpcService?" + Authorization);
            var requestContext = new HttpRequestContext();
            requestContext.Url = new System.Web.Http.Routing.UrlHelper();
            requestContext.Url.Request = httpRequestMessage;
            if (ResponseFormat != null)
                requestContext.Url.Request.Headers.Add("Accept", "Application/Xml");
            httpRequestMessage.Properties[HttpPropertyKeys.RequestContextKey] = requestContext;
            httpRequestMessage.Properties.Add("requestId", new Guid());
            if (Authorization != null & !isLegacy)
                httpRequestMessage.Headers.Add("Authorization", Authorization);
            var handler = new RpcAuthenticationHandler(IPmp)
            {
                InnerHandler = new RpcAuthenticationHandler(IPmp)
            };
            var client = new HttpClient(handler);
            return client.SendAsync(httpRequestMessage).Result;
        }
        #endregion
    }
}
