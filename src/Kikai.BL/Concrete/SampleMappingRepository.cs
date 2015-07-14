using Kikai.BL.IRepository;
using System.Collections.Generic;
using System.Linq;
using Kikai.Internal.Contracts.Objects;

namespace Kikai.BL.Concrete
{
    public class SampleMappingRepository : IGenericRepository<GMISampleObject>, ISampleMappingRepository
    {
        /// <summary>
        /// This function selects all the samples data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GMISampleObject> SelectAll()
        {
            return new DataUtils().GetList<GMISampleObject>("EXEC SampleMapping_GetAll").ToList();
        }

        /// <summary>
        /// This function selects sample data for a given sample Id
        /// </summary>
        /// <returns></returns>
        public GMISampleObject SelectByID(object sampleId)
        {
            return new DataUtils().GetList<GMISampleObject>("EXEC SampleMapping_GetBySampleId @SampleId=@P1", sampleId).ToList().SingleOrDefault();
        }

        /// <summary>
        /// This function selects sample data for an internal sample Id
        /// </summary>
        /// <returns></returns>
        public GMISampleObject SelectByInternalSampleID(object sampleId)
        {
            return new DataUtils().GetList<GMISampleObject>("EXEC SampleMapping_GetByInternalSampleId @SampleId=@P1", sampleId).ToList().SingleOrDefault();
        }


        /// <summary>
        /// This function inserts a new sample
        /// </summary>
        /// <returns></returns>
        public void Insert(GMISampleObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC SampleMapping_Add 
                                    @SampleId=@P1, 
                                    @InternalSampleId=@P2,
                                    @OfferId=@P3",
                                    obj.SampleId,
                                    obj.InternalSampleId,
                                    obj.OfferId
                                    );
        }

        public void Update(GMISampleObject obj)
        {
        }


        /// <summary>
        /// This function deletes sample by a given sample Id
        /// </summary>
        /// <returns></returns>
        public void Delete(object sampleId)
        {
            new DataUtils().ExcecuteCommand(@"EXEC SampleMapping_Delete @SampleId=@P1", sampleId);
        }

    }
}
