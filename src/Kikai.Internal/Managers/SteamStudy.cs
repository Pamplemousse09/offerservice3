using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.IManagers;
using Kikai.Internal.Utils;
using log4net;
using LSR.WebClient;
using LSR.WebClient.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Kikai.Internal.Managers
{
    public class SteamStudy : ISteamStudy
    {

        readonly static WebClientUser UserInformation = (WebClientUser)ConfigurationManager.GetSection("WebClientUser");
        private readonly string steamHeader = ConfigurationManager.AppSettings["SteamHeader"];
        private readonly string steamHeaderValue = ConfigurationManager.AppSettings["SteamHeaderValue"];

        public SteamStudy()
        {
        }

        /// <summary>
        /// Creates a SteamStudyClient that will call the SteamStudy service
        /// </summary>
        /// <returns></returns>
        internal static LSR.WebClient.SteamStudy.RpcClient GetSteamStudyClient()
        {
            var p = new ConfigurationProvider(UserInformation);
            var client = new LSR.WebClient.SteamStudy.RpcClient(p);
            return client;
        }

        /// <summary>
        ///  Call the Steam Study webservice
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>string webservice response</returns>
        private XDocument CallSteamStudyService(string method, Hashtable parameters, Hashtable headerParameters)
        {
            XDocument result = null;
            var steamStudyClient = GetSteamStudyClient();

            string service = "SteamStudyService - " + method;
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                var xmldoc = JsonConvert.DeserializeXmlNode(steamStudyClient.CallWebservice(method, parameters, headerParameters), "QuotaExpressions");
                result = new XmlUtil().ParseXmlDocument(xmldoc.OuterXml);
                LogUtil.CallSuccess(service, result.Document.ToString());
            }
            catch (Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }

            return result;
        }


        /// <summary>
        /// Fetches the attributes expressions at the quota level from the steam service
        /// </summary>
        /// <param name="studyId"></param>
        /// <param name="sampleId"></param>
        public SteamStudyObject GetQuotasAttributes(int studyId, int sampleId)
        {
            var parameters = new Hashtable { { "study", studyId }, { "sample", sampleId } };
            var headerParameters = new Hashtable { { steamHeader, steamHeaderValue } };
            SteamStudyObject steamStudyObject = new SteamStudyObject();

            string service = "QuotasAttributes";
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                var result = CallSteamStudyService("quotaExpressions", parameters, headerParameters);
                LogUtil.CallSuccess(service, result.ToString());

                steamStudyObject.SampleId = sampleId;
                steamStudyObject.ExpressionsXML = result.ToString();
            }
            catch (Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }

            return steamStudyObject;

        }

        /// <summary>
        /// Update the existing ExpressionXml with the QuotaRemaining values and insert the updated one in the column QuotaExpressionsXML 
        /// </summary>
        /// <param name="steamStudyObject"></param>
        /// <param name="gmiSampleQuotasObject"></param>
        public void UpdateQuotaExpression(SteamStudyObject steamStudyObject, GMISampleQuotasObject gmiSampleQuotasObject)
        {

            string xml = steamStudyObject.ExpressionsXML;
            int count = -1;
            int quotaIndex = -1;
            var quotaIdVar = "<quotaId>";
            var quotaIdVarEnd = "</quotaId>";
            var quotaRemainingVar = "<quotaRemaining>";
            var quotaRemainingVarEnd = "</quotaRemaining>";

            var quotas = gmiSampleQuotasObject.GMIQuotasList.Where(p => p.SampleId == steamStudyObject.SampleId).ToList();

            foreach (GMIQuotaObject gmiQuotaObject in quotas)
            {

                count = (quotaIdVar + gmiQuotaObject.QuotaId + quotaIdVarEnd).Count();
                quotaIndex = xml.IndexOf(quotaIdVar + gmiQuotaObject.QuotaId + quotaIdVarEnd) + count;
                if (quotaIndex < count)
                    return;
                xml = xml.Insert(quotaIndex, quotaRemainingVar + gmiQuotaObject.QuotaRemaining + quotaRemainingVarEnd);

            }

            steamStudyObject.QuotaExpressionsXML = xml;
        }


    }
}
