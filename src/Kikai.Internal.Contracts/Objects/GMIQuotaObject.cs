using System;


namespace Kikai.Internal.Contracts.Objects
{
    public class GMIQuotaObject
    {
        public int InternalQuotaId { get; set; }

        public int QuotaId { get; set; }

        public int SampleId { get; set; }

        public int QuotaRemaining { get; set; }

        public Guid OfferId { get; set; }
    }
}
