
namespace Kikai.WebAdmin.DTO
{
    public class AttributeSearchObject
    {
        public string AttributeId { get; set; }

        public int? Published { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public AttributeSearchObject()
        {
            this.Page = 1;
            this.PageSize = 100;
        }
    }
}