using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kikai.Internal.Contracts.Objects
{
    public class OpenSampleAttributeResponseObject
    {
        public IEnumerable<OpenSampleObject> OpenSampleAttributeList { get; set; }

        public IEnumerable<ServiceErrorObject> ServiceErrorList { get; set; }
    }
}
