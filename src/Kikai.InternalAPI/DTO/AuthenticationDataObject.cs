using Kikai.Logging.DTO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.InternalAPI.DTO
{
    [DataContract(Namespace = "")]
    public class AuthenticationDataObject
    {
        [DataMember(Order = 1)]
        public List<ErrorObject> Errors;
        public AuthenticationDataObject()
        {
            this.Errors = new List<ErrorObject>();
        }
    }
}