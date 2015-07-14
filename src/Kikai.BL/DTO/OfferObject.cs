using System;
using System.Collections.Generic;

namespace Kikai.BL.DTO
{
    public class OfferObject
    {
        public Guid Id { get; set; }

        public int? StudyId { get; set; }

        public int? SampleId { get; set; }

        public string Title { get; set; }

        public string Topic { get; set; }

        public string Description { get; set; }

        public int? LOI { get; set; }

        public float? IR { get; set; }

        public int Status { get; set; }

        public int? QuotaRemaining { get; set; }

        public string OfferLink { get; set; }

        public bool? TestOffer { get; set; }

        public int? RetryCount { get; set; }

        public DateTime? StudyStartDate { get; set; }

        public DateTime? StudyEndDate { get; set; }

        public List<TermObject> Terms { get; set; }

        public List<RespondentAttributeObject> RespondentAttributes { get; set; }
    }
}
