using System;

namespace Kikai.BL.DTO
{
    public class SampleObject
    {
        public int SampleId { get; set; }

        public Guid Id {get; set;}

        public int RetryCount { get; set; }

        public DateTime StudyStartDate { get; set; }

        public DateTime? StudyEndDate { get; set; }
    }
}
