using Kikai.Common.DTO;
using Kikai.WebApi.DTO;
using System.Runtime.Serialization;

namespace Kikai.WebApi.Decorators
{
    [DataContract(Namespace = "", Name = "AttributesWithOptionsResponse")]
    public class AttributeResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public AttributeDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public AttributeResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new AttributeDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}