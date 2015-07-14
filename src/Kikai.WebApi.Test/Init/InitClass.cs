using Kikai.BL.IRepository;
using Kikai.Internal.IManagers;
using Kikai.WebApi.Managers;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace Kikai.WebAPI.Test.Init
{
    public class InitClass
    {
        #region Methods
        public AttributesManager initAttributesManager(IAttributeRepository IAttributeRepository)
        {
            AttributesManager AttributesManager = new AttributesManager(IAttributeRepository);

            return AttributesManager;
        }

        public OffersManager initOffersManager(
            IOfferRepository IOfferRepository,
            IOfferAttributeRepository IOfferAttributeRepository,
            IProviderRepository IProviderRepository,
            IAttributeRepository IAttributeRepository, 
            ILiveMatch ILiveMatch,
            IRespondentCatalog IRespondentCatalog,
            IQuotaExpressionRepository IQuotaExpressionRepository,
            IQuotaMappingRepository IQuotaMappingRepository,
            ISampleMappingRepository ISampleMappingRepository,
            IGMIStudy IGMIStudy,
            ISteamStudy ISteamStudy,
            IQuotaLiveMatch IQuotaLiveMatch
            )
        {
            OffersManager OffersManager = new OffersManager(
            IOfferRepository, 
            IOfferAttributeRepository, 
            IProviderRepository, 
            IAttributeRepository, 
            ILiveMatch, 
            IRespondentCatalog,
            IQuotaExpressionRepository,
            IQuotaMappingRepository,
            ISampleMappingRepository,
            IGMIStudy,
            ISteamStudy,
            IQuotaLiveMatch);

            return OffersManager;
        }

        public HttpRequestMessage initRequest(string controllerName)
        {
            HttpRequestMessage Request = new HttpRequestMessage();
            HttpConfiguration config = new HttpConfiguration();
            IHttpRoute route = config.Routes.MapHttpRoute(controllerName, "api/{controller}/{id}");
            HttpRouteData routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", controllerName } });
            Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            Request.Properties["requestId"] = new Guid().ToString();
            Request.SetRouteData(routeData);
            return Request;
        }

        public HttpRequestMessage initRequest(string controllerName, string parameters)
        {
            HttpRequestMessage Request = new HttpRequestMessage();
            HttpConfiguration config = new HttpConfiguration();
            IHttpRoute route = config.Routes.MapHttpRoute(controllerName, "api/{controller}/{id}");
            HttpRouteData routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", controllerName } });
            Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            Request.Properties["requestId"] = new Guid().ToString();
            Request.SetRouteData(routeData);
            Request.RequestUri = new Uri("http://localhost/api/public/" + controllerName + "?" + parameters);
            return Request;
        }
        #endregion
    }
}
