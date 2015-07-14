using Kikai.WebApi.Handlers;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Kikai.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.MessageHandlers.Add(new MonitorEventHandler());
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.Add(new XmlMediaTypeFormatter());
            config.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.MapHttpAttributeRoutes();

            // Web API routes

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/public/{controller}/{id}/{action}",
                defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional },
                constraints: null,
                handler:
                   HttpClientFactory.CreatePipeline(
                          new System.Web.Http.Dispatcher.HttpControllerDispatcher(config),
                          new DelegatingHandler[]{
                              new ProviderAuthenticationHandler()
                          }
                   )
            );

            config.Routes.MapHttpRoute(
                name: "RpcRoute",
                routeTemplate: "RpcService/{action}",
                defaults: new { controller = "Rpc", action = RouteParameter.Optional },

                constraints: null,
                handler:
                   HttpClientFactory.CreatePipeline(
                          new System.Web.Http.Dispatcher.HttpControllerDispatcher(config),
                          new DelegatingHandler[]{
                              new RpcAuthenticationHandler()
                          }
                   )
            );

            //Automation Controller Routing



            config.Routes.MapHttpRoute(
                name: "LiveOffersAutotmationApi",
                routeTemplate: "api/auto/liveoffers/{id}/{action}",
                defaults: new { controller = "LiveOffersAutomation", id = RouteParameter.Optional, action = RouteParameter.Optional },
                constraints: null,
                handler:
                   HttpClientFactory.CreatePipeline(
                          new System.Web.Http.Dispatcher.HttpControllerDispatcher(config),
                          new DelegatingHandler[]{
                              new ProviderAuthenticationHandler()
                          }
                   )
            );

            config.Routes.MapHttpRoute(
                name: "TestOffersAutotmationApi",
                routeTemplate: "api/auto/testoffers/{id}/{action}",
                defaults: new { controller = "TestOffersAutomation", id = RouteParameter.Optional, action = RouteParameter.Optional },
                constraints: null,
                handler:
                   HttpClientFactory.CreatePipeline(
                          new System.Web.Http.Dispatcher.HttpControllerDispatcher(config),
                          new DelegatingHandler[]{
                              new ProviderAuthenticationHandler()
                          }
                   )
            );

            config.Routes.MapHttpRoute(
                name: "AttributesAutotmationApi",
                routeTemplate: "api/auto/attributes/{id}/{action}",
                defaults: new { controller = "AttributesAutomation", id = RouteParameter.Optional, action = RouteParameter.Optional },
                constraints: null,
                handler:
                   HttpClientFactory.CreatePipeline(
                          new System.Web.Http.Dispatcher.HttpControllerDispatcher(config),
                          new DelegatingHandler[]{
                              new ProviderAuthenticationHandler()
                          }
                   )
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            //config.EnableSystemDiagnosticsTracing();
        }
    }
}
