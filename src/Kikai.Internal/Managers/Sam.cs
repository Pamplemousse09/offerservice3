using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.IManagers;
using Kikai.Internal.Utils;
using log4net;
using LSR.WebClient;
using LSR.WebClient.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace Kikai.Internal.Managers
{
    public class Sam : ISam
    {

        public Sam()
        {
        }

        internal static LSR.WebClient.SampleManager.RpcClient GetSamWebClient()
        {
            WebClientUser userInformation = (WebClientUser)ConfigurationManager.GetSection("WebClientUser");
            ConfigurationProvider p = new ConfigurationProvider(userInformation);
            LSR.WebClient.SampleManager.RpcClient client
                = new LSR.WebClient.SampleManager.RpcClient(p);

            return client;
        }

        /// <summary>
        /// Calls the GetMainstreamStudySamples service call and returns the study Info as XmlDocument
        /// </summary>
        /// <param name="studyId"></param>
        /// <returns></returns>
        public XDocument GetMainstreamStudySamples(int studyId)
        {
            var result = string.Empty;

            string service = "GetMainstreamStudySamples";
            LogUtil.CallingService(service, studyId.ToString());

            try
            {
                result = GetSamWebClient().GetMainstreamStudySamples(studyId);
                LogUtil.CallSuccess(service, result.ToString());
            }
            catch (Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }

            return new XmlUtil().ParseXmlDocument(result);
        }

        /// <summary>
        /// Calls the GetOpenSampleAttributes service call and returns the sample attributes as XmlDocument
        /// </summary>
        /// <param name="sampleId"></param>
        /// <returns></returns>
        public XDocument GetOpenSampleAttributes(string sampleList)
        {
            var result = string.Empty;

            string service = "GetOpenSampleAttributes";
            LogUtil.CallingService(service, sampleList);

            try
            {
                result = GetSamWebClient().GetOpenSampleAttributes(sampleList);
                LogUtil.CallSuccess(service, result);
            }
            catch
            {
                LogUtil.CallFail(service);
                throw;
            }

            return new XmlUtil().ParseXmlDocument(result);
        }

        /// <summary>
        /// Gets a list of the errors returned by the service
        /// </summary>
        /// <param name="serviceResponse"></param>
        /// <returns></returns>
        public IEnumerable<ServiceErrorObject> GetServiceErrors(XDocument serviceResponse)
        {
            try
            {
                return from openSampleNode in serviceResponse.Descendants("Item")
                       select new ServiceErrorObject()
                       {
                           code = XmlUtil.GetSafeStringNodeValue(openSampleNode.Element("Code")),
                           message = XmlUtil.GetSafeStringNodeValue(openSampleNode.Element("Message"))
                       };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the list of the Open Samples available for mainstream
        /// </summary>
        /// <param name="serviceResponse"></param>
        /// <returns></returns>
        public IEnumerable<MainstreamStudySampleObject> GetMainstreamStudySampleResponse(XDocument serviceResponse)
        {
            return from openSampleNode in serviceResponse.Descendants("MainstreamStudySample")
                   select new MainstreamStudySampleObject()
                   {
                       StudyId = XmlUtil.GetSafeIntegerNodeValue(openSampleNode.Element("StudyId")),
                       SampleId = XmlUtil.GetSafeIntegerNodeValue(openSampleNode.Element("SampleId")),
                       SampleName = XmlUtil.GetSafeStringNodeValue(openSampleNode.Element("SampleName")),
                       MainstreamPercentage = XmlUtil.GetSafeIntegerNodeValue(openSampleNode.Element("MainstreamPercentage")),
                       StudyStartDate = XmlUtil.GetSafeDateTime((string)openSampleNode.Element("StudyStartDate")),
                       StudyEndDate = XmlUtil.GetSafeDateTime((string)openSampleNode.Element("StudyEndDate")),
                       OverallQuota = XmlUtil.GetSafeIntegerNodeValue(openSampleNode.Element("OverallQuota")),
                       OverallCompletes = XmlUtil.GetSafeIntegerNodeValue(openSampleNode.Element("OverallCompletes")),
                       RR = XmlUtil.GetSafeFloatNodeValue(openSampleNode.Element("RR")),
                       IR = XmlUtil.GetSafeFloatNodeValue(openSampleNode.Element("IR")),
                       CR = XmlUtil.GetSafeFloatNodeValue(openSampleNode.Element("CR")),
                       LOI = XmlUtil.GetSafeIntegerNodeValue(openSampleNode.Element("StudyLOI"))
                   };
        }

        /// <summary>
        /// Gets the Sample properties and its correspondant attributes
        /// </summary>
        /// <param name="serviceResponse"></param>
        /// <returns></returns>
        public IEnumerable<OpenSampleObject> GetOpenSampleAttributeResponse(XDocument serviceResponse)
        {
            return from openSample in serviceResponse.Descendants("OpenSample")
                   select new OpenSampleObject()
                   {
                       StudyId = XmlUtil.GetSafeIntegerNodeValue(openSample.Element("StudyId")),
                       StudyName = XmlUtil.GetSafeStringNodeValue(openSample.Element("StudyName")),
                       SampleId = XmlUtil.GetSafeIntegerNodeValue(openSample.Element("SampleId")),
                       SampleName = XmlUtil.GetSafeStringNodeValue(openSample.Element("SampleName")),
                       IR = XmlUtil.GetSafeFloatNodeValue(openSample.Element("IR")),
                       MainstreamQuotaRemaining = XmlUtil.GetSafeIntegerNodeValue(openSample.Element("MainstreamQuotaRemaining")),
                       Attributes = (from openSampleAttributes in openSample.Descendants("Attributes")
                                     from openSampleAttribute in openSampleAttributes.Descendants("Attribute")
                                     select new AttributeValuesObject()
                                     {
                                         Ident = XmlUtil.GetSafeStringNodeValue(openSampleAttribute.Element("Ident")),
                                         Values = XmlUtil.GetSafeStringNodeValue(openSampleAttribute.Element("Value"))
                                     }).ToList()
                   };
        }

        /// <summary>
        /// This function gets the list of errors and the list of Open samples and returns the list of open samples
        /// </summary>
        /// <param name="studyId"></param>
        /// <returns>List of open samples. If there's an error return empty list.</returns>
        public MainstreamStudySampleResponseObject ProcessGetMainstreamStudySample(int studyId)
        {
            ISam samServiceCall = new Sam();
            XDocument serviceResponse = samServiceCall.GetMainstreamStudySamples(studyId);

            MainstreamStudySampleResponseObject mainstreamStudySampleResponse = new MainstreamStudySampleResponseObject();
            mainstreamStudySampleResponse.MainstreamStudySamples = new List<MainstreamStudySampleObject>();
            mainstreamStudySampleResponse.Errors = samServiceCall.GetServiceErrors(serviceResponse);

            //If no error were returned get the list of open samples
            if (mainstreamStudySampleResponse.Errors.Count() == 0)
            {
                mainstreamStudySampleResponse.MainstreamStudySamples = samServiceCall.GetMainstreamStudySampleResponse(serviceResponse);
            }

            return mainstreamStudySampleResponse;
        }

        /// <summary>
        /// This function gets the list of errors and the open sample description with its correspondant attributes
        /// </summary>
        /// <param name="sampleId"></param>
        /// <returns>returns the list of attributes and the details of the sample</returns>
        public OpenSampleAttributeResponseObject ProcessGetOpenSampleAttributes(string sampleList)
        {
            try
            {
                ISam samServiceCall = new Sam();
                XDocument serviceResponse = samServiceCall.GetOpenSampleAttributes(sampleList);

                OpenSampleAttributeResponseObject openSampleAttributeResponse = new OpenSampleAttributeResponseObject();
                openSampleAttributeResponse.OpenSampleAttributeList = new List<OpenSampleObject>();
                openSampleAttributeResponse.ServiceErrorList = samServiceCall.GetServiceErrors(serviceResponse);

                //If no error were returned get the list of attributes with the details of the sample
                if (openSampleAttributeResponse.ServiceErrorList.Count() == 0)
                {
                    openSampleAttributeResponse.OpenSampleAttributeList = samServiceCall.GetOpenSampleAttributeResponse(serviceResponse);
                }

                return openSampleAttributeResponse;
            }
            catch
            {
                throw;
            }
        }
    }
}
