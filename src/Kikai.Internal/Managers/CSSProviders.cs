using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.IManagers;
using Kikai.Internal.Utils;
using LSR.WebClient;
using LSR.WebClient.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kikai.Internal.Managers
{
    public class CSSProviders : ICSSProviders
    {
        readonly static WebClientUser UserInformation = (WebClientUser)ConfigurationManager.GetSection("WebClientUser");

        /// <summary>
        /// Creates a CSSProvidersUser that will call the CSSProviders service
        /// </summary>
        /// <returns></returns>
        internal static LSR.WebClient.CSSProviders.RpcClient GetCSSProviderClient()
        {
            var p = new ConfigurationProvider(UserInformation);
            var client = new LSR.WebClient.CSSProviders.RpcClient(p);
            return client;
        }

        public XDocument CallShowMainstreamProviderService(string providerId)
        {
            XDocument result = null;
            string service = "CSSProviders - ShowMainstreamProviders";
            LogUtil.CallingService(service, providerId);

            try
            {
                result = new XmlUtil().ParseXmlDocument(GetCSSProviderClient().CallShowMainstreamProviders(providerId));
                LogUtil.CallSuccess("showMainstreamProvider", result.ToString());
            }
            catch (Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }
            return result;
        }

        public MainstreamProviderResponseObject GetMainstreamProviderInfo(string providerId)
        {
            var webServiceResponse = this.CallShowMainstreamProviderService(providerId);

            var Errors = webServiceResponse.Descendants("Errors");
            MainstreamProviderResponseObject msProvider = new MainstreamProviderResponseObject();
            if (Errors.Count() > 0)
            {
                List<ServiceErrorObject> serviceErrors = new List<ServiceErrorObject>();
                foreach (var err in Errors)
                {
                    serviceErrors.Add(new ServiceErrorObject()
                    {
                        code = err.Descendants("Code").First().Value,
                        message = err.Descendants("Message").First().Value
                    });
                }
                msProvider.Errors = serviceErrors;
            }
            else
            {
                List<MainstreamProviderObject> msProviders = new List<MainstreamProviderObject>();
                var ProviderItems = webServiceResponse.Element("MainstreamShowProvidersResponse").Element("Data").Elements("Item");
                foreach (var provider in ProviderItems)
                {
                    MainstreamProviderObject p = new MainstreamProviderObject();
                    p.ProviderId = providerId;
                    var firstEnabledWelcomeUrl = provider.Descendants("welcomeUrl").Descendants("Item").Where(w => w.Descendants("enabled").First().Value == "1");
                    
                    if (firstEnabledWelcomeUrl.Count() > 0)
                        p.WelcomeUrlCode = firstEnabledWelcomeUrl.First().Element("urlCode").Value;
                    else
                        p.WelcomeUrlCode = null;

                    if (p.WelcomeUrlCode != null)
                    {
                        p.Enabled = (provider.Descendants("enabled").First().Value == "1") ? true : false;
                    }
                    else
                    {
                        p.Enabled = false;
                    }
                    msProviders.Add(p);
                }
                msProvider.MainstreamProviderObject = msProviders;
            }
            return msProvider;
        }
    }
}
