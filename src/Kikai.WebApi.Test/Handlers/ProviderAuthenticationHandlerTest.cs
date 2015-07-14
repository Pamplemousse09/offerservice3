using Kikai.BL.DTO;
using Kikai.BL.IRepository;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.IManagers;
using Kikai.WebApi.Decorators;
using Kikai.WebApi.Handlers;
using Kikai.WebAPI.Test.Init;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Xml;
using Xunit;

namespace Kikai.WebAPI.Test.Handlers
{
    public class ProviderAuthenticationHandlerTest
    {
        #region Properties
        private Mock<IProviderRepository> IProviderRepository;
        private Mock<IPmp> IPmp;
        private Mock<ICSSProviders> ICSSProviders;
        #endregion

        #region Constructor
        public ProviderAuthenticationHandlerTest()
        {
            new InitCache();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// In this test we expect a valid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - A valid provider that exists in our databasen and was updated in the last [PROVIDER_MANAGEMENT_CHECK_INTERVAL] minutes
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void ProviderAuthenticationSuccessTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();

            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject() { Enabled = true, Update_Date = DateTime.Now });
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.Empty(response.Data.Errors);
        }

        /// <summary>
        /// In this test we expect an invalid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - A valid provider that exists in our database
        /// - An invalid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGSET)
        /// The handler should respond with an error code equals to 2000
        /// </summary>
        [Fact]
        public void ProviderAuthenticationSharedKeyFailureTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();
            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject());
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(false);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2000));
        }

        /// <summary>
        /// In this test we expect an invalid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - A valid provider that exists in our database
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - An invalid Authorization type (LSR2-DIGEST)
        /// The handler should respond with an error code equals to 2000
        /// </summary>
        [Fact]
        public void ProviderAuthenticationTypeFailureTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();
            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject());
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR2-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2000));
        }

        /// <summary>
        /// In this test we expect an invalid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header not present in the request
        /// The handler should respond with an error code equals to 2000
        /// </summary>
        [Fact]
        public void ProviderAuthenticationNoAuthenticationHeaderFailureTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();
            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object);
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2000));
        }


        /// <summary>
        /// In this test we expect an invalid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - A provider that exists in our database
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that doesn't have OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with an error code equals to 2001
        /// </summary>
        [Fact]
        public void ProviderAuthenticationNotAuthorizedProviderTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();
            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject());
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(false);

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2001));
        }

        /// <summary>
        /// In this test we expect an invalid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - A provider that exists in our database and has the active flag set to false
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has have OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with an error code equals to 2005
        /// </summary>
        [Fact]
        public void ProviderAuthenticationEnabledFlagTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();
            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject() { Enabled = false });
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(false);

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2001));
        }

        /// <summary>
        /// In this test we expect an invalid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request but with missing apiuser parameter
        /// - A valid provider that exists in our database
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with an error code equals to 2000
        /// </summary>
        [Fact]
        public void ProviderAuthenticationMissingApiUserFromAuthorizationHeaderTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();
            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject());
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2000));
        }

        /// <summary>
        /// In this test we expect an invalid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request but with missing sharedkey parameter
        /// - A valid provider that exists in our database
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with an error code equals to 2000
        /// </summary>
        [Fact]
        public void ProviderAuthenticationMissingSharedKeyFromAuthorizationHeaderTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();
            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject());
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2000));
        }

        /*
         * ------------------------------ Changes Introduced to R188 providers ----------------------
         * ------------------------------ Updated ProviderAuthenticationSuccessTest----------------------
         */

        /// <summary>
        /// In this test we expect an invalid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - A disabled provider that exists in our databasen and was updated in the last 30 minutes
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void ProviderAuthenticationNotEnabledProviderTest()
        {
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();
            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject() { Enabled = false, Update_Date = DateTime.Now });
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2005));
        }

        /// <summary>
        /// In this test we expect a valid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - provider exists in database but not up to date
        /// - showMainstreamProvider service returned unknown provider Id
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void ProviderAuthenticationProviderNotUpToDateServiceDidntReturnProviderTest()
        {
            var interval = Convert.ToInt32(ConfigurationManager.AppSettings["PROVIDER_MANAGEMENT_CHECK_INTERVAL"]);
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();

            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject() { Update_Date = DateTime.Now.AddMinutes(interval * -2) });

            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            ICSSProviders.Setup(i => i.GetMainstreamProviderInfo("xunit")).Returns(new MainstreamProviderResponseObject() { Errors = new List<ServiceErrorObject>() { new ServiceErrorObject() { code = "111" } }, MainstreamProviderObject = null });

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2003));
        }

        /// <summary>
        /// In this test we expect a valid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - provider exists in database but not up to date
        /// - showMainstreamProvider service returned disabled provider
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void ProviderAuthenticationProviderNotUpToDateServiceReturnedDisabledProviderTest()
        {
            var interval = Convert.ToInt32(ConfigurationManager.AppSettings["PROVIDER_MANAGEMENT_CHECK_INTERVAL"]);
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();

            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject() { Update_Date = DateTime.Now.AddMinutes(interval * -2) });

            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            ICSSProviders.Setup(i => i.GetMainstreamProviderInfo("xunit")).Returns(new MainstreamProviderResponseObject() { Errors = null, MainstreamProviderObject = new List<MainstreamProviderObject>() { new MainstreamProviderObject() { Enabled = false } } });

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2005));
        }

        /// <summary>
        /// In this test we expect a valid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - provider exists in database but not up to date
        /// - showMainstreamProvider service returned provider with no welcome Url or disabled welcome Url
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void ProviderAuthenticationProviderNotUpToDateServiceReturnedProviderWithNoWelcomeUrlTest()
        {
            var interval = Convert.ToInt32(ConfigurationManager.AppSettings["PROVIDER_MANAGEMENT_CHECK_INTERVAL"]);
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();

            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(new ProviderObject() { Update_Date = DateTime.Now.AddMinutes(interval * -2) });

            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            ICSSProviders.Setup(i => i.GetMainstreamProviderInfo("xunit")).Returns(new MainstreamProviderResponseObject() { Errors = null, MainstreamProviderObject = new List<MainstreamProviderObject>() { new MainstreamProviderObject() { WelcomeUrlCode = null } } });

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2005));
        }

        /// <summary>
        /// In this test we expect a valid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - provider does not exist in database
        /// - showMainstreamProvider service returned provider with no welcome Url or disabled welcome Url
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void ProviderAuthenticationProviderDoesNotExistServiceReturnedProviderWithNoWelcomeUrlTest()
        {
            var interval = Convert.ToInt32(ConfigurationManager.AppSettings["PROVIDER_MANAGEMENT_CHECK_INTERVAL"]);
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();

            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(It.IsAny<ProviderObject>());

            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            ICSSProviders.Setup(i => i.GetMainstreamProviderInfo("xunit")).Returns(new MainstreamProviderResponseObject() { Errors = null, MainstreamProviderObject = new List<MainstreamProviderObject>() { new MainstreamProviderObject() { WelcomeUrlCode = null } } });

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2005));
        }


        /// <summary>
        /// In this test we expect a valid provider authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - provider does not exist in database
        /// - showMainstreamProvider service returned disabled provider
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid provider that has OfferService resource for PMP Authorization (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void ProviderAuthenticationProviderDoesNotExistServiceReturnedDisabledProviderlTest()
        {
            var interval = Convert.ToInt32(ConfigurationManager.AppSettings["PROVIDER_MANAGEMENT_CHECK_INTERVAL"]);
            IProviderRepository = new Mock<IProviderRepository>();
            IPmp = new Mock<IPmp>();
            ICSSProviders = new Mock<ICSSProviders>();

            IProviderRepository.Setup(i => i.SelectByProviderId("xunit")).Returns(It.IsAny<ProviderObject>());

            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);
            IPmp.Setup(i => i.Authorize("xunit", ConfigurationManager.AppSettings["WS_USER_RESOURCE"])).Returns(true);

            ICSSProviders.Setup(i => i.GetMainstreamProviderInfo("xunit")).Returns(new MainstreamProviderResponseObject() { Errors = null, MainstreamProviderObject = new List<MainstreamProviderObject>() { new MainstreamProviderObject() { Enabled = false } } });

            var ProviderAuthenticationHandler = new InitHandler().initProviderAuthenticationHandler(IProviderRepository.Object, IPmp.Object, ICSSProviders.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = ProviderAuthenticationHandler.Content.ReadAsAsync<ProviderAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 2005));
        }
        #endregion
    }
}
