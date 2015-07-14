using Kikai.Logging.DTO;
using System.Collections.Generic;

namespace Kikai.Logging.Extensions.IExtensions
{
    public interface IMethods
    {
        MonitorObject ToMonitorObject(string Method, string Path, string RequestId, bool Success, string StatusCode, int ResponseTime);
                
        LogObject Error_ToLogObject(string RequestId, string User, string Type, string Name, Dictionary<string, string> Parameters, List<ErrorObject> WebServiceResponse = null);
        
        LogObject Response_ToLogObject(string RequestId, string User, string Type, string Name, Dictionary<string, string> Parameters, object WebServiceResponse = null);
    }
}
