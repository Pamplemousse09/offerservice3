using System.Runtime.Serialization;

namespace Kikai.BL.DTO.ApiObjects
{
    [DataContract(Namespace = "", Name = "Attribute")]
    public class AttributeApiObject
    {
        [DataMember(Name = "Id", Order = 1)]
        public string Id { get; set; }

        [DataMember(Name = "Question", Order = 3)]
        public string Label { get; set; }
    }
}
