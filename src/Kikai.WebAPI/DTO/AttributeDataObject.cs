using Kikai.BL.DTO.ApiObjects;
using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.WebApi.DTO
{
    [DataContract(Namespace = "")]
    public class AttributeDataObject
    {
        [DataMember(Order = 1)]
        //Edit for R184
        //public List<AttributeDetailsApiObject> Attributes;
        public AttributeDetailsApiObject Attribute;
        [DataMember(Order = 2)]
        public List<ErrorObject> Errors;
        public AttributeDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }

    }
}
