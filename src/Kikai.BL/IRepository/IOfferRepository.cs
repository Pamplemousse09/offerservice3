using Kikai.BL.DTO;
using Kikai.BL.DTO.ApiObjects;
using Kikai.BL.DTO.InternalApiObjects;
using System;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
    public interface IOfferRepository
    {
        OfferObject SelectOfferBySampleId(int SampleId);

        IEnumerable<StudyOfferObject> GetStudyIdsFromOfferIds(string OfferIds);

        IEnumerable<StudyOfferObject> GetOfferIdsFromStudyIds(string StudyIds);

        IEnumerable<SampleObject> GetActiveSampleIds();

        IEnumerable<SampleObject> GetSuspendedSampleIdsAndDates();

        IEnumerable<SampleObject> GetPendingSampleIdsAndRetryCount();

        IEnumerable<FilteredOfferObject> GetFilteredOffers(int? StudyId, string OfferTitle, int? OfferStatus, int? OfferType, int Page, int RecordsPerPage, string sortby = "", string sortdir = "");

        IEnumerable<OfferApiObject> GetActiveOffersHavingValidTerm(bool testOffer);

        IEnumerable<OfferApiObject> GetActiveOffersHavingValidTerm();

        IEnumerable<InternalApiOffersObject> SelectByStudyId(object id);

        OfferObject SelectByID(object id);

        void UpdateOfferStatus(int SampleId, int Status);

        void UpdateRetryCount(int SampleId, int RetryCount);

        void UpdateQuotaRemaining(int SampleId, int QuotaRemaining);
    }
}
