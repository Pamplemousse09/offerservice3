using Kikai.BL.DTO;
using Kikai.BL.IRepository;
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
using Xunit;

namespace Kikai.WebAPI.Test.Handlers
{
    public class RpcAuthenticationHandlerTest
    {

        #region Properties
        private Mock<IPmp> IPmp;
        #endregion

        #region Constructor
        public RpcAuthenticationHandlerTest()
        {
            new InitCache();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// In this test we expect a valid hub authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void RpcAuthenticationSuccessTest()
        {
            IPmp = new Mock<IPmp>();
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);

            var RpcAuthenticationHandler = new InitHandler().initRpcAuthenticationHandler(IPmp.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = RpcAuthenticationHandler.Content.ReadAsAsync<RpcAuthenticationResponse>().Result;

            Assert.Empty(response.Data.Errors);
        }

        /// <summary>
        /// In this test we expect a invalid hub authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - An invalid sharedSecret for PMP Authentication (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with an error code equals 7
        /// </summary>
        [Fact]
        public void RpcAuthenticationSharedKeyFailureTest()
        {
            IPmp = new Mock<IPmp>();
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(false);

            var RpcAuthenticationHandler = new InitHandler().initRpcAuthenticationHandler(IPmp.Object, null, "LSR-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = RpcAuthenticationHandler.Content.ReadAsAsync<RpcAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 7));
        }

        /// <summary>
        /// In this test we expect a invalid hub authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header present in the request
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - An invalid Authorization type (LSR2-DIGEST)
        /// The handler should respond with an error code equals 7
        /// </summary>
        [Fact]
        public void RpcInvalidAuthenticationTypeFailureTest()
        {
            IPmp = new Mock<IPmp>();
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);

            var RpcAuthenticationHandler = new InitHandler().initRpcAuthenticationHandler(IPmp.Object, null, "LSR2-DIGEST apiuser=xunit, sharedSecret=xunit");
            var response = RpcAuthenticationHandler.Content.ReadAsAsync<RpcAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 7));
        }

        /// <summary>
        /// In this test we expect a invalid hub authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header not present in the request
        /// - An invalid sharedSecret for PMP Authentication (simulation)
        /// - A valid Authorization type (LSR-DIGEST)
        /// The handler should respond with an error code equals 7
        /// </summary>
        [Fact]
        public void RpcAuthenticationNoAuthenticationHeaderFailureTest()
        {
            IPmp = new Mock<IPmp>();
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(false);

            var RpcAuthenticationHandler = new InitHandler().initRpcAuthenticationHandler(IPmp.Object);
            var response = RpcAuthenticationHandler.Content.ReadAsAsync<RpcAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 7));
        }

        /// <summary>
        /// In this test we expect a valid hub authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header not present in the request
        /// - A valid sharedSecret for PMP Authentication (simulation)
        /// - A legacy format authentication (authorization sent as a parameter in the URL)
        /// The handler should respond with a null error object
        /// </summary>
        [Fact]
        public void RpcAuthenticationLegacyFormatTest()
        {
            IPmp = new Mock<IPmp>();
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(true);

            var RpcAuthenticationHandler = new InitHandler().initRpcAuthenticationHandler(IPmp.Object, null, "username=xunit&sharedSecret=xunit", true);
            var response = RpcAuthenticationHandler.Content.ReadAsAsync<RpcAuthenticationResponse>().Result;

            Assert.Empty(response.Data.Errors);
        }

        /// <summary>
        /// In this test we expect a invalid hub authentication.
        /// We are providing the following characteristics to the handler:
        /// - Authorization header not present in the request
        /// - A invalid sharedSecret for PMP Authentication (simulation)
        /// - A legacy format authentication (authorization sent as a parameter in the URL)
        /// The handler should respond with an error code equals 7
        /// </summary>
        [Fact]
        public void RpcAuthenticationLegacyFormatFailureTest()
        {
            IPmp = new Mock<IPmp>();
            IPmp.Setup(i => i.Authenticate("xunit", "xunit")).Returns(false);

            var RpcAuthenticationHandler = new InitHandler().initRpcAuthenticationHandler(IPmp.Object, null, "username=xunit&sharedSecret=xunit", true);
            var response = RpcAuthenticationHandler.Content.ReadAsAsync<RpcAuthenticationResponse>().Result;

            Assert.True(response.Data.Errors.Exists(i => i.Code == 7));
        }
        #endregion
    }
}
