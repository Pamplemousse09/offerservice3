using System;

namespace Kikai.BL.DTO
{
    public class FilteredOfferObject
    {
        public Guid OfferId { get; set; }

        public int StudyId { get; set; }

        public int SampleId { get; set; }

        public string Title { get; set; }

        public string Topic { get; set; }

        public string Description { get; set; }
    
        public int QuotaRemaining { get; set;}

        public DateTime StudyStartDate { get; set; }

        public DateTime? StudyEndDate { get; set; }

        public int LOI { get; set; }

        public int IR { get; set; }

        public bool TestOffer { get; set; }

        public Guid? TermId { get; set; }

        public float? CPI { get; set; }

        public int Status { get; set; }

        public int TotalRows { get; set; }

        public int FromTotalRows { get; set; }
    }
}
