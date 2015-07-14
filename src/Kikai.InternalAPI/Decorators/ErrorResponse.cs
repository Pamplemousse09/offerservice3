using Kikai.Common.DTO;
using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.InternalAPI.Decorators
{
    [DataContract(Namespace = "", Name = "OfferServiceResponse")]
    public class ErrorResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public List<ErrorObject> Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public ErrorResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new List<ErrorObject>();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}