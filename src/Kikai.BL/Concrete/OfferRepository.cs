using Kikai.BL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Kikai.BL.IRepository;
using Kikai.BL.DTO.ApiObjects;
using Kikai.BL.DTO.InternalApiObjects;

namespace Kikai.BL.Concrete
{
    public class OfferRepository : IGenericRepository<OfferObject>, IOfferRepository
    {
        public IEnumerable<OfferObject> SelectAll()
        {
            return new DataUtils().GetList<OfferObject>("EXEC Offer_GetAll").ToList();
        }

        public OfferObject SelectByID(object id)
        {
            OfferObject offer = new DataUtils().GetList<OfferObject>("EXEC Offer_GetById @OfferId=@P1", id).ToList().FirstOrDefault();
            if (offer != null)
            {
                offer.Terms = new TermRepository().SelectByOfferId(id);
                offer.RespondentAttributes = new RespondentAttributeRepository().SelectByOffer((Guid)id).ToList();
            }
            return offer;
        }

        public IEnumerable<InternalApiOffersObject> SelectByStudyId(object id)
        {
            return new DataUtils().GetList<InternalApiOffersObject>("EXEC Offer_GetByStudyId @StudyId=@P1", id).ToList();
        }

        /// <summary>
        /// This function takes the Sample Id and returns the correspondent Offer for it
        /// </summary>
        /// <param name="SampleId"></param>
        /// <returns></returns>
        public OfferObject SelectOfferBySampleId(int SampleId)
        {
            return new DataUtils().GetList<OfferObject>("EXEC Offer_GetBySampleId @SampleId=@P1", SampleId).ToList().FirstOrDefault();
        }

        public void Insert(OfferObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Offer_Add @StudyId=@P1, 
                                    @SampleId=@P2, 
                                    @LOI=@P3, 
                                    @IR=@P4, 
                                    @Status=@P5, 
                                    @Description=@P6, 
                                    @Title=@P7, 
                                    @Topic=@P8, 
                                    @OfferLink=@P9,
                                    @QuotaRemaining=@P10, 
                                    @StudyStartDate=@P11, 
                                    @StudyEndDate=@P12",
                                    obj.StudyId,
                                    obj.SampleId,
                                    obj.LOI,
                                    obj.IR,
                                    obj.Status,
                                    obj.Description,
                                    obj.Title,
                                    obj.Topic,
                                    obj.OfferLink,
                                    obj.QuotaRemaining,
                                    obj.StudyStartDate,
                                    obj.StudyEndDate);
        }

        public void Update(OfferObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Offer_Update @Id=@P1,
                                    @StudyId=@P2,
                                    @SampleId=@P3,
                                    @LOI=@P4,
                                    @IR=@P5,
                                    @Status=@P6,
                                    @Test=@P7,
                                    @Description=@P8,
                                    @Title=@P9,
                                    @Topic=@P10,
                                    @OfferLink=@P11,
                                    @QuotaRemaining=@P12,
                                    @StudyStartDate=@P13,
                                    @StudyEndDate=@P14,
 									@RetryCount=@P15",
                                    obj.Id,
                                    obj.StudyId,
                                    obj.SampleId,
                                    obj.LOI,
                                    obj.IR,
                                    obj.Status,
                                    obj.TestOffer,
                                    obj.Description,
                                    obj.Title,
                                    obj.Topic,
                                    obj.OfferLink,
                                    obj.QuotaRemaining,
                                    obj.StudyStartDate,
                                    obj.StudyEndDate,
                                    obj.RetryCount);
        }

        public void Delete(object id)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Offer_Delete @Id=@P1", id);
        }

        /// <summary>
        /// Gets the list of Study Ids from a comma separated string of Offer Ids.
        /// </summary>
        /// <param name="OfferIds"></param>
        /// <returns></returns>
        public IEnumerable<StudyOfferObject> GetStudyIdsFromOfferIds(string OfferIds)
        {
            return new DataUtils().GetList<StudyOfferObject>(@"EXEC Offer_GetStudyIdsFromOfferIds @OfferIds=@P1", OfferIds).ToList();
        }

        /// <summary>
        /// Gets the list of Offer Ids from a comma separated string of Study Ids
        /// </summary>
        /// <param name="StudyIds"></param>
        /// <returns></returns>
        public IEnumerable<StudyOfferObject> GetOfferIdsFromStudyIds(string StudyIds)
        {
            return new DataUtils().GetList<StudyOfferObject>(@"EXEC Offer_GetOfferIdsFromStudyIds @StudyIds=@P1", StudyIds).ToList();
        }

        /// <summary>
        /// Gets the offer Id and the Sample Id from all the active offers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SampleObject> GetActiveSampleIds()
        {
            try
            {
                return new DataUtils().GetList<SampleObject>(@"EXEC Offer_GetActiveSampleIds").ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the offer Id and the Sample Id from all the Suspended offers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SampleObject> GetSuspendedSampleIdsAndDates()
        {
            try
            {
                return new DataUtils().GetList<SampleObject>(@"EXEC Offer_GetSuspendedSampleIdsAndDates").ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the offer Id, the Sample Id, and the Retry Count from all the pending offers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SampleObject> GetPendingSampleIdsAndRetryCount()
        {
            try
            {
                return new DataUtils().GetList<SampleObject>(@"EXEC Offer_GetPendingSampleIdsAndRetryCount").ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Function that returns a filtered list of offers
        /// </summary>
        /// <param name="StudyId"></param>
        /// <param name="OfferTitle"></param>
        /// <param name="OfferStatus"></param>
        /// <param name="OfferType"></param>
        /// <param name="Page"></param>
        /// <param name="RecordsPerPage"></param>
        /// <returns></returns>
        public IEnumerable<FilteredOfferObject> GetFilteredOffers(int? StudyId, string OfferTitle, int? OfferStatus, int? OfferType, int Page, int RecordsPerPage, string sortby = "", string sortdir = "")
        {
            return new DataUtils().GetList<FilteredOfferObject>(@"exec Offer_GetFilteredOffers @StudyId=@P1,@OfferTitle=@P2,@OfferStatus=@P3,@OfferType=@P4,@Page=@P5,@RecordsPerPage=@P6,@SortBy=@P7,@SortDir=@P8", StudyId, OfferTitle, OfferStatus, OfferType, Page, RecordsPerPage, sortby, sortdir).ToList();
        }

        /// <summary>
        /// Gets the list of Offers that are active and have a valid Term 
        /// - testOffer = 1 -> get offers of type TEST. testOffer = 0 -> get offers of type LIVE
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OfferApiObject> GetActiveOffersHavingValidTerm(bool testOffer)
        {
            return new DataUtils().GetList<OfferApiObject>("EXEC Offer_GetActiveOffers @OfferType=@P1", testOffer).ToList();

        }

        /// <summary>
        /// Gets the list of Offers that are active and have a valid Term regardless it was a test offer or live 1.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OfferApiObject> GetActiveOffersHavingValidTerm()
        {
            return new DataUtils().GetList<OfferApiObject>("EXEC Offer_GetActiveOffers").ToList();

        }

        /// <summary>
        /// This function updates the status of the offer associated with the specified sample
        /// </summary>
        /// <param name="OfferId"></param>
        /// <param name="Status"></param>
        public void UpdateOfferStatus(int SampleId, int Status)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Offer_UpdateStatus @SampleId=@P1,@Status=@P2", SampleId, Status);
        }

        /// <summary>
        /// This function sets the retrycount of the offer associated with the specified sample
        /// </summary>
        /// <param name="OfferId"></param>
        /// <param name="Status"></param>
        public void UpdateRetryCount(int SampleId, int RetryCount)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Offer_UpdateRetryCount @SampleId=@P1,@RetryCount=@P2", SampleId, RetryCount);
        }

        /// <summary>
        /// This function sets the QuotaRemaining of the offer associated with the specified sample
        /// </summary>
        /// <param name="OfferId"></param>
        /// <param name="Status"></param>
        public void UpdateQuotaRemaining(int SampleId, int QuotaRemaining)
        {
            new DataUtils().ExcecuteCommand(@"EXEC Offer_UpdateQuotaRemaining @SampleId=@P1,@QuotaRemaining=@P2", SampleId, QuotaRemaining);
        }
    }
}
