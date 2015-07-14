using System.Runtime.Serialization;
using Kikai.WebApi.DTO;
using Kikai.Common.DTO;

namespace Kikai.WebApi.Decorators
{
    [DataContract(Namespace = "", Name = "LiveOffersResponse")]
    public class OffersResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public OffersDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public OffersResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new OffersDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}