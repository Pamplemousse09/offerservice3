using Kikai.Common.DTO;
using Kikai.InternalAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kikai.InternalAPI.Decorators
{
    [DataContract(Namespace = "", Name = "OfferServiceStudies")]
    public class OffersStudyIdResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public OffersStudyIdDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public OffersStudyIdResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new OffersStudyIdDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}