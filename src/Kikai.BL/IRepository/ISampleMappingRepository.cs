using Kikai.Internal.Contracts.Objects;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
    public interface ISampleMappingRepository
    {
         IEnumerable<GMISampleObject> SelectAll();
         GMISampleObject SelectByID(object sampleId);
         GMISampleObject SelectByInternalSampleID(object sampleId);
         void Insert(GMISampleObject obj);
         void Update(GMISampleObject obj);
         void Delete(object sampleId);
    }
}
