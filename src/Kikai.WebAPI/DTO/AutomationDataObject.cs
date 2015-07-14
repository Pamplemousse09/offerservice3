using Kikai.Logging.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kikai.WebApi.DTO
{
    [DataContract(Namespace = "")]
    public class AutomationDataObject
    {
        [DataMember(Order = 1)]
        public string Info;
        [DataMember(Order = 2)]
        public List<ErrorObject> Errors;
        public AutomationDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }
    }
}