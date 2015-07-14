using Kikai.BL.DTO.ApiObjects;
using Kikai.Domain.Common;
using Kikai.Logging.DTO;
using Kikai.WebApi.Managers.IManagers;
using Kikai.WebApi.Controllers;
using Kikai.WebApi.Decorators;
using Kikai.WebApi.DTO;
using Kikai.WebAPI.Test.Init;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Xunit;

namespace Kikai.WebAPI.Test.Controllers
{
    public class TestOffersControllerTest
    {
        #region Properties
        TestOffersController controller;
        #endregion

        #region Constructor
        public TestOffersControllerTest()
        {
            new InitCache();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get
        /// the active test offers.
        /// We expect the controller to return us a list of active testoffers and a null
        /// error object.
        /// </summary>
        [Fact]
        public void GetTestOffersTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OffersResponse> response;
            OffersDataObject OffersData = new OffersDataObject()
            {
                Errors = new List<ErrorObject>(),
                Offers = new List<OfferApiObject>()
                {
                    new OfferApiObject()
                }
            };

            IOffersManager.Setup(i => i.GetOffers(It.IsAny<HttpRequestMessage>(), It.IsAny<string>())).Returns(OffersData);
            controller = new InitController("Testoffers").initTestOffersController(IOffersManager.Object);
            response = controller.Get() as OkNegotiatedContentResult<OffersResponse>;

            Assert.NotNull(response.Content.Data.Offers);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get
        /// the active test offers.
        /// We expect the controller to return us a null list of active testoffers and an error
        /// object with an error code equals to 3000 which indicates an internal error.
        /// </summary>
        [Fact]
        public void GetTestOffersFailureTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OffersResponse> response;
            OffersDataObject OffersData = new OffersDataObject()
            {
                Errors = new List<ErrorObject>()
                {
                    new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL)
                },
                Offers = null
            };

            IOffersManager.Setup(i => i.GetOffers(It.IsAny<HttpRequestMessage>(), It.IsAny<string>())).Returns(OffersData);
            controller = new InitController("Testoffers").initTestOffersController(IOffersManager.Object);
            response = controller.Get() as OkNegotiatedContentResult<OffersResponse>;

            Assert.Null(response.Content.Data.Offers);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get
        /// the active test offers by pid.
        /// We expect the controller to return us a list of active testoffers that matches
        /// our request and a null error object.
        /// </summary>
        [Fact]
        public void GetTestOffersWithPIDTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OffersResponse> response;
            OffersDataObject OffersData = new OffersDataObject()
            {
                Errors = new List<ErrorObject>(),
                Offers = new List<OfferApiObject>()
                {
                    new OfferApiObject()
                }
            };

            IOffersManager.Setup(i => i.GetOffersByPid(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(),It.IsAny<string>())).Returns(OffersData);
            controller = new InitController("Testoffers").initTestOffersController(IOffersManager.Object);
            response = controller.Get(It.IsAny<string>()) as OkNegotiatedContentResult<OffersResponse>;

            Assert.NotNull(response.Content.Data.Offers);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get
        /// the active test offers by pid.
        /// We expect the controller to return us a null list of active testoffers that matches
        /// our request and an error object with a code equals to 3000 which indicates
        /// an internal error.
        /// </summary>
        [Fact]
        public void GetTestOffersWithPIDFailureTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OffersResponse> response;
            OffersDataObject OffersData = new OffersDataObject()
            {
                Errors = new List<ErrorObject>()
                {
                    new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL)
                },
                Offers = null
            };

            IOffersManager.Setup(i => i.GetOffersByPid(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), It.IsAny<string>())).Returns(OffersData);
            controller = new InitController("Testoffers").initTestOffersController(IOffersManager.Object);
            response = controller.Get(It.IsAny<string>()) as OkNegotiatedContentResult<OffersResponse>;

            Assert.Null(response.Content.Data.Offers);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get
        /// the attributes of an active testoffer.
        /// We expect the controller to return us a list of attributes and a null error object.
        /// </summary>
        [Fact]
        public void GetTestOfferAttributesTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OfferAttributesResponse> response;
            OfferAttributesDataObject OfferAttributesData = new OfferAttributesDataObject()
            {
                Errors = new List<ErrorObject>(),
                Attributes = new List<OfferAttributeApiObject>()
                {
                    new OfferAttributeApiObject()
                }
            };

            IOffersManager.Setup(i => i.GetOfferAttributes(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), new Guid().ToString())).Returns(OfferAttributesData);
            controller = new InitController("Testoffers").initTestOffersController(IOffersManager.Object);
            response = controller.GetAttributes(new Guid().ToString()) as OkNegotiatedContentResult<OfferAttributesResponse>;

            Assert.NotNull(response.Content.Data.Attributes);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get
        /// the attributes of an active testoffer.
        /// We expect the controller to return us a null list of attributes and 
        /// an error object with a code equals to 3000 which indicates an internal error
        /// </summary>
        [Fact]
        public void GetTestOfferAttributesFailureTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OfferAttributesResponse> response;
            OfferAttributesDataObject OfferAttributesData = new OfferAttributesDataObject()
            {
                Errors = new List<ErrorObject>()
                {
                    new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL)
                },
                Attributes = null
            };

            IOffersManager.Setup(i => i.GetOfferAttributes(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), new Guid().ToString())).Returns(OfferAttributesData);
            controller = new InitController("Testoffers").initTestOffersController(IOffersManager.Object);
            response = controller.GetAttributes(new Guid().ToString()) as OkNegotiatedContentResult<OfferAttributesResponse>;

            Assert.Null(response.Content.Data.Attributes);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get
        /// the quota expression of an active testoffer.
        /// We expect the controller to return us the quota cell expression and a null error object.
        /// </summary>
        [Fact]
        public void GetTestOfferQuotaExpressionTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OfferQuotaCellsResponse> response;
            QuotaExpressionsObjectResponse OfferQuotaCellData = new QuotaExpressionsObjectResponse()
            {
                Errors = new List<ErrorObject>(),
                QuotaCells = "UnitTest"
            };

            IOffersManager.Setup(i => i.GetOfferQuotaExpression(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), new Guid().ToString())).Returns(OfferQuotaCellData);
            controller = new InitController("Testoffers").initTestOffersController(IOffersManager.Object);
            response = controller.GetQuotaExpressions(new Guid().ToString()) as OkNegotiatedContentResult<OfferQuotaCellsResponse>;

            Assert.NotNull(response.Content.Data.QuotaCells);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get
        /// the quota expression of an active testoffer.
        /// We expect the controller to return us a null quota cell expression and an error
        /// object with a code equals to 3000 which indicates an internal error
        /// </summary>
        [Fact]
        public void GetTestOfferQuotaExpressionFailureTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OfferQuotaCellsResponse> response;
            QuotaExpressionsObjectResponse OfferQuotaCellData = new QuotaExpressionsObjectResponse()
            {
                Errors = new List<ErrorObject>()
                {
                    new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL)
                },
                QuotaCells = null
            };

            IOffersManager.Setup(i => i.GetOfferQuotaExpression(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), new Guid().ToString())).Returns(OfferQuotaCellData);
            controller = new InitController("Testoffers").initTestOffersController(IOffersManager.Object);
            response = controller.GetQuotaExpressions(new Guid().ToString()) as OkNegotiatedContentResult<OfferQuotaCellsResponse>;

            Assert.Null(response.Content.Data.QuotaCells);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }
        #endregion
    }
}
