
namespace Kikai.WebAdmin.DTO
{
    public class OfferSearchObject
    {
        public int? StudyId { get; set; }

        public string OfferTitle { get; set; }

        public int? OfferStatus { get; set; }

        public int? OfferType { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public OfferSearchObject()
        {
            this.Page = 1;
            this.PageSize = 50;
        }
    }
}