using Kikai.Logging.Utils;
using Kikai.Internal.IManagers;
using Kikai.Internal.Utils;
using log4net;
using LSR.WebClient;
using LSR.WebClient.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace Kikai.Internal.Managers
{
    public class LiveMatch : ILiveMatch
    {
        readonly static WebClientUser UserInformation = (WebClientUser)ConfigurationManager.GetSection("WebClientUser");
        private const string Screened = "3";
        private const string Completed = "4";
        private const string QuotaFull = "5";

        public LiveMatch()
        {
        }

        /// <summary>
        /// Creates a LiveMatchClient that will call the LiveMatch service
        /// </summary>
        /// <returns></returns>
        internal static LSR.WebClient.LiveMatch.RpcClient GetLiveMatchClient()
        {
            var p = new ConfigurationProvider(UserInformation);
            var client = new LSR.WebClient.LiveMatch.RpcClient(p);
            return client;
        }

        /// <summary>
        /// Calls the Livematch service and returns result as XML Document
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>XmlDocument</returns>
        public XmlDocument CallLiveMatchService(string method, Hashtable parameters)
        {
            var liveMatchClient = GetLiveMatchClient();

            string service = "LiveMatchClient";
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            XmlDocument result = null;
            try
            {
                result = new XmlUtil().LoadXmlDocument(liveMatchClient.CallWebservice(method, parameters));
                LogUtil.CallSuccess(service, result.ToString());
            }
            catch (Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Get the internal respondent ID from the respondent catalogue given the PID and Provider ID
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="providerId"></param>
        /// <param name="requestId"></param>
        /// <returns>String</returns>
        public string GetInternalPid(string pid, string providerId, string requestId = null)
        {
            string localPid = string.Empty;
            var result = string.Empty;

            var parameters = new Hashtable();
            parameters.Add("externalId", pid);
            parameters.Add("providerId", providerId);
            parameters.Add("cookie", providerId + pid);
            parameters.Add("respondentType", "TPLM");

            string service = "GettingInternalPID";
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                var response = CallLiveMatchService("identifyNonPanelist", parameters);
                LogUtil.CallSuccess(service, response.ToString());
                result = ProcessLiveMatchIntenalPIDResponse(response, requestId);
            }
            catch (Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Get the internal PID from the live Match webservice response
        /// </summary>
        /// <param name="liveMatchIntenalPIDResponse"></param>
        /// <returns>String</returns>
        public string ProcessLiveMatchIntenalPIDResponse(XmlDocument liveMatchIntenalPIDResponse, string requestId = null)
        {
            string localPid = string.Empty;
            string callStatus = string.Empty;
            if (liveMatchIntenalPIDResponse.InnerXml == string.Empty)
            {
                localPid = null;
            }
            else
            {
                XmlNodeList nodeList = liveMatchIntenalPIDResponse.GetElementsByTagName("Status");
                foreach (XmlNode node in nodeList)
                {
                    callStatus = node.InnerText;
                }
                if (callStatus.Equals("false"))
                {
                    localPid = null;
                }
                else
                {
                    nodeList = liveMatchIntenalPIDResponse.GetElementsByTagName("NonGTMId");

                    foreach (XmlNode node in nodeList)
                    {
                        localPid = node.InnerText;
                    }
                }
            }
            return localPid;
        }

        /// <summary>
        /// Get the list of studies filtered by the activities: Screened, Completed,QuotaFull
        /// </summary>
        /// <param name="liveMatchStudiesActivityResponse"></param>
        /// <returns>Return list of studies filtered by the activities: Screened, Completed,QuotaFull </returns>
        /// 
        public List<string> ProcessLiveMatchStudiesActivityResponse(string liveMatchStudiesActivityResponse)
        {
            var studiesActivitiesDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var studiesList = new List<string>();

            try
            {
                //Load xml
                var xdoc = new XmlUtil().ParseXmlDocument(liveMatchStudiesActivityResponse);
                studiesList = (from c in xdoc.Descendants("NonGTMStudyData")
                               let xElement = c.Element("ActivityType")
                               let element = c.Element("StudyId")
                               where xElement != null && element != null && (xElement.Value == Completed || xElement.Value == QuotaFull || xElement.Value == Screened)
                               select element.Value).Distinct().ToList();
            }
            catch
            {
                throw;
            }
            return studiesList;
        }

    }
}
