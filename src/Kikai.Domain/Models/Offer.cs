using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("Offers")]
    public class Offer
    {
        public Guid Id { get; set; }

        public int StudyId { get; set; }

        public int SampleId { get; set; }

        public string Title { get; set; }

        public string  Topic { get; set; }

        public string Description { get; set; }

        public int LOI { get; set; }

        public float IR { get; set; }

        public int Status { get; set; }

        public int QuotaRemaining { get; set; }

        public string OfferLink { get; set; }

        public bool TestOffer { get; set; }

        public DateTime StudyStartDate { get; set; }

        public DateTime StudyEndDate { get; set; }

        public int RetryCount { get; set; }

        ICollection<Term> terms;

        ICollection<RespondentAttribute> respondentAttributes;

        public virtual ICollection<Term> Terms
        {
            get
            {
                if (terms == null)
                    terms = new HashSet<Term>();
                return terms;
            }
            set
            {
                terms = value;
            }
        }

        public virtual ICollection<RespondentAttribute> RespondentAttributes
        {
            get
            {
                if (respondentAttributes == null)
                    respondentAttributes = new HashSet<RespondentAttribute>();
                return respondentAttributes;          
            }
            set
            {
                respondentAttributes = value;
            }
        }
    }
}
