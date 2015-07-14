using System.Runtime.Serialization;
using Kikai.WebApi.DTO;
using Kikai.Common.DTO;

namespace Kikai.WebApi.Decorators
{
    [DataContract(Namespace = "", Name = "AttributesResponse")]
    public class CodebookResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public CodebookDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public CodebookResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new CodebookDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}