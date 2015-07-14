using System;

namespace Kikai.Internal.Contracts.Objects
{
    public class QuotaLiveObject
    {
        public int InternalSampleId { get; set; }

        public int InternalQuotaId { get; set; }
        
        public int QuotaRemaining { get; set; }

        public Guid OfferId { get; set; }
    }
}
