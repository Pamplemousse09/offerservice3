using System;

namespace Kikai.Internal.Contracts.Objects
{
    public class SteamStudyObject
    {
        public int SampleId { get; set; }

        public string ExpressionsXML { get; set; }
        
        public string QuotaExpressionsXML { get; set; }

        public Guid OfferId { get; set; }
    }
}
