using System;

namespace Kikai.Internal.Contracts.Objects
{
    public class MainstreamStudySampleObject
    {
        public int StudyId { get; set; }

        public int SampleId { get; set; }

        public string SampleName { get; set; }

        public int MainstreamPercentage { get; set; }

        public int OverallQuota { get; set; }

        public int OverallCompletes { get; set; }

        public float RR { get; set; }

        public float IR { get; set; }

        public float CR { get; set; }

        public DateTime? StudyStartDate { get; set; }

        public DateTime? StudyEndDate { get; set; }

        public int LOI { get; set; }
    }
}
