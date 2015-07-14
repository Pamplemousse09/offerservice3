using Kikai.BL.DTO;
using Kikai.BL.DTO.ApiObjects;
using Kikai.Domain.Common;
using Kikai.WebApi.Decorators;
using Kikai.WebApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Kikai.WebApi.Managers.IManagers
{
    public interface IOffersManager
    {
        Dictionary<string, string> GetRequestAttributes(HttpRequestMessage Request);
        bool VerifyRequiredAttributes(Dictionary<string, string> respondentAttributes);
        List<OfferApiObject> RetrieveAllOffers(Dictionary<string, string> filterAttributes, string ApiUser);
        List<OfferApiObject> RetrieveLiveOffers(Dictionary<string, string> filterAttributes, string ApiUser);
        List<OfferApiObject> RetrieveTestOffers(Dictionary<string, string> filterAttributes, string ApiUser);
        List<OfferApiObject> FilterOffers(List<OfferApiObject> liveOffers, Dictionary<string, string> respondentAttributes);
        List<int> GetOffersStudiesList(List<OfferApiObject> offers);
        List<string> FetchNonPanelistStudies(List<OfferApiObject> offers, string internalPid, string requestId);
        List<OfferApiObject> RetrieveExludedOffers(List<string> studiesList, List<OfferApiObject> offers);
        List<OfferApiObject> UpdateOffersLink(List<OfferApiObject> offers, string ApiUser, string countryCode = null);
        OffersDataObject GetOffers(HttpRequestMessage Request, string ApiUser);
        OffersDataObject GetOffersByPid(HttpRequestMessage Request, string ApiUser, string pid);
        OfferAttributesDataObject GetOfferAttributes(HttpRequestMessage Request, string ApiUser, string offerId);
        QuotaExpressionsObjectResponse GetOfferQuotaExpression(HttpRequestMessage Request, string ApiUser, string offerId);
    }
}