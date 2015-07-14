using Kikai.Internal.Contracts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Kikai.Internal.IManagers
{
    public interface ICSSProviders
    {
        XDocument CallShowMainstreamProviderService(string providerId);

        MainstreamProviderResponseObject GetMainstreamProviderInfo(string providerId);
    }
}
