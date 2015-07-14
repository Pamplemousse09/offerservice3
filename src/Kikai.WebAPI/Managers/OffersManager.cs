using Kikai.BL.Concrete;
using Kikai.BL.DTO;
using Kikai.BL.DTO.ApiObjects;
using Kikai.Internal.Managers;
using Kikai.Domain.Common;
using Kikai.WebApi.Managers.IManagers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kikai.Internal.Contracts.Objects;
using Kikai.Logging.Extensions;
using Kikai.BL.IRepository;
using Kikai.Logging.DTO;
using Kikai.Logging.Utils;
using Kikai.WebApi.DTO;
using Newtonsoft.Json;
using Kikai.WebApi.Decorators;
using System.Xml;
using Kikai.Internal.IManagers;
using Newtonsoft.Json.Linq;

namespace Kikai.WebApi.Managers
{
    public class OffersManager : IOffersManager
    {
        #region Constants
        private const string DocumentNotFound = "404";
        private readonly string OperationType = operationType.WS.ToString();
        private const string Corebirthdate = "COREbirthdate";
        private const string Coreage = "COREage";
        #endregion

        #region Private Variables
        private IOfferRepository IOfferRepository;
        private IOfferAttributeRepository IOfferAttributeRepository;
        private IProviderRepository IProviderRepository;
        private IAttributeRepository IAttributeRepository;
        private ILiveMatch ILiveMatch;
        private IRespondentCatalog IRespondentCatalog;
        private IQuotaExpressionRepository IQuotaExpressionRepository;
        private IQuotaMappingRepository IQuotaMappingRepository;
        private ISampleMappingRepository ISampleMappingRepository;
        private IGMIStudy IGMIStudy;
        private ISteamStudy ISteamStudy;
        private IQuotaLiveMatch IQuotaLiveMatch;
        private bool xunit = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public OffersManager()
        {
        }

        /// <summary>
        /// Xunit Constructor
        /// </summary>
        /// <param name="IOfferRepository"></param>
        /// <param name="IOfferAttributeRepository"></param>
        /// <param name="IProviderRepository"></param>
        /// <param name="IAttributeRepository"></param>
        /// <param name="ILiveMatch"></param>
        /// <param name="IRespondentCatalog"></param>
        public OffersManager(
            IOfferRepository IOfferRepository,
            IOfferAttributeRepository IOfferAttributeRepository,
            IProviderRepository IProviderRepository,
            IAttributeRepository IAttributeRepository,
            ILiveMatch ILiveMatch,
            IRespondentCatalog IRespondentCatalog,
            IQuotaExpressionRepository IQuotaExpressionRepository,
            IQuotaMappingRepository IQuotaMappingRepository,
            ISampleMappingRepository ISampleMappingRepository,
            IGMIStudy IGMIStudy,
            ISteamStudy ISteamStudy,
            IQuotaLiveMatch IQuotaLiveMatch
            )
        {
            //log = LoggerFactory.GetLogger("Xunit");
            //detailedLog = LoggerFactory.GetLogger("Xunit");
            this.xunit = true;
            this.IOfferRepository = IOfferRepository;
            this.IOfferAttributeRepository = IOfferAttributeRepository;
            this.IProviderRepository = IProviderRepository;
            this.IAttributeRepository = IAttributeRepository;
            this.ILiveMatch = ILiveMatch;
            this.IRespondentCatalog = IRespondentCatalog;
            this.IQuotaExpressionRepository = IQuotaExpressionRepository;
            this.IQuotaMappingRepository = IQuotaMappingRepository;
            this.ISampleMappingRepository = ISampleMappingRepository;
            this.IGMIStudy = IGMIStudy;
            this.ISteamStudy = ISteamStudy;
            this.IQuotaLiveMatch = IQuotaLiveMatch;
        }
        #endregion

        #region RepositoryCalls
        private IOfferRepository OfferRepository()
        {
            if (xunit)
                return IOfferRepository;
            else
                return new OfferRepository();
        }

        private IOfferAttributeRepository OfferAttributeRepository()
        {
            if (xunit)
                return IOfferAttributeRepository;
            else
                return new OfferAttributeRepository();
        }

        private IProviderRepository ProviderRepository()
        {
            if (xunit)
                return IProviderRepository;
            else
                return new ProviderRepository();
        }

        private IAttributeRepository AttributeRepository()
        {
            if (xunit)
                return IAttributeRepository;
            else
                return new AttributeRepository();
        }

        private IQuotaExpressionRepository QuotaExpressionRepository()
        {
            if (xunit)
                return IQuotaExpressionRepository;
            else
                return new QuotaExpressionRepository();
        }

        private IQuotaMappingRepository QuotaMappingRepository()
        {
            if (xunit)
                return IQuotaMappingRepository;
            else
                return new QuotaMappingRepository();
        }

        private ISampleMappingRepository SampleMappingRepository()
        {
            if (xunit)
                return ISampleMappingRepository;
            else
                return new SampleMappingRepository();
        }
        #endregion

        #region WebServicesCalls
        private ILiveMatch LiveMatch()
        {
            if (xunit)
                return ILiveMatch;
            else
                return new LiveMatch();
        }

        private IRespondentCatalog RespondentCatalog()
        {
            if (xunit)
                return IRespondentCatalog;
            else
                return new RespondentCatalog();
        }

        private IGMIStudy GMIStudy()
        {
            if (xunit)
                return IGMIStudy;
            else
                return new GMIStudy();
        }

        private ISteamStudy SteamStudy()
        {
            if (xunit)
                return ISteamStudy;
            else
                return new SteamStudy();
        }

        private IQuotaLiveMatch QuotaLiveMatch()
        {
            if (xunit)
                return IQuotaLiveMatch;
            else
                return new QuotaLiveMatch();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get the attributes from the request sent by the provider
        /// </summary>
        /// <returns>Dictionary of the request Attributes</returns>
        public Dictionary<string, string> GetRequestAttributes(HttpRequestMessage Request)
        {
            var requestAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (var parameter in Request.GetQueryNameValuePairs())
                {
                    var attributeName = String.Empty;
                    if ((attributeName = ValidateRespondentAttribute(parameter.Key)) != null)
                        requestAttributes.Add(attributeName, parameter.Value);
                }
            }
            catch
            {
                throw;
            }

            return requestAttributes;
        }

        /// <summary>
        /// Check if Offer Service Request contains the required attributes Country and Language
        /// </summary>
        /// <param name="respondentAttributes"></param>
        /// <returns>return boolean </returns>
        public bool VerifyRequiredAttributes(Dictionary<string, string> respondentAttributes)
        {
            var requiredAttributes = new List<AttributeObject>();

            try
            {
                requiredAttributes = AttributeRepository().SelectRequiredAttributes().ToList();

                if (respondentAttributes.Count <= 1 || requiredAttributes.Count == 0)
                    return false;

                foreach (var requiredAttribute in requiredAttributes)
                {
                    var value = string.Empty;
                    if (respondentAttributes.TryGetValue(requiredAttribute.Id, out value))
                    {
                        if (string.IsNullOrEmpty(value) || value.Contains("\""))
                            return false;
                    }
                }
            }
            catch
            {
                throw;
            }

            var respondentAttributesHashset = new HashSet<string>(respondentAttributes.Select(x => x.Key));
            var requiredAttributesHashset = new HashSet<string>(requiredAttributes.Select(x => x.Id));

            return requiredAttributesHashset.IsSubsetOf(respondentAttributesHashset);
        }


        /// <summary>
        /// Get the offers filtered by a given list of attributes in order to be displayed on the offer wall.
        /// </summary>
        /// <param name="filterAttributes"></param>
        /// <returns>return list of active offers suitable for displaying on an offer wall filtered by request attributes.</returns>
        private List<OfferApiObject> RetrieveOffers(Dictionary<string, string> filterAttributes, string ApiUser, OfferFilter offerFilter)
        {
            List<OfferApiObject> offers = null;
            try
            {
                if (offerFilter.Equals(OfferFilter.LiveOffer))
                    offers = OfferRepository().GetActiveOffersHavingValidTerm(false).ToList();
                else if (offerFilter.Equals(OfferFilter.TestOffer))
                    offers = OfferRepository().GetActiveOffersHavingValidTerm(true).ToList();
                else
                    offers = OfferRepository().GetActiveOffersHavingValidTerm().ToList();

                if (offers != null && offers.Count > 0)
                    offers = FilterOffers(offers, filterAttributes);
                else
                    return new List<OfferApiObject>();

                string country = string.Empty;

                if (filterAttributes.TryGetValue("COREcontact_country", out country) == false)
                {
                    country = DefaultCountryCode();
                }

                offers = UpdateOffersLink(offers, ApiUser, country);
            }
            catch
            {
                throw;
            }

            return offers;
        }

        public List<OfferApiObject> RetrieveAllOffers(Dictionary<string, string> filterAttributes, string ApiUser)
        {
            return RetrieveOffers(filterAttributes, ApiUser, OfferFilter.AnyOffer);
        }

        public List<OfferApiObject> RetrieveLiveOffers(Dictionary<string, string> filterAttributes, string ApiUser)
        {
            return RetrieveOffers(filterAttributes, ApiUser, OfferFilter.LiveOffer);
        }

        public List<OfferApiObject> RetrieveTestOffers(Dictionary<string, string> filterAttributes, string ApiUser)
        {
            return RetrieveOffers(filterAttributes, ApiUser, OfferFilter.TestOffer);
        }

        /// <summary>
        /// Get the offers filtered by a given list of attributes
        /// </summary>
        /// <param name="liveOffers"></param>
        /// <param name="filterAttributes"></param>
        /// <returns>return list of active offers suitable for displaying on an offer wall filtered by request attributes.</returns>
        public List<OfferApiObject> FilterOffers(List<OfferApiObject> liveOffers, Dictionary<string, string> respondentAttributes)
        {
            var returnedOffers = new List<OfferApiObject>();
            var sampleAttributes = new List<OfferAttributeApiObject>();
            try
            {

                foreach (OfferApiObject offer in liveOffers)
                {
                    sampleAttributes = OfferAttributeRepository().GetOfferAttributes(offer.OfferID).ToList();
                    bool addOffer = true;
                    foreach (KeyValuePair<string, string> kv in respondentAttributes)
                    {
                        OfferAttributeApiObject nonmatchingAttribute = sampleAttributes.Find(x => x.Name.Equals(kv.Key, StringComparison.OrdinalIgnoreCase) && !Match(kv.Value, x.Value));
                        if (nonmatchingAttribute != null)
                        {
                            // non matching attribute detected, exclude offer
                            addOffer = false;

                            break;
                        }
                    }

                    if (addOffer)
                    {
                        returnedOffers.Add(offer);
                    }
                }
            }
            catch
            {
                throw;
            }

            return returnedOffers;
        }

        /// <summary>
        ///    matches an attribute value, to attributes values definition (values).
        ///    The values can be exact value, list of values comma separate, or a range separated by dash.
        /// </summary>
        /// <param name="value">value to match</param>
        /// <param name="values">value definition comma separated values (range is defined by AND)</param>
        /// <returns>true if matched, false if not matched</returns>
        private bool Match(string value, string values)
        {
            bool result = false;
            try
            {
                Char[] segment_separator = { ',' };
                string[] segments = values.Split(segment_separator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; (i < segments.Length && result == false); ++i)
                {
                    string segment = segments[i].Trim();
                    if (segment.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        result = true;
                    }
                    else if (segment.ToUpper().Contains("AND"))
                    { // defines a range
                        string[] separators = { "AND", "and", "And", "ANd", "aNd", "aND", "anD" };
                        string[] ranges = segment.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        float left = float.Parse(ranges[0]);
                        float right = float.Parse(ranges[1]);
                        if (!String.IsNullOrEmpty(value))
                        {
                            float fv = float.Parse(value);
                            if (left <= right && left <= fv && right >= fv)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        ///   Return US country code for now. 
        /// </summary>
        /// <returns>165</returns>
        private static string DefaultCountryCode()
        {
            return "165";
        }

        /// <summary>
        /// Get the list of studies for a given list of offers
        /// </summary>
        /// <param name="offers"></param>
        /// <returns>the list of studies for a given list of offers </returns>
        /// 
        public List<int> GetOffersStudiesList(List<OfferApiObject> offers)
        {
            string joinedOffers = string.Empty;
            List<StudyOfferObject> studies = new List<StudyOfferObject>();
            var offerList = offers.Select(l => l.OfferID).ToList();
            List<int> studyIds = new List<int>();
            joinedOffers = string.Join(",", offerList.Select(x => string.Format("'{0}'", x)));
            try
            {
                studies = OfferRepository().GetStudyIdsFromOfferIds(joinedOffers).ToList();
            }
            catch
            {
                throw;
            }

            //Add the study id's from the StudyOfferObject into the studyIds list
            foreach (var study in studies)
            {
                studyIds.Add(study.StudyId);
            }

            return studyIds;
        }

        /// <summary>
        ///  Uri parameter is validated and mapped to respondent catalog attribute name.
        /// </summary>
        /// <param name="param">attribute from Uri</param>
        /// <returns>respondent catalog attribute name of valid, null if invalid</returns>
        private string ValidateRespondentAttribute(string attribute)
        {
            AttributeObject RequestAttribute = null;
            string RequestAttributeId = null;
            try
            {
                RequestAttribute = AttributeRepository().SelectByID(attribute);
                if (RequestAttribute != null)
                    RequestAttributeId = RequestAttribute.Id;
            }
            catch
            {
                throw;
            }

            return RequestAttributeId;
        }

        /// <summary>
        /// Get the list of excluded studies for given respondent internal ID and provider ID filtered by activities
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="offers"></param>
        /// <returns>Return List of excluded studies</returns>
        public List<string> FetchNonPanelistStudies(List<OfferApiObject> offers, string internalPid, string requestId)
        {
            var filteredOffers = new List<OfferApiObject>();
            var studiesList = new List<string>();

            List<int> studies = GetOffersStudiesList(offers);
            var parameters = new Hashtable();
            parameters.Add("nonGtmId", internalPid);

            if (studies != null && studies.Count > 0)
                parameters.Add("studyid", string.Join(",", studies));

            try
            {

                var result = LiveMatch().CallLiveMatchService("fetchNonPanelistActivity", parameters);
                studiesList = LiveMatch().ProcessLiveMatchStudiesActivityResponse(result.InnerXml);

                //Log.DebugFormat("Result of the request sent to " + UserInformation.Services[SupportedService.LiveMatch] + " : " + result);
                //Log.InfoFormat("LiveMatch fetchNonPanelistActivity Duration: " + (int)Math.Round(sw.Elapsed.TotalMilliseconds));

                var param = new Dictionary<string, string>();
                param.Add("InternalPID", internalPid);
                var counter = 1;
                foreach (var offer in offers)
                {
                    param.Add("OfferId#" + counter, offer.OfferID.ToString());
                    counter++;
                }
            }
            catch
            {
                throw;
            }
            return studiesList;
        }

        /// <summary>
        /// Get the list of offers filtered by a given List of studies
        /// </summary>
        /// <param name="liveMatchStudiesActivityResponse"></param>
        /// <returns>Return the list of offers filtered by a given List of studies </returns>
        /// 
        public List<OfferApiObject> RetrieveExludedOffers(List<string> studiesList, List<OfferApiObject> offers)
        {
            string studies = string.Join(",", studiesList);
            List<OfferApiObject> offerList = new List<OfferApiObject>();
            List<Guid> offersGuids = new List<Guid>();
            try
            {
                var offersGuidsResponse = OfferRepository().GetOfferIdsFromStudyIds(studies).ToList();
                foreach (var offersGuid in offersGuidsResponse)
                {
                    offersGuids.Add(offersGuid.OfferId);
                }
                if (offersGuids.ToList().Count > 0)
                {
                    foreach (OfferApiObject offer in offers)
                    {
                        if (!offersGuids.Contains(offer.OfferID))
                            offerList.Add(offer);
                    }
                }
            }
            catch
            {
                throw;
            }

            return offerList;
        }

        public List<OfferApiObject> UpdateOffersLink(List<OfferApiObject> offers, string providerId, string countryCode = null)
        {
            try
            {
                ProviderObject provider = ProviderRepository().SelectByProviderId(providerId);
                if (countryCode == null)
                    countryCode = DefaultCountryCode();
                foreach (var offer in offers)
                {
                    if (provider != null) offer.updateLink(DefaultCountryCode(), provider.WelcomeURLCode);
                }
            }
            catch
            {
                throw;
            }
            return offers;
        }

        /// <summary>
        /// Get the age from the COREbirthdate attribute
        /// </summary>
        /// <param name="birthdate"></param>
        /// <returns>int age</returns>
        private int GenerateAge(string birthdate)
        {
            int age = 0;

            try
            {
                DateTime birthDateTime = DateTime.Parse(birthdate);
                age = DateTime.Now.Year - birthDateTime.Year;

                if (DateTime.Now.Month < birthDateTime.Month || (DateTime.Now.Month == birthDateTime.Month && DateTime.Now.Day < birthDateTime.Day))
                {
                    age--;
                }
            }
            catch
            {
                throw;
            }

            return age;
        }

        /// <summary>
        ///   It processes the attributes (form HTTP request). 
        ///   The attributes are augmented by any attribute from respondent catalog if they are missing.
        ///   If any new attribute detected in request attributes that is different or non-existent in
        ///   respondent catalog, the attributes will be added.
        /// </summary>
        /// <param name="attributes">List of attributes from request, it may be updated from respondent catalog</param>
        /// <param name="InternalPid">internal persistent id of the respondent</param>
        private bool AddRespondentCatalogAttributes(Dictionary<string, string> attributes, string internalPid)
        {
            //Get the internal respondent ID from the respondent catalog given the PID and Provider ID
            var respondentAttributes = new Dictionary<string, string>();
            try
            {
                respondentAttributes = RespondentCatalog().GetRespondentCatalogueAttributes(internalPid);

                if (respondentAttributes == null)
                    return false;


                string birthValue = string.Empty;
                if (attributes.TryGetValue(Corebirthdate, out birthValue) && !attributes.ContainsKey(Coreage))
                {
                    attributes.Add(Coreage, GenerateAge(birthValue).ToString());
                }

                foreach (KeyValuePair<string, string> raKv in respondentAttributes)
                {
                    // only update attributes if they are not present in the request attributes
                    if (!attributes.ContainsKey(raKv.Key))
                    {
                        //Log.DebugFormat("  add respondent attribute from catalog: {0}={1}", raKv.Key, raKv.Value);
                        attributes.Add(raKv.Key, raKv.Value);
                    }
                    else
                    {
                        //Log.DebugFormat("  skip respondent attribute from catalog: {0}={1} already in request", raKv.Key, raKv.Value);
                    }
                }

                // Any request parameter representing respondent attribute is checked for catalog update
                var updateAttributes = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> kv in attributes)
                {
                    string value = string.Empty;

                    if (!respondentAttributes.TryGetValue(kv.Key, out value) || !string.Equals(kv.Value, value, StringComparison.OrdinalIgnoreCase))
                    {
                        updateAttributes.Add(kv.Key, kv.Value);
                    }
                }

                if (updateAttributes.Count > 0)
                {
                    RespondentCatalog().UpdateRespondentCatalogueAttributes(internalPid, updateAttributes);
                }
            }
            catch
            {
                throw;
            }
            return true;
        }

        /// <summary>
        /// Function that will return offers depending on the request (LiveOffers or TestOffers) without PID
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="ApiUser"></param>
        /// <returns></returns>
        public OffersDataObject GetOffers(HttpRequestMessage Request, string ApiUser)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            OffersDataObject data = new OffersDataObject();
            String requestId = Request.Properties["requestId"].ToString();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            OfferFilter OfferType = GetOfferType(Request.GetRouteData().Values["controller"].ToString());
            string OperationName = (OfferType.Equals(OfferFilter.TestOffer)) ? operationName.GetAllTestOffers.ToString() : operationName.GetAllLiveOffers.ToString();

            log.ProcessingDebug(requestId, "Received GET " + OfferType + "s request.");
            try
            {
                log.ProcessingDebug(requestId, "Getting request attributes from URL.");
                Dictionary<string, string> respondentAttributes = GetRequestAttributes(Request);
                foreach (var respondentAttribute in respondentAttributes)
                {
                    parameters.Add(respondentAttribute.Key, respondentAttribute.Value);
                }
                log.ProcessingDebug(requestId, "Getting " + OfferType + "s from database.");
                var offers = RetrieveOffers(respondentAttributes, ApiUser, OfferType);
                //Check if active offers count is greater than 0 fill the response envelope data
                if (offers != null && offers.Count() > 0)
                {
                    //Filling the data object in the response envelope with the LiveOffers list
                    data.Offers = offers;
                }

                else
                {
                    if (respondentAttributes.Count == 0)
                        data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_NO_AVAILABLE_OFFER));
                    else
                        data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_NO_MATCHING_OFFERS));
                }
            }
            catch (Exception e)
            {
                data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                log.InfoJson(new Methods().Exception_ToLogObject(requestId, ApiUser, OperationType, OperationName, e));
            }
            finally
            {
                //If the response has errors, insert an error message into the logs
                //Edit for R184
                if (data.Errors.Count != 0)
                {
                    log.InfoJson(new Methods().Error_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data.Errors));
                    log.ProcessingDebug(requestId, "GET " + OfferType + "s request was unsuccessful.");
                }

                //The response has no errors, we insert a request successful message into the logs
                else
                {
                    log.InfoJson(new Methods().Response_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data.Offers));
                    log.ProcessingDebug(requestId, "GET " + OfferType + "s request was successful.");
                }
            }
            return data;
        }

        /// <summary>
        /// Function that will return offers depending on the request (LiveOffers or TestOffers) with PID
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="ApiUser"></param>
        /// <param name="pid"></param>
        /// <param name="respondentAttributes"></param>
        /// <returns></returns>
        public OffersDataObject GetOffersByPid(HttpRequestMessage Request, string ApiUser, string pid)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            OffersDataObject data = new OffersDataObject();
            String requestId = Request.Properties["requestId"].ToString();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            OfferFilter OfferType = GetOfferType(Request.GetRouteData().Values["controller"].ToString());
            string OperationName = operationName.GetOffersPID.ToString();
            int errorKey = ErrorKey.ERR_INTERNAL_FATAL;
            parameters.Add("RespondentId", pid);
            log.ProcessingDebug(requestId, "Received GET " + OfferType + "s with pid request.");
            try
            {
                var excludedOffers = new List<Guid>();
                log.ProcessingDebug(requestId, "Getting request attributes from URL.");
                Dictionary<string, string> respondentAttributes = GetRequestAttributes(Request);
                foreach (var respondentAttribute in respondentAttributes)
                {
                    parameters.Add(respondentAttribute.Key, respondentAttribute.Value);
                }
                if (VerifyRequiredAttributes(respondentAttributes))
                {
                    // handle the PID attribute, get/update attributes in respondent catalog

                    // Before each internal web service call we have to set the error key to the PROVIDER_BACKEND_UNAVAILABLE to be able to show the provider the correct message
                    errorKey = ErrorKey.ERR_PROVIDER_BACKEND_UNAVAILABLE;
                    log.ProcessingDebug(requestId, "Calling LiveMatch to get the internal pid.");
                    var internalPid = LiveMatch().GetInternalPid(pid, ApiUser, requestId);

                    // The internal web service call succeeded so we reset back the error key to the internal key
                    errorKey = ErrorKey.ERR_INTERNAL_FATAL;
                    if (!string.IsNullOrEmpty(internalPid))
                    {
                        errorKey = ErrorKey.ERR_PROVIDER_BACKEND_UNAVAILABLE;
                        log.ProcessingDebug(requestId, "Updating attributes to respondent catalog.");
                        if (AddRespondentCatalogAttributes(respondentAttributes, internalPid))
                        {
                            errorKey = ErrorKey.ERR_INTERNAL_FATAL;
                            log.ProcessingDebug(requestId, "Getting " + OfferType + "s from the database.");
                            List<OfferApiObject> offers = RetrieveOffers(respondentAttributes, ApiUser, OfferType);

                            if (offers != null && offers.Count > 0)
                            {
                                errorKey = ErrorKey.ERR_PROVIDER_BACKEND_UNAVAILABLE;
                                log.ProcessingDebug(requestId, "Fetching non panelist studies.");
                                var studiesList = (FetchNonPanelistStudies(offers, internalPid, requestId));
                                errorKey = ErrorKey.ERR_INTERNAL_FATAL;
                                if (studiesList != null && studiesList.Count > 0)
                                    offers = (RetrieveExludedOffers(studiesList, offers));
                                if (offers != null && offers.Count > 0)
                                {
                                    data.Offers = offers;
                                }
                                else
                                {
                                    data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_NO_MATCHING_OFFERS));
                                }
                            }
                            //There is no matching offers
                            else
                            {
                                data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_NO_MATCHING_OFFERS));
                            }
                        }
                        else
                        {
                            data.Errors.Add(new ErrorObject(errorKey));
                        }
                    }
                }
                else
                    data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_MISSING_DATA_ARGUMENTS));
            }
            catch (Exception e)
            {
                data.Errors.Add(new ErrorObject(errorKey));
                log.ErrorJson(new Methods().Exception_ToLogObject(requestId, ApiUser, OperationType, OperationName, e));
            }
            finally
            {
                //If the response has errors, insert an error message into the logs
                //Edit for R184
                if (data.Errors.Count != 0)
                {
                    log.InfoJson(new Methods().Error_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data.Errors));
                    log.ProcessingDebug(requestId, "GET " + OfferType + "s with PID request was unsuccessful.");
                }

                //The response has no errors, we insert a request successful message into the logs
                else
                {
                    log.InfoJson(new Methods().Response_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data));
                    log.ProcessingDebug(requestId, "GET " + OfferType + "s with PID request was successful.");
                }
            }
            return data;
        }

        /// <summary>
        /// Function that will return the attributes of a given offer based on the request (Liveoffer or Testoffer)
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="ApiUser"></param>
        /// <param name="offerId"></param>
        /// <returns></returns>
        public OfferAttributesDataObject GetOfferAttributes(HttpRequestMessage Request, string ApiUser, string offerId)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            OfferAttributesDataObject data = new OfferAttributesDataObject();
            String requestId = Request.Properties["requestId"].ToString();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            OfferFilter OfferType = GetOfferType(Request.GetRouteData().Values["controller"].ToString());
            OfferObject offerObj = null;
            List<OfferAttributeApiObject> offerAttributesList = null;
            string OperationName = operationName.GetOffersAttributes.ToString();

            parameters.Add("OfferId", offerId);
            log.ProcessingDebug(requestId, "Received GET Offer attributes request.");
            
            try
            {
                var oid = new Guid();
                Guid.TryParse(offerId, out oid);
                offerObj = OfferRepository().SelectByID(oid);
                
                if (offerObj == null || offerObj.Status != (int)OfferStatus.Active)
                {
                    //Filling the error object in the response envelope with the Error object
                    //Edit for R184
                    data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_INVALID_OFFER_ARGUMENT, parameters));
                    //R185 Modification
                    //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_INVALID_OFFER_ARGUMENT, parameters));
                }
                else
                {
                    log.ProcessingDebug(requestId, "Getting Offer attributes from database.");
                    
                    // Check if the request type and the offer type match 
                    if ((OfferType.Equals(OfferFilter.LiveOffer) && !(bool)offerObj.TestOffer) ||
                        (OfferType.Equals(OfferFilter.TestOffer) && (bool)offerObj.TestOffer))
                    {
                        offerAttributesList = OfferAttributeRepository().GetOfferAttributes(offerObj.Id).ToList();
                        if (offerAttributesList != null && offerAttributesList.Count() > 0)
                        {
                            //Filling the data object in the response envelope with the list of respondent attributes
                            data.Attributes = offerAttributesList;
                        }
                        else
                        {
                            //Filling the error object in the response envelope with the Error object
                            //Edit for R184
                            data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_OFFER_HAS_NO_PUBLISHED_ATTRIBUTES, parameters));
                            //R185 Modification
                            //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_OFFER_HAS_NO_PUBLISHED_ATTRIBUTES, parameters));
                        }
                    }
                    else
                    {
                        // The request type and offer type don't match (provider is either asking testoffers api for a LIVE offer attributes or vice versa
                        data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_INVALID_OFFER_ARGUMENT, parameters));
                    }
                    //Check if there are respondent attribute appended to the offer
                    
                }
            }
            catch (Exception e)
            {
                //Edit for R184
                data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                log.InfoJson(new Methods().Exception_ToLogObject(requestId, ApiUser, OperationType, OperationName, e));
            }
            finally
            {
                if (data.Errors.Count != 0)
                {
                    log.InfoJson(new Methods().Error_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data.Errors));
                    log.ProcessingDebug(requestId, "GET Offer attributes request was unsuccessful.");
                }

                //The response has no errors, we insert a request successful message into the logs
                else
                {
                    log.InfoJson(new Methods().Response_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data));
                    log.ProcessingDebug(requestId, "GET Offer attributes request was successful.");
                }
            }
            return data;
        }

        /// <summary>
        /// Function that will return the quota expression for a requested offer (Liveoffer or Testoffer)
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="ApiUser"></param>
        /// <param name="offerId"></param>
        /// <returns></returns>
        public QuotaExpressionsObjectResponse GetOfferQuotaExpression(HttpRequestMessage Request, string ApiUser, string offerId)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            QuotaExpressionsObjectResponse data = new QuotaExpressionsObjectResponse();
            String requestId = Request.Properties["requestId"].ToString();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            OfferFilter OfferType = GetOfferType(Request.GetRouteData().Values["controller"].ToString());
            string OperationName = operationName.GetQuotaExpressions.ToString();
            parameters.Add("OfferId", offerId);

            var oid = new Guid();

            int errorKey = ErrorKey.ERR_PROVIDER_BACKEND_QUOTA_EXPRESSIONS_UNAVAILABLE;

            try
            {
                if (offerId == null)
                    data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_MISSING_OFFER_DATA_ARGUMENTS, null));
                //Check if given offer id is of type GUID
                else if (Guid.TryParse(offerId, out oid) == false)
                    data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_INVALID_OFFER_ARGUMENT, parameters));
                else
                {
                    log.ProcessingDebug(requestId, "Received GET Quota Expressions request for a " + OfferType + ".");

                    OfferObject offerObject = OfferRepository().SelectByID(oid);
                    if (offerObject == null
                            || offerObject.Status != (int)OfferStatus.Active
                            || (OfferType == OfferFilter.TestOffer && !(bool)offerObject.TestOffer) || (OfferType == OfferFilter.LiveOffer && (bool)offerObject.TestOffer))
                    {
                        data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_INVALID_OFFER_ARGUMENT, parameters));
                    }
                    else
                    {
                        var sampleId = offerObject.SampleId.Value;
                        SteamStudyObject steamStudyObject = QuotaExpressionRepository().SelectByID(sampleId);
                            var responseType = Request.Headers.Accept.ToString().ToLower().Contains("application/xml");
                            if (!responseType)
                            {
                                //Filling the data object in the response envelope with the Quota Expressions
                                var xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(steamStudyObject.QuotaExpressionsXML);
                                data.Errors = null;
                                data.QuotaCells = JObject.Parse(JsonConvert.SerializeXmlNode(xmlDoc));
                            }
                            else
                            {
                                data = new QuotaExpressionsObjectResponse(steamStudyObject.QuotaExpressionsXML);
                            }
                        }
                    }
                }
            catch (Exception e)
            {
                data = new QuotaExpressionsObjectResponse();
                data.Errors.Add(new ErrorObject(errorKey));
                log.InfoJson(new Methods().Exception_ToLogObject(requestId, ApiUser, OperationType, OperationName, e));
            }
            finally
            {
                //If the response has errors, insert an error message into the logs
                if (data.Errors != null)
                {
                    log.InfoJson(new Methods().Error_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data.Errors));
                    log.ProcessingDebug(requestId, "GET Quota Expressions request was unsuccessful.");
                }

                //The response has no errors, we insert a request successful message into the logs
                else
                {
                    log.InfoJson(new Methods().Response_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data));
                    log.ProcessingDebug(requestId, "GET Quota Expressions request was successful.");
                }
            }
            return data;
        }

        /// <summary>
        /// Function that will return the offer type based on the controller name
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        private OfferFilter GetOfferType(string controllerName)
        {
            var OfferType = OfferFilter.None;
            if (controllerName.ToLower().Equals("testoffers"))
                OfferType = OfferFilter.TestOffer;
            else if (controllerName.ToLower().Equals("liveoffers"))
                OfferType = OfferFilter.LiveOffer;
            return OfferType;
        }

        #endregion
    }
}
