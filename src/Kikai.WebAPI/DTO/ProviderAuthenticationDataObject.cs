using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.WebApi.DTO
{
    [DataContract(Namespace = "")]
    public class ProviderAuthenticationDataObject
    {
        [DataMember(Order = 1)]
        public List<ErrorObject> Errors;
        public ProviderAuthenticationDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }
    }
}
