using System;

namespace Kikai.WebAdmin.UIModels
{
    public class TermsModel
    {
        public Guid Id { get; set; }

        public Guid OfferId { get; set; }

        public String Status
        {
            get
            {
                if (Active == true && Active != null)
                {
                    return "Active";
                }
                else if (Expiration > DateTime.Now)
                {
                    return "Not Active";
                }
                else
                {
                    return "Expired";
                }
            }
        }

        public float? CPI { get; set; }

        public bool? Active { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? Expiration { get; set; }

        public string User { get; set; }
    }
}