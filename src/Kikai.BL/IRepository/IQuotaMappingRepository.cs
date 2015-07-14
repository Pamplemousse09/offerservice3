using Kikai.Internal.Contracts.Objects;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
    public interface IQuotaMappingRepository
    {
         IEnumerable<GMIQuotaObject> SelectAll();
         GMIQuotaObject SelectByID(object QuotaId);
         IEnumerable<GMIQuotaObject> SelectBySampleID(object SampleId);
         GMIQuotaObject SelectByInternalQuotaID(object QuotaId);
         void Insert(GMIQuotaObject obj);
         void Update(GMIQuotaObject obj);
         void Delete(object QuotaId);
         void DeletebySampleId(object SampleId);
    }
}
