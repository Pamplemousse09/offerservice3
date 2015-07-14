using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("AttributeSettings")]
    public class AttributeSetting
    {
        [ForeignKey("Attribute")]
        public string AttributeId { get; set; }

        public bool Publish { get; set; }

        public bool Required { get; set; }

        public DateTime Creation_Date { get; set; }

        public DateTime Update_Date { get; set; }

        public string Created_By { get; set; }

        public string Last_Updated_By { get; set; }

        public Models.Attribute Attribute { get; set; }
    }
}
