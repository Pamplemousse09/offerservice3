using Kikai.BL.DTO.ApiObjects;
using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.WebApi.DTO
{
    [DataContract(Namespace = "")]
    public class OffersDataObject
    {
        [DataMember(Order = 1)]
        public List<OfferApiObject> Offers;
        [DataMember(Order = 2)]
        public List<ErrorObject> Errors;
        public OffersDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }
    }
}
