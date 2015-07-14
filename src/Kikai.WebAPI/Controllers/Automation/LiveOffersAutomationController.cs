using Kikai.BL.DTO.ApiObjects;
using Kikai.Domain.Common;
using Kikai.WebApi.Decorators;
using Kikai.Logging.DTO;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Kikai.WebApi.Managers;
using Kikai.WebApi.Managers.IManagers;
using Kikai.BL.DTO;
using Moq;
using System.Web.Http.Routing;
using Kikai.WebAPI.Managers;
using System.Web.Http.Results;

namespace Kikai.WebApi.Controllers.Automation
{
    public class LiveOffersAutomationController : ApiController
    {
        static IOffersManager OffersManager;

        [ResponseType(typeof(OfferApiObject))]
        public IHttpActionResult Get()
        {
            String providerId = RequestContext.Principal.Identity.Name;
            AutomationManager.OfferObjectToOfferApiObject(AutomationManager.offers, false);
            AutomationManager.RespondentAttributeToOfferApiObject(AutomationManager.offers);
            AutomationManager.IProviderRepository.Setup(i => i.SelectByProviderId(It.IsAny<string>())).Returns(new ProviderObject() { ProviderId = providerId });
            RequestContext.RouteData = new HttpRouteData(RequestContext.RouteData.Route, new HttpRouteValueDictionary { { "controller", "liveoffers" } });
            OffersManager = new OffersManager(AutomationManager.IOfferRepository.Object, AutomationManager.IOfferAttributeRepository.Object, AutomationManager.IProviderRepository.Object, AutomationManager.IAttributeRepository.Object, AutomationManager.ILiveMatch.Object, AutomationManager.IRespondentCatalog.Object, AutomationManager.IQuotaExpressionRepository.Object, AutomationManager.IQuotaMappingRepository.Object, AutomationManager.ISampleMappingRepository.Object, AutomationManager.IGMIStudy.Object, AutomationManager.ISteamStudy.Object, AutomationManager.IQuotaLiveMatch.Object);
            var result = new LiveOffersController(OffersManager, this.Request, this.RequestContext).Get() as OkNegotiatedContentResult<OffersResponse>;
            return Ok(result.Content);
        }

        public IHttpActionResult Get([FromUri]string pid)
        {
            String providerId = RequestContext.Principal.Identity.Name;
            AutomationManager.SetRequiredAttributes();
            AutomationManager.SetStudyIds();
            AutomationManager.OfferObjectToOfferApiObject(AutomationManager.offers, false);
            AutomationManager.RespondentAttributeToOfferApiObject(AutomationManager.offers);
            AutomationManager.IProviderRepository.Setup(i => i.SelectByProviderId(It.IsAny<string>())).Returns(new ProviderObject() { ProviderId = providerId });
            RequestContext.RouteData = new HttpRouteData(RequestContext.RouteData.Route, new HttpRouteValueDictionary { { "controller", "liveoffers" } });
            OffersManager = new OffersManager(AutomationManager.IOfferRepository.Object, AutomationManager.IOfferAttributeRepository.Object, AutomationManager.IProviderRepository.Object, AutomationManager.IAttributeRepository.Object, AutomationManager.ILiveMatch.Object, AutomationManager.IRespondentCatalog.Object, AutomationManager.IQuotaExpressionRepository.Object, AutomationManager.IQuotaMappingRepository.Object, AutomationManager.ISampleMappingRepository.Object, AutomationManager.IGMIStudy.Object, AutomationManager.ISteamStudy.Object, AutomationManager.IQuotaLiveMatch.Object);
            var result = new LiveOffersController(OffersManager, this.Request, this.RequestContext).Get(pid) as OkNegotiatedContentResult<OffersResponse>;
            return Ok(result.Content);
        }

        [ActionName("Attributes")]
        [ResponseType(typeof(OfferAttributeApiObject))]
        public IHttpActionResult GetAttributes(string id)
        {
            String apiUser = RequestContext.Principal.Identity.Name;
            AutomationManager.OfferObjectToOfferApiObject(AutomationManager.offers, false);
            AutomationManager.RespondentAttributeToOfferApiObject(AutomationManager.offers);
            RequestContext.RouteData = new HttpRouteData(RequestContext.RouteData.Route, new HttpRouteValueDictionary { { "controller", "liveoffers" } });
            OffersManager = new OffersManager(AutomationManager.IOfferRepository.Object, AutomationManager.IOfferAttributeRepository.Object, AutomationManager.IProviderRepository.Object, AutomationManager.IAttributeRepository.Object, AutomationManager.ILiveMatch.Object, AutomationManager.IRespondentCatalog.Object, AutomationManager.IQuotaExpressionRepository.Object, AutomationManager.IQuotaMappingRepository.Object, AutomationManager.ISampleMappingRepository.Object, AutomationManager.IGMIStudy.Object, AutomationManager.ISteamStudy.Object, AutomationManager.IQuotaLiveMatch.Object);
            var result = new LiveOffersController(OffersManager, this.Request, this.RequestContext).GetAttributes(id) as OkNegotiatedContentResult<OfferAttributesResponse>;
            return Ok(result.Content);
        }

        [ActionName("QuotaCells")]
        [ResponseType(typeof(QuotaExpressionsApiObject))]
        public IHttpActionResult GetQuotaExpressions(string id)
        {
            AutomationManager.OfferObjectToOfferApiObject(AutomationManager.offers, false);
            String providerId = RequestContext.Principal.Identity.Name;
            AutomationManager.IProviderRepository.Setup(i => i.SelectByProviderId(It.IsAny<string>())).Returns(new ProviderObject() { ProviderId = providerId });
            RequestContext.RouteData = new HttpRouteData(RequestContext.RouteData.Route, new HttpRouteValueDictionary { { "controller", "liveoffers" } });
            OffersManager = new OffersManager(AutomationManager.IOfferRepository.Object, AutomationManager.IOfferAttributeRepository.Object, AutomationManager.IProviderRepository.Object, AutomationManager.IAttributeRepository.Object, AutomationManager.ILiveMatch.Object, AutomationManager.IRespondentCatalog.Object, AutomationManager.IQuotaExpressionRepository.Object, AutomationManager.IQuotaMappingRepository.Object, AutomationManager.ISampleMappingRepository.Object, AutomationManager.IGMIStudy.Object, AutomationManager.ISteamStudy.Object, AutomationManager.IQuotaLiveMatch.Object);
            var result = new LiveOffersController(OffersManager, this.Request, this.RequestContext).GetQuotaExpressions(id) as OkNegotiatedContentResult<OfferQuotaCellsResponse>;
            return Ok(result.Content);
        }
    }
}
