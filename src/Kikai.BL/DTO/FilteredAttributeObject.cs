using Kikai.Domain.Common;

namespace Kikai.BL.DTO
{
    public class FilteredAttributeObject
    {
        public string AttributeId { get; set; }

        public bool Status
        {
            get;
            set;
        }
         
        public string StatusLabel 
        {
            get
            {
                if (Status == false)
                    return AttributeStatus.Unpublished.ToString();
                else
                    return AttributeStatus.Published.ToString();
            }
        }

        public string Label { get; set; }

        public int TotalRows { get; set; }

        public int FromTotalRows { get; set; }

        public int Used { get; set; }

        public string Last_Updated_By { get; set; }

        public string Update_Date { get; set; }
    }
}
