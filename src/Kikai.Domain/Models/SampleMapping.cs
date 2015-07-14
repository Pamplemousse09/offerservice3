using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("SampleMapping")]
    public class SampleMapping
    {
        public int SampleId { get; set; }
        public int InternalSampleId { get; set; }
        public Guid OfferId { get; set; }

    }
}
