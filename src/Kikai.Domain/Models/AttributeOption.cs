using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("AttributeOptions")]
    public class AttributeOption
    {
        public string AttributeId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}
