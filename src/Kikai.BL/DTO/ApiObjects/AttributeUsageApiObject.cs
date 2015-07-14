using System.Runtime.Serialization;

namespace Kikai.BL.DTO.ApiObjects
{
    [DataContract(Namespace = "",Name="AttributeUsage")]
    public class AttributeUsageApiObject
    {
        [DataMember(Name="AttributeName")]
        public string Ident { get; set; }
    }
}
