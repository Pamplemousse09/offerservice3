using Kikai.BL.DTO;
using System.Collections.Generic;

namespace Kikai.WebAdmin.UIModels
{
    public class OfferAttributesModel
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRows { get; set; }

        public List<RespondentAttributeObject> Attributes { get; set; }
    }
}