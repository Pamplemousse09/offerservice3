
namespace Kikai.Domain.Common
{
    public enum operationName
    {
        Authentication = 1,
        GetAttributes = 2,
        GetAttributeOptions = 3,
        GetAllLiveOffers = 4,
        GetOffersAttributes = 5,
        GetOffersPID = 6,
        GetAllTestOffers = 7,
        GetQuotaExpressions = 8,
        ApiCall = 9,
        GetOfferAttributeUsage = 10,
        GetOffersByStudyId = 11
    }

    public enum operationType
    {
        //The operationType enumerator is used in Info and Error logs saved in JSON format.
        //Debug level logs format is a string that does not take operationType as a parametre

        //The Internal operation type is not used at the moment but should be used when we want to log any internal request (E.g. making a call to the database) 
        Internal = 0,
        //The Hub operation type is used in the logging of the RpcService requests
        Hub = 1,
        //WS (WebService) operation type is used in the logging of the providers requests
        WS = 2,
        //SamCall operation type is used in the logging of the UI SAM service requests
        SAMCall = 3,
        //SteamServiceCall operation type is used in the logging of the UI Steam service requests
        SteamServiceCall = 4,
        //InternalAPI operation type is used in the logging of the Internal API requests
        InternalAPI = 5
    }

    public enum AttributeStatus
    {
        Unpublished = 0,
        Published = 1
    }

    public enum OfferFilter
    {
        None = -1,
        LiveOffer = 0,
        TestOffer = 1,
        AnyOffer = 2
    }

    public enum OfferStatus
    {
        Inactive = 0,
        Active = 1,
        Suspended = 2,
        Pending = 3
    }
}
