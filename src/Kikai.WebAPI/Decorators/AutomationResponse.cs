using Kikai.Common.DTO;
using Kikai.WebApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kikai.WebAPI.Decorators
{
    public class AutomationResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public AutomationDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public AutomationResponse(bool status = false)
        {
            this.Status = status;
            this.Data = new AutomationDataObject();
            this.Meta = new MetaDataObject(Guid.NewGuid().ToString());
        }
    }
}