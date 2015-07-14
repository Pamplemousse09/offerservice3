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
    public class RespondentCatalog : IRespondentCatalog
    {
        readonly static WebClientUser UserInformation = (WebClientUser)ConfigurationManager.GetSection("WebClientUser");
        private const string NoAttributes = "No attributes";
        private const string Corebirthdate = "COREbirthdate";

        public RespondentCatalog()
        {
        }

        internal static LSR.WebClient.RespondentCatalog.RpcClient GetRespWebClient()
        {
            var p = new ConfigurationProvider(UserInformation);
            var client = new LSR.WebClient.RespondentCatalog.RpcClient(p);

            return client;
        }


        /// <summary>
        ///  Call the Respondent Catalogue webservice
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>string webservice response</returns>
        private string CallRespondentCatalogueService(string method, Hashtable parameters)
        {
            var result = string.Empty;

            string service = "RespondentCatalogueService - " + method;
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            try
            {
                result = GetRespWebClient().CallWebservice(method, parameters);
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
        /// Fetches the attributes of the respondent by pid from the respondentCatalog
        /// </summary>
        /// <param name="localPid"></param>
        /// <returns>Dictionary with the respondentAttributes</returns>
        public Dictionary<string, string> GetRespondentCatalogueAttributes(string localPid)
        {
            string service = "fetchProfile";
            var respondentCatalogueAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var parameters = new Hashtable { { "data[id_respondent]", localPid } };

            try
            {

                LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

                var result = CallRespondentCatalogueService(service, parameters);

                LogUtil.CallSuccess(service, result.ToString());
                
                if (string.IsNullOrEmpty(result))
                    return null;

                //Log.DebugFormat("Result of the request sent to " + UserInformation.Services[SupportedService.RespondentCatalog] + " : " + result);
                //Log.InfoFormat("RespondentCatalog fetchProfile Duration: " + (int)Math.Round(sw.Elapsed.TotalMilliseconds));
                var respondentCatalog = new RespondentCatalog();
                respondentCatalogueAttributes = ProcessRespondentCatalogAttributesResponse(result);

                var param = new Dictionary<string, string>();
                param.Add("localPid", localPid);
            }
            catch(Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }

            return respondentCatalogueAttributes;
        }


        /// <summary>
        /// Updates the respondent catalog with the attributes received in the provider request
        /// </summary>
        /// <param name="localPid"></param>
        /// <param name="attributes"></param>
        /// <returns>string</returns>        
        public void UpdateRespondentCatalogueAttributes(string localPid, Dictionary<string, string> attributes)
        {
            string status = "-1";

            var parameters = new Hashtable { { "data[id_respondent]", localPid } };

            string service = "RespondentCatalogueService - Update";
            LogUtil.CallingService(service, LogUtil.getHashtableString(parameters));

            var param = new Dictionary<string, string>();
            param.Add("LocalPid", localPid);
            foreach (var attribute in attributes)
            {
                param.Add(attribute.Key, attribute.Value);
            }

            foreach (KeyValuePair<string, string> attribute in attributes)
            {
                parameters.Add("data[" + attribute.Key + "]", attribute.Value);
            }

            try
            {

                var result = CallRespondentCatalogueService("update", parameters);

                LogUtil.CallSuccess(service, result.ToString());

                status = ProcessRespondentCatalogUpdateResponse(result, param);

                //Log.DebugFormat("Result of the request sent to " + UserInformation.Services[SupportedService.RespondentCatalog] + " : " + result + " :  status : " + status);
                //Log.InfoFormat("RespondentCatalog update Duration: " + (int)Math.Round(sw.Elapsed.TotalMilliseconds));
            }
            catch(Exception e)
            {
                LogUtil.CallFail(service, e);
                throw;
            }
        }

        /// <summary>
        /// Get the list of Respondent attributes from the Respondent Catalog webservice response
        /// </summary>
        /// <param name="respondentCatalogAttributesResponse"></param>
        /// <returns>Dictionary with the respondent Attributes</returns>
        public Dictionary<string, string> ProcessRespondentCatalogAttributesResponse(string respondentCatalogAttributesResponse)
        {
            var respondentCatalogueAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                //Load xml
                var xdoc = new XmlUtil().ParseXmlDocument(respondentCatalogAttributesResponse);

                respondentCatalogueAttributes = xdoc.Descendants("Item")
                    .Select(x =>
                    {
                        var xElement = x.Element("Ident");

                        return xElement != null ? new
                        {
                            name = xElement.Value,
                            id = ((x.Element("Value") != null && string.Equals(xElement.Value, Corebirthdate, StringComparison.OrdinalIgnoreCase) && x.Element("Value").Value.Length >= 10) ? x.Element("Value").Value.Substring(0, 10) : x.Element("Value").Value),
                        } :

                                     new
                                     {
                                         name = NoAttributes,
                                         id = x.Element("Message").Value
                                     }
                                     ;
                    })
                    .Where(x => x.name != null)
                    .ToDictionary(x => x.name, x => x.id);


                if (respondentCatalogueAttributes.ContainsKey(NoAttributes))
                    return new Dictionary<string, string>();

            }
            catch
            {
            }
            return respondentCatalogueAttributes;
        }

        /// <summary>
        /// Get the update status when modifiying attributes in the Respondent Catalog 
        /// </summary>
        /// <param name="respondentCatalogUpdateResponse"></param>
        /// <returns>string : Update status</returns>
        public string ProcessRespondentCatalogUpdateResponse(string respondentCatalogUpdateResponse, Dictionary<string, string> param = null)
        {
            string status = "-1";

            try
            {
                //Load xml
                var xmldoc = new XmlUtil().LoadXmlDocument(respondentCatalogUpdateResponse);

                XmlNodeList nodeList = xmldoc.GetElementsByTagName("Status");
                string callStatus = "-1";
                foreach (XmlNode node in nodeList)
                {
                    callStatus = node.InnerText;
                }
                if (callStatus.Equals(""))
                {
                    nodeList = xmldoc.GetElementsByTagName("Code");
                    string code = "";
                    foreach (XmlNode node in nodeList)
                    {
                        code = node.InnerText;
                    }

                    nodeList = xmldoc.GetElementsByTagName("Message");
                    string message = "";
                    foreach (XmlNode node in nodeList)
                    {
                        message = node.InnerText;
                    }
                    var errorMessage = "Failed to update the respondent attributes. Error Code: " + code + " Message: " + message;

                    return "Failed to update the respondent attributes. Error Code: " + code + " Message: " + message;
                }

                foreach (XmlNode node in nodeList)
                {
                    status = node.InnerText;
                }
            }
            catch
            {
            }
            return status;
        }
    }
}
