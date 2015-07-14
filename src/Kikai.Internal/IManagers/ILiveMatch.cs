using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Kikai.Internal.IManagers
{
    public interface ILiveMatch
    {
        XmlDocument CallLiveMatchService(string method, Hashtable parameters);
        List<string> ProcessLiveMatchStudiesActivityResponse(string liveMatchStudiesActivityResponse);
        string GetInternalPid(string pid, string providerId, string requestId = null);
    }
}
