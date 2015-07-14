using Kikai.Common.DTO;
using Kikai.WebApi.DTO;
using System.Runtime.Serialization;

namespace Kikai.WebApi.Decorators
{
    [DataContract(Namespace = "", Name = "AuthHandlerResponse")]
    public class ProviderAuthenticationResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public ProviderAuthenticationDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public ProviderAuthenticationResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new ProviderAuthenticationDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}