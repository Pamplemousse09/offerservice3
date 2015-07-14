using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kikai.Common.DTO
{
    [DataContract(Namespace = "", Name = "Meta")]
    public class MetaDataObject
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Timestamp { get; set; }
        [DataMember]
        public string Version { get; set; }


        public MetaDataObject(string RequestId)
        {
            this.Id = RequestId;
            this.Timestamp = DateTime.Now.ToString("u");
            this.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
