using Kikai.BL.DTO;
using System.Collections.Generic;

namespace Kikai.WebAdmin.UIModels
{
    public class AttributeModel
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public List<FilteredAttributeObject> Attributes { get; set; }
    }
}