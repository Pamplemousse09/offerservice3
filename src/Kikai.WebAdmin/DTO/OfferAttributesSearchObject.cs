using System;

namespace Kikai.WebAdmin.DTO
{
    public class OfferAttributesSearchObject
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int totalRows { get; set; }

        public Guid OfferId { get; set; }

        public OfferAttributesSearchObject()
        {
            this.Page = 1;
            this.PageSize = 10;
        }
    }
}