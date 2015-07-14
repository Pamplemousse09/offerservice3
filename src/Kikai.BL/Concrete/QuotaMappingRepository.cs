using Kikai.BL.IRepository;
using System.Collections.Generic;
using System.Linq;
using Kikai.Internal.Contracts.Objects;

namespace Kikai.BL.Concrete
{
    public class QuotaMappingRepository : IGenericRepository<GMIQuotaObject>, IQuotaMappingRepository
    {

        /// <summary>
        /// This function selects all the quota data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GMIQuotaObject> SelectAll()
        {
            return new DataUtils().GetList<GMIQuotaObject>("EXEC QuotaMapping_GetAll").ToList();
        }


        /// <summary>
        /// This function selects Quota data for a given quota id 
        /// </summary>
        /// <returns></returns>
        public GMIQuotaObject SelectByID(object QuotaId)
        {
            return new DataUtils().GetList<GMIQuotaObject>("EXEC QuotaMapping_GetByQuotaId @QuotaId=@P1", QuotaId).ToList().SingleOrDefault();
        }

        /// <summary>
        /// This function selects Quota data for a given sample id 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GMIQuotaObject> SelectBySampleID(object SampleId)
        {
            return new DataUtils().GetList<GMIQuotaObject>("EXEC QuotaMapping_GetBySampleId @SampleId=@P1", SampleId).ToList();
        }

        /// <summary>
        /// This function selects Quota data for a given internal sample id 
        /// </summary>
        /// <returns></returns>
        public GMIQuotaObject SelectByInternalQuotaID(object QuotaId)
        {
            return new DataUtils().GetList<GMIQuotaObject>("EXEC QuotaMapping_GetByInternalQuotaId @QuotaId=@P1", QuotaId).ToList().SingleOrDefault();
        }

        /// <summary>
        /// This function inserts a new quota
        /// </summary>
        /// <returns></returns>
        public void Insert(GMIQuotaObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC QuotaMapping_Add 
                                    @SampleId=@P1,
                                    @QuotaId=@P2, 
                                    @InternalQuotaId=@P3,
                                    @QuotaRemaining=@P4,
                                    @OfferId=@P5",
                                    obj.SampleId,
                                    obj.QuotaId,
                                    obj.InternalQuotaId,
                                    obj.QuotaRemaining,
                                    obj.OfferId
                                    );
        }

        /// <summary>
        /// This function updates the quota remaining by a given quota Id
        /// </summary>
        /// <returns></returns>
        public void Update(GMIQuotaObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC QuotaMapping_Update 
                                    @QuotaId=@P1,
                                    @QuotaRemaining=@P2",
                                    obj.QuotaId,
                                    obj.QuotaRemaining
                                  );
        }

        /// <summary>
        /// This function deletes quota by a given quota Id
        /// </summary>
        /// <returns></returns>
        public void Delete(object QuotaId)
        {
            new DataUtils().ExcecuteCommand(@"EXEC QuotaMapping_Delete @QuotaId=@P1", QuotaId);
        }

        /// <summary>
        /// This function deletes quota by a given sample Id
        /// </summary>
        /// <returns></returns>
        public void DeletebySampleId(object SampleId)
        {
            new DataUtils().ExcecuteCommand(@"EXEC QuotaMapping_DeleteBySample @SampleId=@P1", SampleId);
        }



    }
}
