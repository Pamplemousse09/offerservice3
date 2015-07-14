using Kikai.BL.DTO.ApiObjects;
using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.WebApi.DTO
{
    [DataContract(Namespace = "")]
    public class RpcOfferDataObject
    {
        [DataMember(Order = 1)]
        public List<AttributeUsageApiObject> LivematchAttributeUsage;
        [DataMember(Order = 2)]
        public RpcOfferDetailsApiObject Offer;
        [DataMember(Order = 3)]
        public List<ErrorObject> Errors;
        public RpcOfferDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }
    }
}
