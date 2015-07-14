using Kikai.BL.IRepository;
using Kikai.Internal.Contracts.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Kikai.BL.Concrete
{
    public class QuotaExpressionRepository : IGenericRepository<SteamStudyObject>, IQuotaExpressionRepository
    {
        /// <summary>
        /// This function selects all the  Quota Expressions XML and  Expressions XML 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SteamStudyObject> SelectAll()
        {
            return new DataUtils().GetList<SteamStudyObject>("EXEC QuotaExpression_GetAll").ToList();
        }


        /// <summary>
        /// This function selects the Quota Expressions XML and  the Expressions XML for a given sampleId
        /// </summary>
        /// <returns></returns>
        public SteamStudyObject SelectByID(object sampleId)
        {
            return new DataUtils().GetList<SteamStudyObject>("EXEC QuotaExpression_GetBySampleId @SampleId=@P1", sampleId).ToList().SingleOrDefault();
        }


        /// <summary>
        /// This function inserts Quota Expressions XML and  Expressions XML for a given sampleId
        /// </summary>
        /// <returns></returns>
        public void Insert(SteamStudyObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC QuotaExpression_Add 
                                    @SampleId=@P1, 
                                    @ExpressionsXML=@P2,
                                    @QuotaExpressionsXML=@P3,
                                    @OfferId=@P4",
                                    obj.SampleId,
                                    obj.ExpressionsXML,
                                    obj.QuotaExpressionsXML,
                                    obj.OfferId
                                    );
        }

        /// <summary>
        /// This function updates Quota Expressions XML for a given sampleId
        /// </summary>
        /// <returns></returns>
        public void updateQuotaExpressionsXML(SteamStudyObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC QuotaExpressionQuota_Update 
                                    @SampleId=@P1, 
                                    @QuotaExpressionsXML=@P2",
                                    obj.SampleId,
                                    obj.QuotaExpressionsXML
                                    );
        }



        /// <summary>
        /// This function updates Expressions XML for a given sampleId
        /// </summary>
        /// <returns></returns>
        public void Update(SteamStudyObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC QuotaExpression_Update 
                                    @SampleId=@P1,
                                    @ExpressionsXML=@P2",
                                    obj.SampleId,
                                    obj.ExpressionsXML);
        }

        /// <summary>
        /// This function deletes Quota Expressions XML and  Expressions XML for a given sampleId
        /// </summary>
        /// <returns></returns>
        public void Delete(object sampleId)
        {
            new DataUtils().ExcecuteCommand(@"EXEC QuotaExpression_Delete @SampleId=@P1", sampleId);
        }

    }
}
