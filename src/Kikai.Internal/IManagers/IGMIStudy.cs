using Kikai.Internal.Contracts.Objects;

namespace Kikai.Internal.IManagers
{
    public interface IGMIStudy
    {
        GMISampleQuotasObject GetGMISamples(int studyId, int sampleId);
    }
}
