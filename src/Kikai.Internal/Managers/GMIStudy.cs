using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
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
using System.Xml.Linq;

namespace Kikai.Internal.Managers
{
    public class GMIStudy : IGMIStudy
    {
        readonly static WebClientUser UserInformation = (WebClientUser)ConfigurationManager.GetSection("WebClientUser");

        public GMIStudy()
        {
        }
        
        /// <summary>
        /// Creates a GMIStudyClient that will call the GMIStudy service
        /// </summary>
        /// <returns></returns>
        internal static LSR.WebClient.GMIStudy.RpcClient GetGMIStudyClient()
        {
            var p = new ConfigurationProvider(UserInformation);
            var client = new LSR.WebClient.GMIStudy.RpcClient(p);
            return client;
        }

        /// <summary>
        /// Call the GMI Study webservice
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>XDocument webservice response</returns>
        private XDocument CallGMIStudyService(string method, Hashtable parameters)
        {
            XDocument result = null;
            var gMIStudyClient = GetGMIStudyClient();

            string service = "GMI StudyService - " + method;
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                result = new XmlUtil().ParseXmlDocument(gMIStudyClient.CallWebservice(method, parameters));
                LogUtil.CallSuccess(service, result.ToString());
            }
            catch(Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }

            return result;
        }

        /// Fetches the samples and their quotas from GMI service for a specific study
        /// </summary>
        /// <param name="studyId"></param>
        public GMISampleQuotasObject GetGMISamples(int studyId, int sampleId)
        {
            var parameters = new Hashtable { { "data[study_id]", studyId } };
            GMISampleQuotasObject gmiSampleQuotasObject = new GMISampleQuotasObject();

            var service = "GMI getSamples";
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                var result = CallGMIStudyService("getSamples", parameters);
                LogUtil.CallSuccess(service, result.ToString());
                gmiSampleQuotasObject = ProcessGMIStudyResponse(result, sampleId);
            }
            catch(Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }
            return gmiSampleQuotasObject;
        }

        /// <summary>
        /// Get the externals/internals Quotas and samples from the GMI service response
        /// </summary>
        /// <param name="result"></param>
        /// <returns>GMISampleQuotasObject</returns>
        private GMISampleQuotasObject ProcessGMIStudyResponse(XDocument result, int sampleId)
        {

            GMISampleQuotasObject gmiSampleQuotasObject = new GMISampleQuotasObject();
            List<GMISampleObject> gmiSampleQuotasList = new List<GMISampleObject>();
            List<GMISampleObject> gmiSamplesList = new List<GMISampleObject>();
            List<GMIQuotaObject> gmiQuotasList = new List<GMIQuotaObject>();

            try
            {

                gmiSampleQuotasObject = (from openSampleNode in result.Descendants("Samples")
                                         select new GMISampleQuotasObject()
                                                   {
                                                       GMISampleQuotasList = (from QuotaAttributes in openSampleNode.Descendants("Item")
                                                                              where 0 != XmlUtil.GetSafeIntegerNodeValue(QuotaAttributes.Element("Id"))
                                                                                 && 0 != XmlUtil.GetSafeIntegerNodeValue(QuotaAttributes.Element("ExternalId"))
                                                                              select new GMISampleObject()
                                                                              {
                                                                                  InternalSampleId = XmlUtil.GetSafeIntegerNodeValue(QuotaAttributes.Element("Id")),
                                                                                  SampleId = XmlUtil.GetSafeIntegerNodeValue(QuotaAttributes.Element("ExternalId"))
                                                                              }).ToList(),
                                                       GMIQuotasList = (from QuotaAttributes in openSampleNode.Descendants("QuotaCells")
                                                                        from QuotaAttribute in QuotaAttributes.Descendants("Item")
                                                                        where 0 != XmlUtil.GetSafeIntegerNodeValue(QuotaAttribute.Element("Id"))
                                                                          && 0 != XmlUtil.GetSafeIntegerNodeValue(QuotaAttribute.Element("ExternalId"))
                                                                        select new GMIQuotaObject()
                                                                        {
                                                                            InternalQuotaId = XmlUtil.GetSafeIntegerNodeValue(QuotaAttribute.Element("Id")),
                                                                            QuotaId = XmlUtil.GetSafeIntegerNodeValue(QuotaAttribute.Element("ExternalId"))
                                                                        }).ToList()

                                                   }).FirstOrDefault();

                if (gmiSampleQuotasObject == null)
                    return new GMISampleQuotasObject();

                gmiQuotasList = gmiSampleQuotasObject.GMIQuotasList;
                gmiSampleQuotasList = gmiSampleQuotasObject.GMISampleQuotasList;
                int i = -1;

                foreach (GMISampleObject gmiSampleObject in gmiSampleQuotasList)
                {
                    var quota = gmiQuotasList.Where(p => p.QuotaId == gmiSampleObject.SampleId).ToList();

                    if (quota.Count > 0)
                    {
                        if (gmiSamplesList.Count > 0)
                            quota[0].SampleId = gmiSamplesList[i].SampleId;
                    }
                    else
                    {
                        i++;
                        gmiSamplesList.Add(new GMISampleObject(gmiSampleObject.SampleId, gmiSampleObject.InternalSampleId));
                    }
                }
                gmiSampleQuotasObject.GMISampleQuotasList = gmiSamplesList.Where(s => s.SampleId == sampleId).ToList();
                gmiSampleQuotasObject.GMIQuotasList = gmiQuotasList.Where(p => p.SampleId == sampleId).ToList();
            }
            catch
            {
                throw;
            }
            return gmiSampleQuotasObject;
        }
    }
}
