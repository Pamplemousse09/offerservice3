using Kikai.Internal.Contracts.Objects;
using Kikai.Logging.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kikai.Common.Extensions.IExtensions
{
    public interface ICommonMethods
    {
        LogObject Authentication_ToLogObject(string RequestId, string User, string Type, string Name, AuthenticationObject Parameters, List<ErrorObject> WebServiceResponse = null);
        AuthenticationObject GetAuthenticationHeader(HttpRequestMessage request);
        HttpRequestMessage CombobulateRequest(HttpRequestMessage request);
        HttpRequestMessage CombobulateArgumentRequest(HttpRequestMessage request);
    }
}
