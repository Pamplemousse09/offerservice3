using System.Runtime.Serialization;

namespace Kikai.BL.DTO.WebAdminObjects
{
    [DataContract(Namespace = "", Name = "Attribute")]
    public class AttributeInfoObject
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Type { get; set; }

    }
}
