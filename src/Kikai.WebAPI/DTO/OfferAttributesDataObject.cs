using Kikai.BL.DTO.ApiObjects;
using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.WebApi.DTO
{
    [DataContract(Namespace = "")]
    public class OfferAttributesDataObject
    {
        [DataMember(Order = 1, Name = "OfferAttributes")]
        public List<OfferAttributeApiObject> Attributes;
        [DataMember(Order = 2)]
        public List<ErrorObject> Errors;
        public OfferAttributesDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }
    }
}
