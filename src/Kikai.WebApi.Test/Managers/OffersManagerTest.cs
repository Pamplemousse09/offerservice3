using Kikai.BL.DTO;
using Kikai.BL.DTO.ApiObjects;
using Kikai.BL.IRepository;
using Kikai.Internal.IManagers;
using Kikai.WebApi.Managers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using Xunit;
using System.Collections;
using System.Xml;
using Kikai.Internal.Contracts.Objects;
using Kikai.WebAPI.Test.Init;

namespace Kikai.WebAPI.Test.Managers
{
    public class OffersManagerTest
    {
        #region Properties
        OffersManager OffersManager;
        HttpRequestMessage Request;
        Mock<IOfferRepository> IOfferRepository;
        Mock<IOfferAttributeRepository> IOfferAttributeRepository;
        Mock<IProviderRepository> IProviderRepository;
        Mock<IAttributeRepository> IAttributeRepository;
        Mock<ILiveMatch> ILiveMatch;
        Mock<IRespondentCatalog> IRespondentCatalog;
        Mock<IQuotaExpressionRepository> IQuotaExpressionRepository;
        Mock<IQuotaMappingRepository> IQuotaMappingRepository;
        Mock<ISampleMappingRepository> ISampleMappingRepository;
        Mock<IGMIStudy> IGMIStudy;
        Mock<ISteamStudy> ISteamStudy;
        Mock<IQuotaLiveMatch> IQuotaLiveMatch;
        #endregion

        #region Constructor
        public OffersManagerTest()
        {
            new InitCache();
            this.IOfferRepository = new Mock<IOfferRepository>();
            this.IOfferAttributeRepository = new Mock<IOfferAttributeRepository>();
            this.IProviderRepository = new Mock<IProviderRepository>();
            this.IAttributeRepository = new Mock<IAttributeRepository>();
            this.ILiveMatch = new Mock<ILiveMatch>();
            this.IRespondentCatalog = new Mock<IRespondentCatalog>();
            this.IQuotaExpressionRepository = new Mock<IQuotaExpressionRepository>();
            this.IQuotaMappingRepository = new Mock<IQuotaMappingRepository>();
            this.ISampleMappingRepository = new Mock<ISampleMappingRepository>();
            this.IGMIStudy = new Mock<IGMIStudy>();
            this.ISteamStudy = new Mock<ISteamStudy>();
            this.IQuotaLiveMatch = new Mock<IQuotaLiveMatch>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// In this test we expect that OffersManager will return us a list of active
        /// liveoffers
        /// </summary>
        [Fact]
        public void GetAllLiveOffersTest()
        {
            List<OfferApiObject> offersData = new List<OfferApiObject>()
            {
                new OfferApiObject()
            };

            IOfferRepository.Setup(i => i.GetActiveOffersHavingValidTerm(false)).Returns(offersData);
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(new List<OfferAttributeApiObject>());
            IProviderRepository.Setup(i => i.SelectByProviderId(It.IsAny<string>())).Returns(new ProviderObject());
            Request = new InitClass().initRequest("liveoffers");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffers(Request, It.IsAny<string>());

            Assert.NotEmpty(result.Offers);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// In this test we expect that OffersManager will return us an error with a code
        /// equals to 2011 that indicates that there is no active liveoffers in the offer service
        /// </summary>
        [Fact]
        public void GetAllLiveOffersFailureTest()
        {
            List<OfferApiObject> offersData = new List<OfferApiObject>();

            IOfferRepository.Setup(i => i.GetActiveOffersHavingValidTerm(false)).Returns(offersData);
            Request = new InitClass().initRequest("liveoffers");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffers(Request, It.IsAny<string>());

            Assert.Null(result.Offers);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2011));
        }

        /// <summary>
        /// In this test we expect that OffersManager will return us an error with a code
        /// equals to 2011 that indicates that there is no active testoffers in the offer service
        /// </summary>
        [Fact]
        public void GetAllTestOffersTest()
        {
            List<OfferApiObject> offersData = new List<OfferApiObject>()
            {
                new OfferApiObject()
            };


            IOfferRepository.Setup(i => i.GetActiveOffersHavingValidTerm(true)).Returns(offersData);
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(new List<OfferAttributeApiObject>());
            IProviderRepository.Setup(i => i.SelectByProviderId(It.IsAny<string>())).Returns(new ProviderObject());
            Request = new InitClass().initRequest("testoffers");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffers(Request, It.IsAny<string>());

            Assert.NotEmpty(result.Offers);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// In this test we expect that OffersManager will return us a list of active
        /// testoffers
        /// </summary>
        [Fact]
        public void GetAllTestOffersFailureTest()
        {
            List<OfferApiObject> offersData = new List<OfferApiObject>();

            IOfferRepository.Setup(i => i.GetActiveOffersHavingValidTerm(true)).Returns(offersData);
            Request = new InitClass().initRequest("liveoffers");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffers(Request, It.IsAny<string>());

            Assert.Null(result.Offers);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2011));
        }

        /// <summary>
        /// In this test we expect that OffersManager will return us a list of active
        /// liveoffers filtered by pid
        /// </summary>
        [Fact]
        public void GetAllLiveOffersWithPIDTest()
        {
            PIDSetup(false, true);
            Request = new InitClass().initRequest("liveoffers", "COREcontact_country=165&CORElanguage=14");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffersByPid(Request, It.IsAny<string>(), It.IsAny<string>());

            Assert.NotNull(result.Offers);
            Assert.Equal(1, result.Offers.Count);
            Assert.Equal("Xunit Offer", result.Offers[0].Title);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// In this test we are invoking a call to liveoffers controller to get the list of liveoffers
        /// by pid without sending the required parameters in the URL
        /// so we expect that OffersManager will return us an error with a code
        /// equals to 2008 that the required attributes are missing from the request
        /// </summary>
        [Fact]
        public void GetAllLiveOffersWithPIDWithoutRequiredAttributesFailureTest()
        {
            PIDSetup(false, false);
            Request = new InitClass().initRequest("liveoffers");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffersByPid(Request, It.IsAny<string>(), It.IsAny<string>());

            Assert.Null(result.Offers);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2008));
        }

        /// <summary>
        /// In this test we are invoking a call to liveoffers controller to get the list of liveoffers
        /// by pid without having any active liveoffer in the database
        /// so we expect that OffersManager will return us an error with a code
        /// equals to 2012 that there is no active offers that matches the sent attributes
        /// </summary>
        [Fact]
        public void GetAllLiveOffersWithPIDNoLiveOffersFailureTest()
        {
            PIDSetup(false, false);
            Request = new InitClass().initRequest("liveoffers", "COREcontact_country=165&CORElanguage=14");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffersByPid(Request, It.IsAny<string>(), It.IsAny<string>());

            Assert.Null(result.Offers);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2012));
        }

        /// <summary>
        /// In this test we expect that OffersManager will return us a list of active
        /// testoffers filtered by pid
        /// </summary>
        [Fact]
        public void GetAllTestOffersWithPIDTest()
        {
            PIDSetup(true, true);
            Request = new InitClass().initRequest("testoffers", "COREcontact_country=165&CORElanguage=14");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffersByPid(Request, It.IsAny<string>(), It.IsAny<string>());

            Assert.NotNull(result.Offers);
            Assert.Equal(1, result.Offers.Count);
            Assert.Equal("Xunit Offer", result.Offers[0].Title);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get the list of testoffers
        /// by pid without sending the required parameters in the URL
        /// so we expect that OffersManager will return us an error with a code
        /// equals to 2008 that the required attributes are missing from the request
        /// </summary>
        [Fact]
        public void GetAllTestOffersWithPIDWithoutRequiredAttributesFailureTest()
        {
            PIDSetup(true, false);
            Request = new InitClass().initRequest("testoffers");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffersByPid(Request, It.IsAny<string>(), It.IsAny<string>());

            Assert.Null(result.Offers);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2008));
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller to get the list of testoffers
        /// by pid without having any active testoffer in the database
        /// so we expect that OffersManager will return us an error with a code
        /// equals to 2012 that there is no active offers that matches the sent attributes
        /// </summary>
        [Fact]
        public void GetAllTestOffersWithPIDNoTestOffersFailureTest()
        {
            PIDSetup(true, false);
            Request = new InitClass().initRequest("testoffers", "COREcontact_country=165&CORElanguage=14");
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOffersByPid(Request, It.IsAny<string>(), It.IsAny<string>());

            Assert.Null(result.Offers);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2012));
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller demanding the attributes
        /// of a specific liveoffer that is active in the offer service. We expect OffersManager 
        /// to return us a list of attributes.
        /// </summary>
        [Fact]
        public void GetLiveOfferAttributesTest()
        {
            Request = new InitClass().initRequest("liveoffers");
            IOfferRepository.Setup(i => i.SelectByID(new Guid())).Returns(new InitObject().OfferObject(1, false));
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(new InitObject().ListOfferAttributeApiObject());
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferAttributes(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.NotNull(result.Attributes);
            Assert.NotEmpty(result.Attributes);
            Assert.Equal(2, result.Attributes.Count);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller demanding the attributes
        /// of a specific liveoffer that is inactive in the offer service. We expect OffersManager
        /// to return us an error with code equal to 2009 meaning that the offer requested
        /// is either not active or invalid.
        /// </summary>
        [Fact]
        public void GetLiveOfferAttributesFailureTest()
        {
            Request = new InitClass().initRequest("liveoffers");
            IOfferRepository.Setup(i => i.SelectByID(new Guid())).Returns(new InitObject().OfferObject(0, false));
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(new InitObject().ListOfferAttributeApiObject());
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferAttributes(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.Null(result.Attributes);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2009));
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller demanding the attributes
        /// of a specific testoffer that is active in the offer service. We expect OffersManager to return us a list of attributes
        /// </summary>
        [Fact]
        public void GetTestOfferAttributesTest()
        {
            Request = new InitClass().initRequest("testoffers");
            IOfferRepository.Setup(i => i.SelectByID(new Guid())).Returns(new InitObject().OfferObject(1, true));
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(new InitObject().ListOfferAttributeApiObject());
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferAttributes(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.NotNull(result.Attributes);
            Assert.NotEmpty(result.Attributes);
            Assert.Equal(2, result.Attributes.Count);
            Assert.Empty(result.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller demanding the attributes
        /// of a specific testoffer that is inactive in the offer service. We expect OffersManager
        /// to return us an error with code equal to 2009 meaning that the offer requested
        /// is either not active or invalid.
        /// </summary>
        [Fact]
        public void GetTestOfferAttributesFailureTest()
        {
            Request = new InitClass().initRequest("testoffers");
            IOfferRepository.Setup(i => i.SelectByID(new Guid())).Returns(new InitObject().OfferObject(0, true));
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(new InitObject().ListOfferAttributeApiObject());
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferAttributes(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.Null(result.Attributes);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2009));
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller demanding the quota cell expression
        /// of a specific liveoffer that is active in the offer service. We expect OffersManager
        /// to return us a valid quota response
        /// </summary>
        [Fact]
        public void GetLiveOfferQuotaExpressionTest()
        {
            Request = new InitClass().initRequest("liveoffers");
            QuotaCellSetup(false, true, true);
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferQuotaExpression(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.NotNull(result.QuotaCells);
            Assert.Null(result.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller demanding the quota cell expression
        /// of a specific liveoffer that is inactive in the offer service. We expect OffersManager
        /// to return us an error with a code equal to 2009 which means that the offer is either 
        /// inactive or not invalid
        /// </summary>
        [Fact]
        public void GetLiveOfferQuotaExpressionFailureTest()
        {
            Request = new InitClass().initRequest("liveoffers");
            QuotaCellSetup(false, false, true);
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferQuotaExpression(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.Null(result.QuotaCells);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2009));
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller demanding the quota cell expression
        /// of a specific testoffer that is active in the offer service. We expect OffersManager
        /// to return us a valid quota response
        /// </summary>
        [Fact]
        public void GetTestOfferQuotaExpressionTest()
        {
            Request = new InitClass().initRequest("testoffers");
            QuotaCellSetup(true, true, true);
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferQuotaExpression(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.NotNull(result.QuotaCells);
            Assert.Null(result.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to testoffers controller demanding the quota cell expression
        /// of a specific testoffer that is inactive in the offer service. We expect OffersManager
        /// to return us an error with a code equal to 2009 which means that the offer is either 
        /// inactive or not invalid
        /// </summary>
        [Fact]
        public void GetTestOfferQuotaExpressionFailureTest()
        {
            Request = new InitClass().initRequest("testoffers");
            QuotaCellSetup(true, false, true);
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferQuotaExpression(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.Null(result.QuotaCells);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2009));
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller demanding the attributes
        /// of a specific liveoffer that is active in the offer service but has no published attributes. 
        /// We expect OffersManager to return us an error with code equal to 2017 meaning that the offer 
        /// requested has no published attributes.
        /// </summary>
        [Fact]
        public void GetActiveLiveOfferAttributesFailureTest()
        {
            Request = new InitClass().initRequest("liveoffers");
            IOfferRepository.Setup(i => i.SelectByID(new Guid())).Returns(new InitObject().OfferObject(1, false));
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(new InitObject().ListOfferUnpublishedAttributeApiObject());
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferAttributes(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.Null(result.Attributes);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2017));
        }

        /// <summary>
        /// In this test we are simulating a call to liveoffers controller demanding the attributes
        /// of a specific liveoffer that is active in the offer service but has no published attributes. 
        /// We expect OffersManager to return us an error with code equal to 2017 meaning that the offer 
        /// requested has no published attributes.
        /// </summary>
        [Fact]
        public void GetActiveTestOfferAttributesFailureTest()
        {
            Request = new InitClass().initRequest("testoffers");
            IOfferRepository.Setup(i => i.SelectByID(new Guid())).Returns(new InitObject().OfferObject(1, true));
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(new InitObject().ListOfferUnpublishedAttributeApiObject());
            OffersManager = new InitClass().initOffersManager(IOfferRepository.Object, IOfferAttributeRepository.Object, IProviderRepository.Object, IAttributeRepository.Object, ILiveMatch.Object, IRespondentCatalog.Object, IQuotaExpressionRepository.Object, IQuotaMappingRepository.Object, ISampleMappingRepository.Object, IGMIStudy.Object, ISteamStudy.Object, IQuotaLiveMatch.Object);

            var result = OffersManager.GetOfferAttributes(Request, It.IsAny<string>(), new Guid().ToString());

            Assert.Null(result.Attributes);
            Assert.NotNull(result.Errors);
            Assert.True(result.Errors.Exists(i => i.Code == 2017));
        }

        #endregion

        #region Setup
        /// <summary>
        /// In this method we are setting up our mocked objects to prepare all the required information
        /// needed by OffersManager class in order to handle our get offers by pid request
        /// </summary>
        /// <param name="testoffers"></param>
        /// <param name="success"></param>
        private void PIDSetup(bool testoffers, bool success)
        {
            List<OfferApiObject> offersData = null;
            if (success)
            {
                offersData = new List<OfferApiObject>()
                {
                    new InitObject().OfferApiObject(false)
                };
            }
            else
            {
                offersData = new List<OfferApiObject>();
            }

            List<OfferAttributeApiObject> attributesData = new InitObject().ListOfferAttributeApiObject();
            foreach (var attribute in attributesData)
            {
                IAttributeRepository.Setup(i => i.SelectByID(attribute.Name)).Returns(new AttributeObject() { Id = attribute.Name });
            }
            IOfferRepository.Setup(i => i.GetActiveOffersHavingValidTerm(testoffers)).Returns(offersData);
            IOfferRepository.Setup(i => i.GetStudyIdsFromOfferIds('\'' + new Guid().ToString() + '\'')).Returns(new InitObject().ListStudyOfferObject());
            IOfferRepository.Setup(i => i.GetOfferIdsFromStudyIds(It.IsAny<string>())).Returns(new InitObject().ListStudyOfferObject(true));
            IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(new Guid())).Returns(attributesData);
            IProviderRepository.Setup(i => i.SelectByProviderId(It.IsAny<string>())).Returns(new ProviderObject());
            IAttributeRepository.Setup(i => i.SelectRequiredAttributes()).Returns(new InitObject().ListRequiredAttributes());
            IRespondentCatalog.Setup(i => i.GetRespondentCatalogueAttributes(It.IsAny<string>())).Returns(new Dictionary<string,string>());
            ILiveMatch.Setup(i => i.CallLiveMatchService(It.IsAny<string>(), It.IsAny<Hashtable>())).Returns(new XmlDocument());
            ILiveMatch.Setup(i => i.ProcessLiveMatchStudiesActivityResponse(It.IsAny<string>())).Returns(new List<string>() { "1" });
            ILiveMatch.Setup(i => i.GetInternalPid(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("1");

        }

        /// <summary>
        /// In this method we are setting up our mocked object to prepare all the required information
        /// needed by OffersManager class in order to handle our Quota Cell Request
        /// </summary>
        /// <param name="test"></param>
        /// <param name="success"></param>
        private void QuotaCellSetup(bool test, bool active, bool initalized)
        {
            if (active)
            {
                IOfferRepository.Setup(i => i.SelectByID(new Guid())).Returns(new InitObject().OfferObject(1, test));
            }
            else
            {
                IOfferRepository.Setup(i => i.SelectByID(new Guid())).Returns(new InitObject().OfferObject(0, test));
            }
            if (initalized)
            {
                IQuotaExpressionRepository.Setup(i => i.SelectByID(1)).Returns(new SteamStudyObject() { ExpressionsXML = "<xml></xml>", OfferId = new Guid(), QuotaExpressionsXML = "<xml></xml>", SampleId = 0 });
            }
            else
            {
                IQuotaExpressionRepository.Setup(i => i.SelectByID(1)).Returns(new SteamStudyObject() { ExpressionsXML = null, OfferId = new Guid(), QuotaExpressionsXML = null, SampleId = 0 });
            }
        }
        #endregion
    }
}
