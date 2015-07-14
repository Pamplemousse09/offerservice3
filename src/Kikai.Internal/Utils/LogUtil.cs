using Kikai.Internal.IUtils;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kikai.Logging.Utils;

namespace Kikai.Internal.Utils
{
    public static class LogUtil
    {
        private static List<string> WebServicesFailureStatus = new List<string>() { "<status>false</status>", "<status>0</status>", "<status></status>" };

        public static string getHashtableString(System.Collections.Hashtable parameters)
        {
            var ParamString = string.Empty;
            foreach (DictionaryEntry entry in parameters)
            {
                ParamString += " " + entry.Key + ": " + entry.Value + ",";
            }
            ParamString.Remove(ParamString.Length - 1);
            return ParamString;
        }

        public static void CallingService(string Service, string Parameters)
        {
            LoggerFactory.GetLogger().Debug(String.Format("Calling service: {0} with parameters: {1}", Service, Parameters));
        }

        public static void CallSuccess(string Service, string Result)
        {
            string status = "successful";
            //Checking if response has failure status
            if (WebServicesFailureStatus.Any(Result.ToLower().Contains))
                status = "unsuccessful";
            LoggerFactory.GetLogger().Debug(String.Format("The call to the service: {0} was " + status + ". Service response: {1}. End of service: {0} response.", Service, Result.Trim()));
        }

        public static void CallFail(string Service, Exception e = null)
        {
            LoggerFactory.GetLogger().Error(String.Format("The call to the service: {0} failed.", Service), e);
        }
    }
}
