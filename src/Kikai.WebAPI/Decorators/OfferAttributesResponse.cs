using System.Runtime.Serialization;
using Kikai.WebApi.DTO;
using Kikai.Common.DTO;

namespace Kikai.WebApi.Decorators
{
    [DataContract(Namespace = "", Name = "OfferAttributesResponse")]
    public class OfferAttributesResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public OfferAttributesDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public OfferAttributesResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new OfferAttributesDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}