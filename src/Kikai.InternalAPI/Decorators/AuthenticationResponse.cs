using Kikai.Common.DTO;
using Kikai.InternalAPI.DTO;
using System.Runtime.Serialization;

namespace Kikai.InternalAPI.Decorators
{
    [DataContract(Namespace = "", Name = "AuthenticationHandlerResponse")]
    public class AuthenticationResponse
    {
        [DataMember(Order = 1)]
        public bool Status;
        [DataMember(Order = 2)]
        public AuthenticationDataObject Data;
        [DataMember(Order = 3)]
        public MetaDataObject Meta;

        public AuthenticationResponse(string RequestId, bool status = false)
        {
            this.Status = status;
            this.Data = new AuthenticationDataObject();
            this.Meta = new MetaDataObject(RequestId);
        }
    }
}