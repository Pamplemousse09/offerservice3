using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.BL.DTO
{
    public class AttributeObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Label { get; set; }

        public string Type { get; set; }

        public List<AttributeOptionObject> AttributeOptions { get; set; }

        public AttributeSettingObject AttributeSetting { get; set; }
    }
}
