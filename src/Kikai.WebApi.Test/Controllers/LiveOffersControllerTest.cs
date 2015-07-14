using Kikai.BL.DTO.ApiObjects;
using Kikai.Logging.DTO;
using Kikai.Domain.Common;
using Kikai.WebApi.Controllers;
using Kikai.WebApi.Decorators;
using Kikai.WebApi.DTO;
using Kikai.WebApi.Managers.IManagers;
using Kikai.WebAPI.Test.Init;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Results;
using Xunit;

namespace Kikai.WebAPI.Test.Controllers
{
    public class LiveOffersControllerTest
    {
        #region Properties
        LiveOffersController controller;
        #endregion

        #region Constructor
        public LiveOffersControllerTest()
        {
            new InitCache();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// In this test we are simulating a call to liveoffers controller to get
        /// the active live offers.
        /// We expect the controller to return us a list of active liveoffers and a null
        /// error object.
        /// </summary>
        [Fact]
        public void GetLiveOffersTest()
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
            controller = new InitController("Liveoffers").initLiveOffersController(IOffersManager.Object);
            response = controller.Get() as OkNegotiatedContentResult<OffersResponse>;

            Assert.NotNull(response.Content.Data.Offers);
            Assert.NotEmpty(response.Content.Data.Offers);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller to get
        /// the active live offers.
        /// We expect the controller to return us a null list of active liveoffers and an error
        /// object with an error code equals to 3000 which indicates an internal error.
        /// </summary>
        [Fact]
        public void GetLiveOffersFailureTest()
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
            controller = new InitController("Liveoffers").initLiveOffersController(IOffersManager.Object);
            response = controller.Get() as OkNegotiatedContentResult<OffersResponse>;

            Assert.Null(response.Content.Data.Offers);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller to get
        /// the active live offers by pid.
        /// We expect the controller to return us a list of active liveoffers that matches
        /// our request and a null error object.
        /// </summary>
        [Fact]
        public void GetLiveOffersWithPIDTest()
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
            controller = new InitController("Liveoffers").initLiveOffersController(IOffersManager.Object);
            response = controller.Get(It.IsAny<string>()) as OkNegotiatedContentResult<OffersResponse>;

            Assert.NotNull(response.Content.Data.Offers);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller to get
        /// the active live offers by pid.
        /// We expect the controller to return us a null list of active liveoffers that matches
        /// our request and an error object with a code equals to 3000 which indicates
        /// an internal error.
        /// </summary>
        [Fact]
        public void GetLiveOffersWithPIDFailureTest()
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
            controller = new InitController("Liveoffers").initLiveOffersController(IOffersManager.Object);
            response = controller.Get(It.IsAny<string>()) as OkNegotiatedContentResult<OffersResponse>;

            Assert.Null(response.Content.Data.Offers);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller to get
        /// the attributes of an active liveoffer.
        /// We expect the controller to return us a list of attributes and a null error object.
        /// </summary>
        [Fact]
        public void GetLiveOfferAttributesTest()
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
            controller = new InitController("Liveoffers").initLiveOffersController(IOffersManager.Object);
            response = controller.GetAttributes(new Guid().ToString()) as OkNegotiatedContentResult<OfferAttributesResponse>;

            Assert.NotNull(response.Content.Data.Attributes);
            Assert.NotEmpty(response.Content.Data.Attributes);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller to get
        /// the attributes of an active liveoffer.
        /// We expect the controller to return us a null list of attributes and 
        /// an error object with a code equals to 3000 which indicates an internal error
        /// </summary>
        [Fact]
        public void GetLiveOfferAttributesFailureTest()
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
            controller = new InitController("Liveoffers").initLiveOffersController(IOffersManager.Object);
            response = controller.GetAttributes(new Guid().ToString()) as OkNegotiatedContentResult<OfferAttributesResponse>;

            Assert.Null(response.Content.Data.Attributes);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller to get
        /// the quota expression of an active liveoffer.
        /// We expect the controller to return us the quota cell expression and a null error object.
        /// </summary>
        [Fact]
        public void GetLiveOfferQuotaExpressionTest()
        {
            var IOffersManager = new Mock<IOffersManager>();
            OkNegotiatedContentResult<OfferQuotaCellsResponse> response;
            QuotaExpressionsObjectResponse OfferQuotaCellData = new QuotaExpressionsObjectResponse()
            {
                Errors = new List<ErrorObject>(),
                QuotaCells = "UnitTest"
            };

            IOffersManager.Setup(i => i.GetOfferQuotaExpression(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), new Guid().ToString())).Returns(OfferQuotaCellData);
            controller = new InitController("Liveoffers").initLiveOffersController(IOffersManager.Object);
            response = controller.GetQuotaExpressions(new Guid().ToString()) as OkNegotiatedContentResult<OfferQuotaCellsResponse>;

            Assert.NotNull(response.Content.Data.QuotaCells);
            Assert.NotEmpty((string)response.Content.Data.QuotaCells);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller to get
        /// the quota expression of an active liveoffer.
        /// We expect the controller to return us a null quota cell expression and an error
        /// object with a code equals to 3000 which indicates an internal error
        /// </summary>
        [Fact]
        public void GetLiveOfferQuotaExpressionFailureTest()
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
            controller = new InitController("Liveoffers").initLiveOffersController(IOffersManager.Object);
            response = controller.GetQuotaExpressions(new Guid().ToString()) as OkNegotiatedContentResult<OfferQuotaCellsResponse>;

            Assert.Null(response.Content.Data.QuotaCells);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }
        #endregion
    }
}
