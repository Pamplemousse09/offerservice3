using System.Runtime.Serialization;

namespace Kikai.BL.DTO.ApiObjects
{
    [DataContract(Namespace = "", Name = "QuotaExpressions")]
    public class QuotaExpressionsApiObject
    {
        [DataMember]
        public string QuotaExpressions { get; set; }

        public QuotaExpressionsApiObject() { }

        public QuotaExpressionsApiObject(string QuotaExpressions)
        {
            this.QuotaExpressions = QuotaExpressions;
        }
    }
}
