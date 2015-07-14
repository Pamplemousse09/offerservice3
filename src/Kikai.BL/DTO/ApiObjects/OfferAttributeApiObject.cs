using System.Runtime.Serialization;

namespace Kikai.BL.DTO.ApiObjects
{
    [DataContract(Namespace = "", Name = "OfferAttribute")]
    public class OfferAttributeApiObject
    {
        [DataMember(Name = "Id", Order = 1)]
        public string Name { get; set; }

        [DataMember(Name = "Value", Order = 2)]
        public string Value { get; set; }
    }
}
