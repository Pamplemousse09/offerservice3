using Kikai.Internal.Contracts.Objects;

namespace Kikai.Internal.IManagers
{
    public interface ISteamStudy
    {
        void UpdateQuotaExpression(SteamStudyObject steamStudyObject, GMISampleQuotasObject gmiSampleQuotasObject);
        SteamStudyObject GetQuotasAttributes(int studyId, int sampleId);
    }
}
