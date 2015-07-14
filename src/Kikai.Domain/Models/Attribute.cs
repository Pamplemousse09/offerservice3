using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kikai.Domain.Models
{
    [Table("Attributes")]
    public class Attribute
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Label { get; set; }

        public string Type { get; set; }

        ICollection<AttributeOption> attributeOptions;

        public virtual AttributeSetting AttributeSettings { get; set; }

        public virtual ICollection<AttributeOption> AttributeOptions
        {
            get
            {
                if (attributeOptions == null)
                    attributeOptions = new HashSet<AttributeOption>();
                return attributeOptions;
            }
            set
            {
                attributeOptions = value;
            }
        }
    }
}
