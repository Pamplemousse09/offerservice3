using Kikai.Common.DTO;
using Kikai.WebApi.DTO;
using System.Runtime.Serialization;

namespace Kikai.WebApi.Decorators
{
    [DataContract(Namespace = "", Name = "RpcOfferResponse")]
    public class RpcAuthenticationResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public RpcAuthenticationDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public RpcAuthenticationResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new RpcAuthenticationDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}