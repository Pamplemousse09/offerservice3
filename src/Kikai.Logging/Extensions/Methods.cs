using Kikai.Logging.DTO;
using Kikai.Logging.Extensions.IExtensions;
using System;
using System.Collections.Generic;

namespace Kikai.Logging.Extensions
{
    public class Methods : IMethods
    {
        /// <summary>
        /// Converts input variables to a monitor object
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="Path"></param>
        /// <param name="RequestId"></param>
        /// <param name="Success"></param>
        /// <param name="StatusCode"></param>
        /// <param name="ResponseTime"></param>
        /// <returns></returns>
        public MonitorObject ToMonitorObject(string Method, string Path, string RequestId, bool Success, string StatusCode, int ResponseTime)
        {
            MonitorObject monitorObject = new MonitorObject();
            monitorObject.Method = Method;
            monitorObject.Path = Path;
            monitorObject.RequestId = RequestId;
            monitorObject.Success = Success;
            monitorObject.StatusCode = StatusCode;
            monitorObject.ResponseTime = ResponseTime;
            return monitorObject;
        }

        public LogObject Error_ToLogObject(string RequestId, string User, string Type, string Name, Dictionary<string, string> Parameters, List<ErrorObject> WebServiceResponse = null)
        {
            LogObject logObject = new LogObject();
            Dictionary<string, string> response = new Dictionary<string, string>();
            logObject.TimeStamp = DateTime.Now;
            logObject.RequestId = RequestId;
            logObject.User = User;
            logObject.Type = Type;
            logObject.Name = Name;
            logObject.Parameters = Parameters;
            if (WebServiceResponse.Count == 0)
            {
                response.Add("REQUEST_SUCCESSFUL", "The request was successful.");
            }
            else
            {
                foreach (var error in WebServiceResponse)
                {
                    response.Add(error.ErrorKey, error.Message);
                }
            }
            logObject.Response = response;
            return logObject;
        }


        //Edit for R184
        public LogObject ErrorItem_ToLogObject(string RequestId, string User, string Type, string Name, Dictionary<string, string> Parameters, ErrorObject WebServiceResponse = null)
        {
            LogObject logObject = new LogObject();
            Dictionary<string, string> response = new Dictionary<string, string>();
            logObject.TimeStamp = DateTime.Now;
            logObject.RequestId = RequestId;
            logObject.User = User;
            logObject.Type = Type;
            logObject.Name = Name;
            logObject.Parameters = Parameters;
            if (WebServiceResponse == null)
            {
                response.Add("REQUEST_SUCCESSFUL", "The request was successful.");
            }
            else
            {
                response.Add(WebServiceResponse.ErrorKey, WebServiceResponse.Message);
            }
            logObject.Response = response;
            return logObject;
        }

        public LogObject Exception_ToLogObject(string RequestId, string User, string Type, string Name, Exception exception, Dictionary<string, string> Parameters = null)
        {
            LogObject logObject = new LogObject();
            Dictionary<string, string> response = new Dictionary<string, string>();
            logObject.TimeStamp = DateTime.Now;
            logObject.RequestId = RequestId;
            logObject.User = User;
            logObject.Type = Type;
            logObject.Name = Name;
            logObject.Parameters = Parameters;
            response.Add("Exception", exception.Message);
            response.Add("StackTrace", exception.StackTrace);
            logObject.Response = response;
            return logObject;
        }

        public LogObject Response_ToLogObject(string RequestId, string User, string Type, string Name, Dictionary<string, string> Parameters, object WebServiceResponse = null)
        {
            LogObject logObject = new LogObject();
            Dictionary<string, string> response = new Dictionary<string, string>();
            logObject.TimeStamp = DateTime.Now;
            logObject.RequestId = RequestId;
            logObject.User = User;
            logObject.Type = Type;
            logObject.Name = Name;
            logObject.Parameters = Parameters;
            response.Add("REQUEST_SUCCESSFUL", "The request was successful.");
            logObject.Response = response;
            return logObject;
        }
    }
}
