using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("QuotaExpressions")]
    public class QuotaExpressions
    {
        public int SampleId { get; set; }
        public string ExpressionsXML { get; set; }
        public string QuotaExpressionsXML { get; set; }
        public Guid OfferId { get; set; }

    }
}
