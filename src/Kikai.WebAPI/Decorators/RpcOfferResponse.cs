using System.Runtime.Serialization;
using Kikai.WebApi.DTO;
using Kikai.Common.DTO;

namespace Kikai.WebApi.Decorators
{
    [DataContract(Namespace = "", Name = "RpcOfferResponse")]
    public class RpcOfferResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public RpcOfferDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public RpcOfferResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new RpcOfferDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}