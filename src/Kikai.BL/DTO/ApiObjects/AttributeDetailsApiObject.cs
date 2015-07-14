using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.BL.DTO.ApiObjects
{
    [DataContract(Namespace = "", Name = "Attribute")]
    public class AttributeDetailsApiObject
    {
        [DataMember(Name = "Id", Order = 1)]
        public string Id { get; set; }

        [DataMember(Name = "Question", Order = 3)]
        public string Label { get; set; }

        [DataMember(Name = "Type", Order = 4)]
        public string Type { get; set; }

        [DataMember(Name = "AttributeOptions", Order = 5)]
        public List<AttributeOptionObject> AttributeOptions { get; set; }
    }
}
