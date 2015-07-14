using System.Collections.Generic;

namespace Kikai.WebAdmin.Models
{
    public class AttributeModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Label { get; set; }

        public bool Publish { get; set; }

        public List<AttributeOptionModel> AttributeOptions { get; set; }
    }
}
