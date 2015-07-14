using System;

namespace Kikai.Internal.Contracts.Objects
{
    public class GMISampleObject
    {
        public GMISampleObject() { }

        public GMISampleObject(int SampleId, int InternalSampleId)
        {
            this.SampleId = SampleId;
            this.InternalSampleId = InternalSampleId;
        }

        public int InternalSampleId { get; set; }
        
        public int SampleId { get; set; }

        public Guid OfferId { get; set; }
    }
}
