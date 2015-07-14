using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.IManagers;
using Kikai.Internal.Utils;
using log4net;
using LSR.WebClient;
using LSR.WebClient.Configuration;
using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Kikai.Internal.Managers
{
    public class QuotaLiveMatch : IQuotaLiveMatch
    {
        readonly static WebClientUser UserInformation = (WebClientUser)ConfigurationManager.GetSection("WebClientUser");

        public QuotaLiveMatch()
        {
        }

        /// <summary>
        /// Creates a QuotaLiveMatchClient that will call the QuotaLiveMatch service
        /// </summary>
        /// <returns></returns>
        internal static LSR.WebClient.QuotaLiveMatch.RpcClient GetQuotaLiveMatchClient()
        {
            var p = new ConfigurationProvider(UserInformation);
            var client = new LSR.WebClient.QuotaLiveMatch.RpcClient(p);
            return client;
        }


        /// <summary>
        ///  Call the Quota LiveMatch webservice
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>string webservice response</returns>
        private XDocument CallQuotaLiveService(string method, Hashtable parameters)
        {
            XDocument result = null;

            string service = "QuotaLiveService - " + method;
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                var quotaLiveMatchClient = GetQuotaLiveMatchClient();
                result = new XmlUtil().ParseXmlDocument(quotaLiveMatchClient.CallWebservice(method, parameters));

                LogUtil.CallSuccess(service, result.ToString());
            }
            catch
            {
                LogUtil.CallFail(service);
                throw;
            }

            return result;
        }

        /// Fetches all the Quota remaining values from LiveMatch for a specific study
        /// </summary>
        /// <param name="studyId"></param>
        public QuotasLiveObject GetQuotaRemainingValues(int studyId)
        {
            var parameters = new Hashtable { { "data[study_id]", studyId }, { "data[quota_type]", "TP" } };
            QuotasLiveObject quotasLiveObject = new QuotasLiveObject();

            string service = "fetchMulti";
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                var result = CallQuotaLiveService("fetchMulti", parameters);

                LogUtil.CallSuccess(service, result.ToString());
                
                quotasLiveObject = ProcessQuotaLiveResponse(result);
            }
            catch
            {
                LogUtil.CallFail(service);
                throw;
            }
            return quotasLiveObject;

        }

        /// <summary>
        /// Get the quota remaining values from Live match 
        /// </summary>
        /// <param name="result"></param>
        /// <returns>QuotasLiveObject</returns>
        private QuotasLiveObject ProcessQuotaLiveResponse(XDocument result)
        {

            QuotasLiveObject quotasLiveObject = new QuotasLiveObject();

            try
            {
                quotasLiveObject = (from openSampleNode in result.Descendants("Data")
                                    select new QuotasLiveObject()
                                         {
                                             QuotasLiveList = (from QuotaLive in openSampleNode.Descendants("Item")
                                                               select new QuotaLiveObject()
                                                                   {
                                                                       InternalSampleId = XmlUtil.GetSafeIntegerNodeValue(QuotaLive.Element("SampleId")),
                                                                       InternalQuotaId = XmlUtil.GetSafeIntegerNodeValue(QuotaLive.Element("QuotaId")),
                                                                       QuotaRemaining = XmlUtil.GetSafeIntegerNodeValue(QuotaLive.Element("NumRemaining"))
                                                                   }).ToList()


                                         }).FirstOrDefault();

                if (quotasLiveObject == null)
                    return new QuotasLiveObject();

            }
            catch
            {
                throw;
            }

            return quotasLiveObject;
        }

        /// <summary>
        /// Update the quota remaining values in the GMISampleQuotasObject
        /// </summary>
        /// <param name="GMISampleQuotasObject"></param>
        /// <param name="QuotasLiveObject"></param>
        public void UpdateQuotaRemaingValues(GMISampleQuotasObject gmiSampleQuotasObject, QuotasLiveObject quotasLiveObject)
        {
            if (quotasLiveObject.QuotasLiveList == null)
                return;

            foreach (QuotaLiveObject quotaLiveObject in quotasLiveObject.QuotasLiveList)
            {
                var quota = gmiSampleQuotasObject.GMIQuotasList.Where(p => p.InternalQuotaId == quotaLiveObject.InternalQuotaId).ToList();
                if (quota.Count > 0)
                    quota[0].QuotaRemaining = quotaLiveObject.QuotaRemaining;
            }

        }

    }
}
