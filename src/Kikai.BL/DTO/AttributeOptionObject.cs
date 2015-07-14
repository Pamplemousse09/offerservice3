using System.Runtime.Serialization;

namespace Kikai.BL.DTO
{
    [DataContract(Namespace = "", Name = "AttributeOption")]
    public class AttributeOptionObject
    {
        public string AttributeId { get; set; }

        [DataMember(Name="Option",Order=1)]
        public string Code { get; set; }

        [DataMember(Order=2)]
        public string Description { get; set; }

    }
}
