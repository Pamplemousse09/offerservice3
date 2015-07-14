using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kikai.BL.DTO.InternalApiObjects
{
    [DataContract(Namespace = "", Name = "Offer")]
    public class InternalApiOffersObject
    {
        [DataMember(Order = 1)]
        public int? StudyId { get; set; }

        [DataMember(Order = 2)]
        public int? SampleId { get; set; }

        [DataMember(Order = 3)]
        public float? CPI { get; set; }

        [DataMember(Order = 4)]
        public int Status { get; set; }
    }
}
