using System;
using Kikai.Internal.Contracts.Objects;

namespace Kikai.Common.IManagers
{
    public interface ISampleManager
    {
        bool CheckIfSampleExist(int sampleId);
        OpenSampleObject GetOpenSampleObject(int sampleId);
        int Activate(int sampleId, Guid offerId);
    }
}
