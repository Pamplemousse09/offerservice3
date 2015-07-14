using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("RespondentAttributes")]
    public class RespondentAttribute
    {
        public int Id { get; set; }

        public Guid OfferId { get; set; }

        public string Ident { get; set; }

        public string Values { get; set; }

        public DateTime Creation_Date { get; set; }

        public DateTime Update_Date { get; set; }
    }
}
