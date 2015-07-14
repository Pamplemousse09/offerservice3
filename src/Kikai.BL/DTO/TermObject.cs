using System;

namespace Kikai.BL.DTO
{
    public class TermObject
    {
        public Guid Id { get; set; }

        public Guid OfferId { get; set; }

        public float? CPI { get; set; }

        public bool? Active { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? Expiration { get; set; }

        public string Created_By { get; set; }

        public string Last_Updated_By { get; set; }
    }
}
