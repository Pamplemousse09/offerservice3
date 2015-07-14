using Kikai.BL.DTO;
using Kikai.BL.DTO.ApiObjects;
using Kikai.BL.IRepository;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.IManagers;
using Kikai.Internal.Managers;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Kikai.WebAPI.Managers
{
    public static class AutomationManager
    {
        public static Mock<IOfferRepository> IOfferRepository = new Mock<IOfferRepository>();
        public static Mock<IOfferAttributeRepository> IOfferAttributeRepository = new Mock<IOfferAttributeRepository>();
        public static Mock<IProviderRepository> IProviderRepository = new Mock<IProviderRepository>();
        public static Mock<IAttributeRepository> IAttributeRepository = new Mock<IAttributeRepository>();
        public static Mock<IQuotaExpressionRepository> IQuotaExpressionRepository = new Mock<IQuotaExpressionRepository>();
        public static Mock<IQuotaMappingRepository> IQuotaMappingRepository = new Mock<IQuotaMappingRepository>();
        public static Mock<ISampleMappingRepository> ISampleMappingRepository = new Mock<ISampleMappingRepository>();

        public static Mock<ILiveMatch> ILiveMatch = new Mock<ILiveMatch>();
        public static Mock<IRespondentCatalog> IRespondentCatalog = new Mock<IRespondentCatalog>();
        public static Mock<IGMIStudy> IGMIStudy = new Mock<IGMIStudy>();
        public static Mock<ISteamStudy> ISteamStudy = new Mock<ISteamStudy>();
        public static Mock<IQuotaLiveMatch> IQuotaLiveMatch = new Mock<IQuotaLiveMatch>();

        public static List<OfferObject> offers = new List<OfferObject>();
        public static List<OfferApiObject> offersapi = new List<OfferApiObject>();
        public static List<AttributeObject> attributes = new List<AttributeObject>();
        public static List<OfferAttributeApiObject> apiattributes = new List<OfferAttributeApiObject>();
        public static List<TermObject> terms = new List<TermObject>();
        public static List<String> studiesList = new List<string>();
        public static Dictionary<string, string> respondentCatalogueAttributes = new Dictionary<string, string>();

        public static string internalpid = null;

        public static void OfferObjectToOfferApiObject(List<OfferObject> offersToConvert, bool Test)
        {
            var convertedOffersApiObjects = new List<OfferApiObject>();
            foreach (var offerToConvert in offersToConvert)
            {
                if (offerToConvert.Status == 1 && offerToConvert.TestOffer == Test)
                {
                    OfferApiObject convertedOfferApiObject = new OfferApiObject();
                    convertedOfferApiObject.CPI = (offerToConvert.Terms != null && offerToConvert.Terms.Count > 0) ? offerToConvert.Terms.FindLast(i => i.Active == true).CPI : 0;
                    convertedOfferApiObject.Description = offerToConvert.Description;
                    convertedOfferApiObject.IR = offerToConvert.IR;
                    convertedOfferApiObject.LOI = offerToConvert.LOI;
                    convertedOfferApiObject.OfferID = offerToConvert.Id;
                    convertedOfferApiObject.OfferLink = offerToConvert.OfferLink;
                    convertedOfferApiObject.QuotaRemaining = offerToConvert.QuotaRemaining;
                    convertedOfferApiObject.TermID = (offerToConvert.Terms != null && offerToConvert.Terms.Count > 0) ? offerToConvert.Terms.FindLast(i => i.Active == true).Id : new Guid();
                    convertedOfferApiObject.TestOffer = (bool)offerToConvert.TestOffer;
                    convertedOfferApiObject.Title = offerToConvert.Title;
                    convertedOfferApiObject.Topic = offerToConvert.Topic;
                    convertedOffersApiObjects.Add(convertedOfferApiObject);
                    IOfferRepository.Setup(i => i.SelectByID(offerToConvert.Id)).Returns(offerToConvert);
                }
            }
            IOfferRepository.Setup(i => i.GetActiveOffersHavingValidTerm(Test)).Returns(convertedOffersApiObjects);
        }

        public static void AttributeObjectToAttributeApiObject(List<AttributeObject> attributesToConvert)
        {
            var convertedAttributesApiObjects = new List<AttributeApiObject>();
            foreach (var attributeToConvert in attributesToConvert)
            {
                AttributeApiObject convertedAttributeApiObject = new AttributeApiObject();
                convertedAttributeApiObject.Id = attributeToConvert.Id;
                convertedAttributeApiObject.Label = attributeToConvert.Label;
                if (attributeToConvert.AttributeSetting.Publish == true)
                {
                    convertedAttributesApiObjects.Add(convertedAttributeApiObject);
                }
                IAttributeRepository.Setup(i => i.SelectByID(convertedAttributeApiObject.Id)).Returns(attributeToConvert);
            }
            IAttributeRepository.Setup(i => i.SelectPublishedAttributes()).Returns(convertedAttributesApiObjects);
        }

        public static void AttributeObjectToAttributeDetailsApiObject(List<AttributeObject> attributesToConvert)
        {
            foreach (var attributeToConvert in attributesToConvert)
            {
                AttributeDetailsApiObject attributeDetails = new AttributeDetailsApiObject();
                attributeDetails.AttributeOptions = (attributeToConvert.AttributeOptions != null) ? attributeToConvert.AttributeOptions : new List<AttributeOptionObject>() { new AttributeOptionObject() { AttributeId = attributeToConvert.Id, Code = "1", Description = "Default Description" } };
                attributeDetails.Id = attributeToConvert.Id;
                attributeDetails.Label = (attributeToConvert.Label != null) ? attributeToConvert.Label : "Default Label";
                attributeDetails.Type = (attributeToConvert.Name != null) ? attributeToConvert.Name : "Default Name";
                IAttributeRepository.Setup(i => i.GetAttributeDetails(attributeToConvert.Id)).Returns(attributeDetails);
            }
        }

        public static void RespondentAttributeToOfferApiObject(List<OfferObject> offers)
        {
            List<OfferAttributeApiObject> convertedAttributes;
            foreach (var offer in offers)
            {
                convertedAttributes = new List<OfferAttributeApiObject>();
                if (offer.RespondentAttributes != null)
                {
                    foreach (var respondent in offer.RespondentAttributes)
                    {
                        if (attributes.Exists(i => i.Id == respondent.Ident))
                        {
                            if ((bool)attributes.Find(i => i.Id == respondent.Ident).AttributeSetting.Publish)
                            {
                                OfferAttributeApiObject oaao = new OfferAttributeApiObject();
                                oaao.Name = respondent.Ident;
                                oaao.Value = respondent.Values;
                                convertedAttributes.Add(oaao);
                            }
                        }
                    }
                    IOfferAttributeRepository.Setup(i => i.GetOfferAttributes(offer.Id)).Returns(convertedAttributes);
                }
            }
        }

        public static void SetRequiredAttributes()
        {
            var requiredAttributes = new List<AttributeObject>()
            {
                new AttributeObject(){Id = "COREcontact_country"},
                new AttributeObject(){Id = "CORElanguage"}
            };
            foreach (var attribute in requiredAttributes)
            {
                IAttributeRepository.Setup(i => i.SelectByID(attribute.Id)).Returns(attribute);
            }
            IAttributeRepository.Setup(i => i.SelectRequiredAttributes()).Returns(requiredAttributes);
        }

        public static void SetStudyIds()
        {
            List<StudyOfferObject> StudyOfferObjects = new List<StudyOfferObject>();
            if (offers != null)
            {
                foreach (var offer in offers)
                {
                    if (offer.StudyId != null)
                    {
                        if (studiesList.Contains(offer.StudyId.ToString()))
                        {
                            StudyOfferObjects.Add(new StudyOfferObject() { OfferId = offer.Id });
                        }
                    }
                }
            }
            IOfferRepository.Setup(i => i.GetOfferIdsFromStudyIds(string.Join(",", studiesList))).Returns(StudyOfferObjects);
        }

        public static bool InsertOffer(WebServiceResponse offer)
        {
            bool success = false;
            if (offer != null && offer.Data != null)
            {
                OfferObject convertedOffer = JsonConvert.DeserializeObject<OfferObject>(offer.Data);
                if (convertedOffer.Id != new Guid())
                {
                    OfferObject offerToAdd = new OfferObject();
                    offerToAdd.Id = convertedOffer.Id;
                    offerToAdd.Description = (convertedOffer.Description != null) ? convertedOffer.Description : "Default Description";
                    offerToAdd.Status = convertedOffer.Status;
                    offerToAdd.IR = (convertedOffer.IR != null) ? convertedOffer.IR : 0;
                    offerToAdd.LOI = (convertedOffer.LOI != null) ? convertedOffer.LOI : 0;
                    offerToAdd.OfferLink = (convertedOffer.OfferLink != null) ? convertedOffer.OfferLink : "https://devhub.globaltestmarket.com/hub/tplm/welcome";
                    offerToAdd.QuotaRemaining = (convertedOffer.QuotaRemaining != null) ? convertedOffer.QuotaRemaining : 0;
                    offerToAdd.RespondentAttributes = new List<RespondentAttributeObject>();
                    offerToAdd.SampleId = (convertedOffer.SampleId != null) ? convertedOffer.SampleId : 0;
                    offerToAdd.StudyId = (convertedOffer.StudyId != null) ? convertedOffer.StudyId : 0;
                    offerToAdd.StudyStartDate = (convertedOffer.StudyStartDate != null) ? convertedOffer.StudyStartDate : null;
                    offerToAdd.StudyEndDate = (convertedOffer.StudyEndDate != null) ? convertedOffer.StudyEndDate : null;
                    offerToAdd.Terms = new List<TermObject>();
                    offerToAdd.TestOffer = (convertedOffer.TestOffer != null) ? convertedOffer.TestOffer : false;
                    offerToAdd.Title = (convertedOffer.Title != null) ? convertedOffer.Title : "Default Title";
                    offerToAdd.Topic = (convertedOffer.Topic != null) ? convertedOffer.Topic : "Default Topic";
                    offerToAdd.RetryCount = (convertedOffer.RetryCount != null) ? convertedOffer.RetryCount : 0;
                    offers.Add(offerToAdd);
                    success = true;
                }
            }
            return success;
        }

        public static bool InsertAttribute(WebServiceResponse attribute)
        {
            bool success = false;
            if (attribute != null && attribute.Data != null)
            {
                AttributeObject attributeToInsert = JsonConvert.DeserializeObject<AttributeObject>(attribute.Data);
                AttributeSettingObject attributeSettingToInsert = JsonConvert.DeserializeObject<AttributeSettingObject>(attribute.Data);
                attributeToInsert.AttributeSetting = new AttributeSettingObject();
                attributeToInsert.AttributeSetting.Publish = (attributeSettingToInsert.Publish != null) ? attributeSettingToInsert.Publish : true;
                AutomationManager.attributes.Add(attributeToInsert);
                AutomationManager.IAttributeRepository.Setup(i => i.SelectByID(attributeToInsert.Id)).Returns(new AttributeObject() { Id = attributeToInsert.Id });
                success = true;
            }
            return success;
        }

        public static bool InsertRespondentAttribute(WebServiceResponse respondentAttribute)
        {
            bool success = false;
            if (respondentAttribute != null && respondentAttribute.Data != null)
            {
                RespondentAttributeObject respondentAttributeToInsert = JsonConvert.DeserializeObject<RespondentAttributeObject>(respondentAttribute.Data);
                if (respondentAttributeToInsert.OfferId != null)
                {
                    if (AutomationManager.offers.Exists(i => i.Id == respondentAttributeToInsert.OfferId))
                    {
                        OfferObject offer = offers.Find(i => i.Id == respondentAttributeToInsert.OfferId);
                        offers.Remove(offer);
                        if (offer.RespondentAttributes == null)
                            offer.RespondentAttributes = new List<RespondentAttributeObject>();
                        offer.RespondentAttributes.Add(respondentAttributeToInsert);
                        offers.Add(offer);
                        success = true;
                    }
                }
            }
            return success;
        }

        public static bool InsertTerm(WebServiceResponse term)
        {
            bool success = false;
            if (term != null && term.Data != null)
            {
                TermObject termToInsert = JsonConvert.DeserializeObject<TermObject>(term.Data);
                if (termToInsert.OfferId != null)
                {
                    if (offers.Exists(i => i.Id == termToInsert.OfferId))
                    {
                        OfferObject offer = offers.Find(i => i.Id == termToInsert.OfferId);
                        offers.Remove(offer);
                        if (offer.Terms == null)
                            offer.Terms = new List<TermObject>();
                        offer.Terms.Add(termToInsert);
                        offers.Add(offer);
                        success = true;
                    }
                }

            }
            return success;
        }

        public static bool LiveMatchInternalPIDResponse(WebServiceResponse InternalPIDResponse)
        {
            bool success = false;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(InternalPIDResponse.Data);
                internalpid = new LiveMatch().ProcessLiveMatchIntenalPIDResponse(xmlDoc);
                ILiveMatch.Setup(i => i.GetInternalPid(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(AutomationManager.internalpid);
                success = true;
            }
            catch
            {

            }
            return success;
        }

        public static bool LiveMatchFetchNonPanelistStudiesResponse(WebServiceResponse FetchNonPaneListResponse)
        {
            bool success = false;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(FetchNonPaneListResponse.Data);
                studiesList = new LiveMatch().ProcessLiveMatchStudiesActivityResponse(FetchNonPaneListResponse.Data);
                AutomationManager.ILiveMatch.Setup(i => i.ProcessLiveMatchStudiesActivityResponse(It.IsAny<string>())).Returns(studiesList);
                AutomationManager.ILiveMatch.Setup(i => i.CallLiveMatchService("fetchNonPanelistActivity", It.IsAny<Hashtable>())).Returns(xmlDoc);
                success = true;
            }
            catch
            {

            }
            return success;
        }

        public static bool ThirdpartyRespondentProfileResponse(WebServiceResponse ThirdPartyRespondentResponse)
        {
            bool success = false;
            try
            {
                respondentCatalogueAttributes = new RespondentCatalog().ProcessRespondentCatalogAttributesResponse(ThirdPartyRespondentResponse.Data);
                IRespondentCatalog.Setup(i => i.GetRespondentCatalogueAttributes(AutomationManager.internalpid)).Returns(respondentCatalogueAttributes);
                success = true;
            }
            catch
            {

            }
            return success;
        }

        public static bool QuotaExpressions(WebServiceResponse QuotaExpressionResponse)
        {
            bool success = false;
            Guid oid = new Guid();
            int SampleId = 0;
            if (Int32.TryParse(QuotaExpressionResponse.OptionalParam, out SampleId))
            {
                if (offers.Exists(i => i.SampleId == SampleId))
                {
                    var offer = offers.Find(i => i.SampleId == Convert.ToInt32(QuotaExpressionResponse.OptionalParam));
                    IQuotaExpressionRepository.Setup(i => i.SelectByID(SampleId)).Returns(new SteamStudyObject() { ExpressionsXML = QuotaExpressionResponse.Data, QuotaExpressionsXML = QuotaExpressionResponse.Data, OfferId = offer.Id, SampleId = SampleId });
                    success = true;
                }
            }
            else if (Guid.TryParse(QuotaExpressionResponse.OptionalParam, out oid))
            {
                if (offers.Exists(i => i.Id == oid))
                {
                    var offer = offers.Find(i => i.Id == oid);
                    if (offer.SampleId != null)
                    {
                        IQuotaExpressionRepository.Setup(i => i.SelectByID(offer.SampleId)).Returns(new SteamStudyObject() { ExpressionsXML = QuotaExpressionResponse.Data, QuotaExpressionsXML = QuotaExpressionResponse.Data, OfferId = offer.Id, SampleId = (int)offer.SampleId });
                        success = true;
                    }
                }
            }
            return success;
        }

        public static void reset()
        {
            IOfferRepository = new Mock<IOfferRepository>();
            IOfferAttributeRepository = new Mock<IOfferAttributeRepository>();
            IProviderRepository = new Mock<IProviderRepository>();
            IAttributeRepository = new Mock<IAttributeRepository>();
            IQuotaExpressionRepository = new Mock<IQuotaExpressionRepository>();
            IQuotaMappingRepository = new Mock<IQuotaMappingRepository>();
            ISampleMappingRepository = new Mock<ISampleMappingRepository>();

            ILiveMatch = new Mock<ILiveMatch>();
            IRespondentCatalog = new Mock<IRespondentCatalog>();
            IGMIStudy = new Mock<IGMIStudy>();
            ISteamStudy = new Mock<ISteamStudy>();
            IQuotaLiveMatch = new Mock<IQuotaLiveMatch>();

            offers = new List<OfferObject>();
            offersapi = new List<OfferApiObject>();
            attributes = new List<AttributeObject>();
            apiattributes = new List<OfferAttributeApiObject>();
            terms = new List<TermObject>();
            studiesList = new List<string>();
            respondentCatalogueAttributes = new Dictionary<string, string>();

            internalpid = null;

        }
    }

    public class WebServiceResponse
    {
        public string Data { get; set; }
        public string OptionalParam { get; set; }
    }

}
