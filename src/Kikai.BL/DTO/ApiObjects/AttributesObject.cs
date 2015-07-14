using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.BL.DTO.ApiObjects
{
    [DataContract(Namespace = "",Name="Attributes")]
    public class AttributesObject
    {
        [DataMember]
        public List<AttributeApiObject> Attributes;

        public AttributesObject(List<AttributeApiObject> Attributes)
        {
            this.Attributes = Attributes;
        }
    }
}
