using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("QuotaMapping")]
    public class QuotaMapping
    {
        public int SampleId { get; set; }
        public int QuotaId { get; set; }
        public int InternalQuotaId { get; set; }
        public int QuotaRemaining { get; set; }
        public Guid OfferId { get; set; }
    }
}
