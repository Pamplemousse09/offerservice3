using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("Providers")]
    public class Provider
    {
        public int Id { get; set; }

        public string ProviderId { get; set; }
        
        public string WelcomeURLCode { get; set; }

        public bool Enabled { get; set; }

        public DateTime Creation_Date { get; set; }

        public DateTime Update_Date { get; set; }
    }
}
