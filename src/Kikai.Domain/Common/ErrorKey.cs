
namespace Kikai.Domain.Common
{
    public static class ErrorKey
    {
        public const int ERR_INTERNAL_FATAL = 0;
        public const int ERR_HUB_AUTHENTICATION_FAILED = 1;
        public const int ERR_HUB_TERM_EXPIRED = 2;
        public const int ERR_HUB_MISSING_DATA_ARGUMENTS = 3;
        public const int ERR_HUB_INVALID_TERM_ARGUMENT = 4;
        public const int ERR_HUB_SERVICE_UNAVAILABLE = 5;
        public const int ERR_HUB_OFFER_SUSPENDED_INACTIVE = 6;
        public const int ERR_HUB_INVALID_OFFER_ARGUMENT = 7;
        public const int ERR_PROVIDER_AUTHENTICATION_FAILED = 8;
        public const int ERR_PROVIDER_AUTHORIZATION_FAILED = 9;
        public const int ERR_PROVIDER_NOT_FOUND = 10;
        public const int ERR_PROVIDER_DEACTIVATED = 11;
        public const int ERR_PROVIDER_SERVICE_UNAVAILABLE = 12;
        public const int ERR_PROVIDER_BACKEND_UNAVAILABLE = 13;
        public const int ERR_PROVIDER_MISSING_DATA_ARGUMENTS = 14;
        public const int ERR_PROVIDER_INVALID_OFFER_ARGUMENT = 15;
        public const int ERR_PROVIDER_INVALID_ATTRIBUTE_ARGUMENT = 16;
        public const int ERR_INTERNAL_SUSPEND_OFFER = 17;
        public const int ERR_PROVIDER_NO_AVAILABLE_OFFER = 18;
        public const int ERR_INTERNAL_SAM_CONNECTION = 19;
        public const int ERR_PROVIDER_NO_MATCHING_OFFERS = 20;
        public const int ERR_INTERNAL_BACKEND_STEAM_UNAVAILABLE = 21;
        public const int ERR_INTERNAL_BACKEND_FETCH_MULTI_UNAVAILABLE = 22;
        public const int ERR_INTERNAL_BACKEND_GET_SAMPLE_UNAVAILABLE  = 23;
        public const int ERR_PROVIDER_BACKEND_QUOTA_EXPRESSIONS_UNAVAILABLE = 24;
        public const int ERR_PROVIDER_MISSING_OFFER_DATA_ARGUMENTS = 25;
        public const int ERR_PROVIDER_MISSING_ACTION = 26;
        public const int ERR_PROVIDER_NO_PUBLISHED_ATTRIBUTES = 27;
        public const int ERR_PROVIDER_OFFER_HAS_NO_PUBLISHED_ATTRIBUTES = 28;
        public const int ERR_INTERNAL_API_MISSING_METHOD = 29;
        public const int ERR_INTERNAL_API_INVALID_STUDY = 30;
        public const int ERR_INTERNAL_API_NO_OFFERS = 31;
    }
}
