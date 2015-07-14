using System;
using System.Runtime.Serialization;

namespace Kikai.BL.DTO.ApiObjects
{
    [DataContract(Namespace = "", Name = "LiveOffer")]
    public class OfferApiObject
    {
        [DataMember(Order=1)]
        public string Topic { get; set; }

        [DataMember(Order=2)]
        public string Title { get; set; }

        [DataMember(Order=3)]
        public string Description { get; set; }

        [DataMember(Order=4)]
        public int? LOI { get; set; }

        [DataMember(Order=5)]
        public float? IR { get; set; }

        [DataMember(Order=6)]
        public float? CPI { get; set; }

        [DataMember(Order=7)]
        public Guid OfferID { get; set; }

        [DataMember(Order=8)]
        public Guid TermID { get; set; }

        [DataMember(Order=9)]
        public string OfferLink { get; set; }

        [IgnoreDataMember]
        public Boolean TestOffer { get; set; }

        [DataMember(Order=10)]
        public int? QuotaRemaining { get; set; }

        public void updateLink(string country = "[COUNTRYCODE]", string ProviderLink = "[PROVIDERCODE]")
        {
            OfferLink = String.Format(this.OfferLink ?? string.Empty, this.OfferID, this.TermID, ("E_" + country + "_" + ProviderLink)).Replace("&oid=", "&id=[ID]&oid=");
        }
    }
}
