using Kikai.BL.DTO.InternalApiObjects;
using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.InternalAPI.DTO
{
    [DataContract(Namespace = "")]
    public class OffersStudyIdDataObject
    {
        [DataMember(Order = 1)]
        public List<InternalApiOffersObject> Offers;
        [DataMember(Order = 2)]
        public List<ErrorObject> Errors;
        public OffersStudyIdDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }
    }
}