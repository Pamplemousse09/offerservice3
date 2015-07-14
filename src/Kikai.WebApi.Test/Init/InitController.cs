using Kikai.WebApi.Controllers;
using Kikai.WebApi.Managers.IManagers;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace Kikai.WebAPI.Test.Init
{
    public class InitController
    {
        #region Properties
        HttpConfiguration config;
        HttpRequestMessage request;
        IHttpRoute route;
        HttpRouteData routeData;
        #endregion

        #region Constructor
        public InitController(String controllerName)
        {
            this.config = new HttpConfiguration();
            this.request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/"+controllerName+"Controller");
            this.route = config.Routes.MapHttpRoute(controllerName, "api/{controller}/{id}");
            this.routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", controllerName } });
        }
        #endregion

        #region Methods
        public LiveOffersController initLiveOffersController(IOffersManager IOffersManager)
        {
            LiveOffersController controller = new LiveOffersController(IOffersManager);
            controller.Request = this.request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = this.config;
            controller.Request.Properties["requestId"] = new Guid().ToString();
            controller.RequestContext.RouteData = this.routeData;
            return controller;
        }

        public TestOffersController initTestOffersController(IOffersManager IOffersManager)
        {
            TestOffersController controller = new TestOffersController(IOffersManager);
            controller.Request = this.request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = this.config;
            controller.Request.Properties["requestId"] = new Guid().ToString();
            controller.RequestContext.RouteData = this.routeData;
            return controller;
        }

        public AttributesController initAttributesController(IAttributesManager IAttributesManager)
        {
            AttributesController controller = new AttributesController(IAttributesManager);
            controller.Request = this.request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = this.config;
            controller.Request.Properties["requestId"] = new Guid().ToString();
            controller.RequestContext.RouteData = this.routeData;
            return controller;
        }
        #endregion
    }
}
