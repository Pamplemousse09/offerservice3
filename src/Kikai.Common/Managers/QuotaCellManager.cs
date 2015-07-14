using Kikai.BL.Concrete;
using Kikai.BL.DTO;
using Kikai.BL.IRepository;
using Kikai.Domain.Common;
using Kikai.Logging.DTO;
using Kikai.Logging.Extensions;
using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.Managers;
using Kikai.Common.IManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kikai.Common.Managers
{
    public class QuotaCellManager : IQuotaCellManager
    {
        private const string DocumentNotFound = "404";

        public QuotaCellManager()
        {
        }

        public bool InitializeQuotaCells(Guid offerId)
        {
            bool result = false;
            LoggingUtility log = LoggerFactory.GetLogger();
            try
            {
                OfferObject offerObject = new OfferRepository().SelectByID(offerId);
                var parameters = new Dictionary<string, string>();
                var studyId = offerObject.StudyId.Value;
                var sampleId = offerObject.SampleId.Value;
                parameters.Add("StudyId", studyId.ToString());
                parameters.Add("SampleId", sampleId.ToString());
                if (offerObject != null)
                {
                    SteamStudyObject steamStudyObject = new QuotaExpressionRepository().SelectByID(offerObject.SampleId.Value);

                    QuotasLiveObject quotasLiveObject = new QuotaLiveMatch().GetQuotaRemainingValues(offerObject.StudyId.Value);
                    if (quotasLiveObject == null || (quotasLiveObject.QuotasLiveList[0].InternalQuotaId == 0 && quotasLiveObject.QuotasLiveList[0].InternalSampleId == 0))
                    {
                        log.Info(new ErrorObject(ErrorKey.ERR_INTERNAL_BACKEND_FETCH_MULTI_UNAVAILABLE, parameters).Message);
                        result = false;
                    }
                    else if (steamStudyObject == null || steamStudyObject.ExpressionsXML == null)
                    {
                        // The offer is not initialized, we need to call services to store quota level attribute, mappings, and quota remaining
                        log.Debug("Calling SteamService to store the quota level attribute, mappings, and quota remaining for sample: " + offerObject.SampleId.Value);
                        result = CreateQuotaCell(offerObject, quotasLiveObject);
                    }
                    else
                    {
                        // The offer is initialized, we need to update the quota remaining if it has changed
                        log.Debug("Check for quota remaining updates for sample: " + offerObject.SampleId.Value);
                        result = UpdateQuotaCell(offerObject, steamStudyObject, quotasLiveObject);
                    }
                }
            }
            catch
            {
                log.Error("An error occurred while trying to initialize the QuotaCells for offerId: " + offerId.ToString());
                throw;
            }

            return result;
        }

        private bool CreateQuotaCell(OfferObject offerObject, QuotasLiveObject quotasLiveObject)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            IQuotaExpressionRepository quotaExpressionRepository = new QuotaExpressionRepository();
            IQuotaMappingRepository quotaMappingRepository = new QuotaMappingRepository();
            ISampleMappingRepository sampleMappingRepository = new SampleMappingRepository();

            try
            {
                var parameters = new Dictionary<string, string>();
                var sampleId = offerObject.SampleId.Value;
                parameters.Add("SampleId", sampleId.ToString());
                // ensure quota related data in the database is purged for this sample
                quotaExpressionRepository.Delete(sampleId);
                sampleMappingRepository.Delete(sampleId);
                quotaMappingRepository.DeletebySampleId(sampleId);

                int studyId = Convert.ToInt32(offerObject.StudyId);
                log.Debug("Calling SteamService to get the attributes at the quota cell level for studyId: " + studyId + ", sample: " + sampleId);
                SteamStudyObject steamStudyObject = new SteamStudy().GetQuotasAttributes(studyId, sampleId);

                if (steamStudyObject.ExpressionsXML == null || steamStudyObject.ExpressionsXML.Contains(DocumentNotFound) || !steamStudyObject.ExpressionsXML.Contains("<data>"))
                {
                    log.Info(new ErrorObject(ErrorKey.ERR_INTERNAL_BACKEND_STEAM_UNAVAILABLE, parameters).Message);
                    return false;
                }
                // In order for the attributes names to be consistent with what is returned to suppliers on other services
                // like CodeBook and LiveOffers need to remove the prefix panelist_ from all attribute names (added by steam service)
                steamStudyObject.ExpressionsXML = steamStudyObject.ExpressionsXML.Replace("panelist_", null);

                log.Debug("Calling GMI GetSample service to get the mapping from internal to external ids for studyId: " + studyId + ", sample: " + sampleId);
                GMISampleQuotasObject gmiSampleQuotasObject = new GMIStudy().GetGMISamples(studyId, sampleId);
                if (gmiSampleQuotasObject == null || (gmiSampleQuotasObject.GMIQuotasList == null && gmiSampleQuotasObject.GMISampleQuotasList == null))
                {
                    log.Info(new ErrorObject(ErrorKey.ERR_INTERNAL_BACKEND_GET_SAMPLE_UNAVAILABLE, parameters).Message);
                    return false;
                }

                log.Debug("Calling fetchMulti service to get the Quota Remaining for studyId: " + studyId + ", sample: " + sampleId);
                new QuotaLiveMatch().UpdateQuotaRemaingValues(gmiSampleQuotasObject, quotasLiveObject);
                new SteamStudy().UpdateQuotaExpression(steamStudyObject, gmiSampleQuotasObject);

                steamStudyObject.OfferId = offerObject.Id;
                quotaExpressionRepository.Insert(steamStudyObject);

                foreach (GMIQuotaObject gmiQuotaObject in gmiSampleQuotasObject.GMIQuotasList)
                {
                    gmiQuotaObject.OfferId = offerObject.Id;
                    quotaMappingRepository.Insert(gmiQuotaObject);
                }

                foreach (GMISampleObject gmiSampleObject in gmiSampleQuotasObject.GMISampleQuotasList)
                {
                    gmiSampleObject.OfferId = offerObject.Id;
                    sampleMappingRepository.Insert(gmiSampleObject);
                }
            }
            catch (Exception e)
            {
                log.Error("An error occurred while trying to create the quota cells for offerId " + offerObject.Id.ToString(), e);
                throw;
            }
            return true;
        }

        private bool UpdateQuotaCell(OfferObject offerObject, SteamStudyObject steamStudyObject, QuotasLiveObject quotasLiveObject)
        {
			var sampleId = offerObject.SampleId.Value;
            LoggingUtility log = LoggerFactory.GetLogger();

            try
            {
                QuotaMappingRepository quotaMappingRepository = new QuotaMappingRepository();
                GMISampleObject sampleObject = new SampleMappingRepository().SelectByID(steamStudyObject.SampleId);

                List<GMISampleObject> GMISampleObjectList = new List<GMISampleObject>();
                GMISampleObjectList.Add((sampleObject != null) ? sampleObject : new GMISampleObject());
                List<GMIQuotaObject> GMIQuotasList = quotaMappingRepository.SelectBySampleID(steamStudyObject.SampleId).ToList();

                bool needUpdate = false;
                foreach (GMIQuotaObject gmiQuotaObject in GMIQuotasList)
                {
                    var quota = quotasLiveObject.QuotasLiveList.Where(p => p.InternalQuotaId == gmiQuotaObject.InternalQuotaId).Where(p => p.InternalSampleId == sampleObject.InternalSampleId).ToList();
                    if (quota.Count() > 0 && gmiQuotaObject.QuotaRemaining != quota[0].QuotaRemaining)
                    {
                        gmiQuotaObject.QuotaRemaining = quota[0].QuotaRemaining;
                        quotaMappingRepository.Update(gmiQuotaObject);
                        needUpdate = true;
                    }
                }

                if (needUpdate)
                {
                    // There was a change in quota remaining, update the quota expressions XML to reflect the new values
                    log.Debug("Update the quota remaining for sample: " + sampleId);
                    GMISampleQuotasObject gmiSampleQuotasObject = new GMISampleQuotasObject();
                    gmiSampleQuotasObject.GMIQuotasList = GMIQuotasList;
                    gmiSampleQuotasObject.GMISampleQuotasList = GMISampleObjectList;

                    new SteamStudy().UpdateQuotaExpression(steamStudyObject, gmiSampleQuotasObject);
                    new QuotaExpressionRepository().updateQuotaExpressionsXML(steamStudyObject);
                }
            }
            catch (Exception e)
            {
                log.Error("An error occurred while trying to update the quota cells for offerId " + offerObject.Id.ToString(), e);
                throw;
            }

            return true;
        }
    }
}
