using Kikai.BL.DTO.ApiObjects;
using Kikai.Domain.Common;
using Kikai.Logging.DTO;
using Kikai.WebApi.Controllers;
using Kikai.WebApi.Decorators;
using Kikai.WebApi.DTO;
using Kikai.WebApi.Managers.IManagers;
using Kikai.WebAPI.Test.Init;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Results;
using Xunit;

namespace Kikai.WebAPI.Test.Controllers
{
    public class AttributesControllerTest
    {
        #region Properties
        AttributesController controller;
        #endregion

        #region Constructor
        public AttributesControllerTest()
        {
            new InitCache();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// In this test we are simulating a call to Attributes controller to get the list
        /// of published attributes.
        /// We expect the controller to return us an HTTP response with a non empty list of
        /// published attributes and a null error object.
        /// </summary>
        [Fact]
        public void GetPublishedAttributesTest()
        {
            var IAttributesManager = new Mock<IAttributesManager>();
            OkNegotiatedContentResult<CodebookResponse> response;
            CodebookDataObject AttributesData = new CodebookDataObject()
            {
                Errors = new List<ErrorObject>(),
                Attributes = new List<AttributeApiObject>()
                {
                    new AttributeApiObject()
                }
            };

            IAttributesManager.Setup(i => i.GetPublishedAttributes(It.IsAny<HttpRequestMessage>(), It.IsAny<string>())).Returns(AttributesData);
            controller = new InitController("Attributes").initAttributesController(IAttributesManager.Object);
            response = controller.GetAttributes() as OkNegotiatedContentResult<CodebookResponse>;

            Assert.NotNull(response.Content.Data.Attributes);
            Assert.NotEmpty(response.Content.Data.Attributes);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to Attributes controller to get the list
        /// of published attributes.
        /// We expect the controller to return us an HTTP response with a null list of published
        /// attributes and an error object with code equal to 3000 which indicates and internal error.
        /// </summary>
        [Fact]
        public void GetPublishedAttributesFailureTest()
        {
            var IAttributesManager = new Mock<IAttributesManager>();
            OkNegotiatedContentResult<CodebookResponse> response;
            CodebookDataObject AttributesData = new CodebookDataObject()
            {
                Errors = new List<ErrorObject>()
                {
                    new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL)
                },
                Attributes = null
            };

            IAttributesManager.Setup(i => i.GetPublishedAttributes(It.IsAny<HttpRequestMessage>(), It.IsAny<string>())).Returns(AttributesData);
            controller = new InitController("Attributes").initAttributesController(IAttributesManager.Object);
            response = controller.GetAttributes() as OkNegotiatedContentResult<CodebookResponse>;

            Assert.Null(response.Content.Data.Attributes);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 2998));
        }

        /// <summary>
        /// In this test we are simulating a call to Attributes controller to get the options of 
        /// a requested attribute id.
        /// We expect the controller to return us an HTTP response with the requested info and a null
        /// error object.
        /// </summary>
        [Fact]
        public void GetAttributeWithOptionTest()
        {
            var IAttributesManager = new Mock<IAttributesManager>();
            OkNegotiatedContentResult<AttributeResponse> response;
            AttributeDataObject AttributeWithOptionData = new AttributeDataObject()
            {
                Errors = new List<ErrorObject>(),
                Attribute = new AttributeDetailsApiObject()
            };

            IAttributesManager.Setup(i => i.GetAttribute(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), It.IsAny<string>())).Returns(AttributeWithOptionData);
            controller = new InitController("Attributes").initAttributesController(IAttributesManager.Object);
            response = controller.GetAttribute(It.IsAny<string>()) as OkNegotiatedContentResult<AttributeResponse>;

            Assert.NotNull(response.Content.Data.Attribute);
            Assert.Null(response.Content.Data.Errors);
        }

        /// <summary>
        /// In this test we are simulating a call to Attributes controller to get the options of
        /// a requested attribute id.
        /// We expect the controller to return us a null list of attributes and an error object
        /// with a code equal to 3000 which indicates and internal error.
        /// </summary>
        [Fact]
        public void GetAttributeWithOptionFailureTest()
        {
            var IAttributesManager = new Mock<IAttributesManager>();
            OkNegotiatedContentResult<AttributeResponse> response;
            AttributeDataObject AttributeWithOptionData = new AttributeDataObject()
            {
                Errors = new List<ErrorObject>(){
                    new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL)
                },
                Attribute = null
            };

            IAttributesManager.Setup(i => i.GetAttribute(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), It.IsAny<string>())).Returns(AttributeWithOptionData);
            controller = new InitController("Attributes").initAttributesController(IAttributesManager.Object);
            response = controller.GetAttribute(It.IsAny<string>()) as OkNegotiatedContentResult<AttributeResponse>;

            Assert.Null(response.Content.Data.Attribute);
            Assert.NotNull(response.Content.Data.Errors);
            Assert.True(response.Content.Data.Errors.Exists(i => i.Code == 3000));
        }
        #endregion
    }
}
