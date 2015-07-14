using System;
using System.Runtime.Serialization;

namespace Kikai.BL.DTO.ApiObjects
{
    [DataContract(Namespace = "")]
    public class RpcOfferDetailsApiObject
    {
        [DataMember]
        public Guid OfferId;
        [DataMember]
        public string OfferType;
        [DataMember]
        public int? StudyId;
        [DataMember]
        public int? SampleId;

        public RpcOfferDetailsApiObject(Guid OfferId, int? StudyId, int? SampleId, string OfferType = "TPLM")
        {
            this.OfferId = OfferId;
            this.StudyId = StudyId;
            this.SampleId = SampleId;
            this.OfferType = OfferType;
        }
    }
}
