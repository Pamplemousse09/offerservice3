using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kikai.Internal.Contracts.Objects
{
    public class MainstreamProviderResponseObject
    {
        public List<MainstreamProviderObject> MainstreamProviderObject { get; set; }

        public List<ServiceErrorObject> Errors { get; set; }
    }
}
