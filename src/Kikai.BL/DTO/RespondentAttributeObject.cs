using System;

namespace Kikai.BL.DTO
{
    public class RespondentAttributeObject
    {
        public int Id { get; set; }

        public Guid OfferId { get; set; }

        public string Ident { get; set; }

        public string Values { get; set; }

        public string Shortname { get; set; }

        public DateTime? Creation_Date { get; set; }

        public DateTime? Update_Date { get; set; }
    }
}
