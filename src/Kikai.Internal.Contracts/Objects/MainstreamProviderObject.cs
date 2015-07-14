using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kikai.Internal.Contracts.Objects
{
    public class MainstreamProviderObject
    {
        public string ProviderId { get; set; }

        public string WelcomeUrlCode { get; set; }

        public bool Enabled { get; set; }
    }
}
