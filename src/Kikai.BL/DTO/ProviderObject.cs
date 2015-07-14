using System;

namespace Kikai.BL.DTO
{
    public class ProviderObject
    {
        public int Id { get; set; }

        public string ProviderId { get; set; }

        public string WelcomeURLCode { get; set; }

        public bool Enabled { get; set; }

        public DateTime Creation_Date { get; set; }

        public DateTime Update_Date { get; set; }
    }
}
