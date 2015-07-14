using Kikai.BL.DTO.ApiObjects;
using Kikai.BL.IRepository;
using Kikai.WebApi.Managers;
using Kikai.WebAPI.Test.Init;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Kikai.WebAPI.Test.Managers
{
    public class AttributesManagerTest
    {
        #region Properties
        AttributesManager AttributesManager;
        HttpRequestMessage Request;
        #endregion

        #region Constructor
        public AttributesManagerTest()
        {
            new InitCache();
            Request = new InitClass().initRequest("attributes");
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// In this test we expect AttributesManager to returns us a non empty list
        /// of published attributes
        /// </summary>
        [Fact]
        public void SelectPublishedAttributesTest()
        {
            var IAttributeRepository = new Mock<IAttributeRepository>();
            List<AttributeApiObject> data = new List<AttributeApiObject>()
            {
               new AttributeApiObject()
            };

            IAttributeRepository.Setup(i => i.SelectPublishedAttributes()).Returns(data);
            AttributesManager = new InitClass().initAttributesManager(IAttributeRepository.Object);

            var response = AttributesManager.GetPublishedAttributes(Request, It.IsAny<string>());

            Assert.NotEmpty(response.Attributes);
            Assert.Empty(response.Errors);
        }

        /// <summary>
        /// In this test we expect that AttributesManager returns us an error with code equal to 
        /// 2016 which indicates that there is no published attributes
        /// </summary>
        [Fact]
        public void SelectPublishedAttributesFailureTest()
        {
            var IAttributeRepository = new Mock<IAttributeRepository>();
            List<AttributeApiObject> data = new List<AttributeApiObject>();
            IAttributeRepository.Setup(i => i.SelectPublishedAttributes()).Returns(data);
            AttributesManager = new InitClass().initAttributesManager(IAttributeRepository.Object);

            var response = AttributesManager.GetPublishedAttributes(Request, It.IsAny<string>());

            Assert.Null(response.Attributes);
            Assert.True(response.Errors.Exists(i => i.Code == 2016));
        }
        #endregion
    }
}
