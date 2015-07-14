using Kikai.Internal.Contracts.Objects;
namespace Kikai.Internal.IManagers
{
    public interface IQuotaLiveMatch
    {
        QuotasLiveObject GetQuotaRemainingValues(int studyId);

        void UpdateQuotaRemaingValues(GMISampleQuotasObject gmiSampleQuotasObject, QuotasLiveObject quotasLiveObject);
    }
}
