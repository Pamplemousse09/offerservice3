using Kikai.BL.DTO;
using System.Collections.Generic;

namespace Kikai.WebAdmin.UIModels
{
    public class OfferModel
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public List<FilteredOfferObject> Offers { get; set; }
    }
}