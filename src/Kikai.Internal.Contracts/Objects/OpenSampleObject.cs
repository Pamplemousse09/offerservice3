using System.Collections.Generic;

namespace Kikai.Internal.Contracts.Objects
{
    public class OpenSampleObject
    {
        public int StudyId { get; set; }

        public string StudyName { get; set; }

        public int SampleId { get; set; }

        public string SampleName { get; set; }

        public float IR { get; set; }

        public int MainstreamQuotaRemaining { get; set; }

        public IEnumerable<AttributeValuesObject> Attributes { get; set; }
    }
}
