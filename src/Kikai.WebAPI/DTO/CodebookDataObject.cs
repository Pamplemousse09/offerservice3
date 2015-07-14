using Kikai.BL.DTO.ApiObjects;
using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.WebApi.DTO
{
    [DataContract(Namespace = "")]
    public class CodebookDataObject
    {
        [DataMember(Order = 1)]
        public List<AttributeApiObject> Attributes;
        [DataMember(Order = 2)]
        public List<ErrorObject> Errors;
        public CodebookDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }
    }
}
