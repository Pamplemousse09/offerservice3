using Kikai.BL.Concrete;
using Kikai.BL.DTO;
using Kikai.Common.Managers;
using Kikai.Domain.Common;
using Kikai.Logging.DTO;
using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.Managers;
using Kikai.WebAdmin.Managers;
using Kikai.WebAdmin.DTO;
using Kikai.WebAdmin.Extensions;
using Kikai.WebAdmin.UIModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Threading;
using Kikai.WebAdmin.Utils;
using Kikai.WebAdmin.Common;

namespace Kikai.WebAdmin.Controllers
{
    [CustomAuthorize(UserPermission.OFFER_CAN_ADD_SAMPLES, UserPermission.OFFER_ADMIN)]
    public class SampleController : Controller
    {
        private MessagesUtil msgUtil;

        public SampleController()
        {
            msgUtil = new MessagesUtil();
        }

        public ActionResult GetStudySamples(int studyId)
        {
            var mainstreamStudySampleResponse = new MainstreamStudySampleResponseModel();
            MainstreamStudySampleResponseObject samples = null;
            Thread searchStudyThread = new Thread(() =>
            {
                try
                {
                    samples = new Sam().ProcessGetMainstreamStudySample(studyId);
                }
                catch (Exception e)
                {
                    LoggerFactory.GetLogger().Error(string.Format(msgUtil.GetMessage(MessageKey.LOG_SEARCHSTUDY_EXCEPTION), studyId), e);
                }
            });
            try
            {
                searchStudyThread.Start();
                if (searchStudyThread.Join(10000))
                {
                    OfferRepository offerRep = new OfferRepository();
                    List<SampleModel> sampleList = new List<SampleModel>();
                    foreach (var sample in samples.MainstreamStudySamples)
                    {
                        SampleModel newSample = new SampleModel();
                        newSample.StudySample = sample;
                        if (offerRep.SelectOfferBySampleId(sample.SampleId) != null)
                        {
                            newSample.Exists = true;
                        }
                        sampleList.Add(newSample);
                    }
                    mainstreamStudySampleResponse.SampleList = sampleList;
                    mainstreamStudySampleResponse.Errors = samples.Errors;
                }
                else
                {
                    List<ServiceErrorObject> errorList = new List<ServiceErrorObject>();
                    ServiceErrorObject error = new ServiceErrorObject();
                    error.message = string.Format(msgUtil.GetMessage(MessageKey.ERR_SEARCHSTUDY_TIMEOUT), studyId);
                    errorList.Add(error);
                    mainstreamStudySampleResponse.Errors = errorList;
                }
            }
            catch
            {
                List<ServiceErrorObject> errorList = new List<ServiceErrorObject>();
                ServiceErrorObject error = new ServiceErrorObject();
                error.message = msgUtil.GetMessage(MessageKey.ERR_SEARCHSTUDY_EXCEPTION);
                errorList.Add(error);
                mainstreamStudySampleResponse.Errors = errorList;
            }
            return PartialView("_StudyResult", mainstreamStudySampleResponse);
        }

        [HttpPost]
        public string AddSample(string sampleList)
        {
            //AddResultsModel is used to return to the UI each SampleId if it was successfully added
            List<AddResultModel> results = new List<AddResultModel>();
            //Convert the json sample list to list of SampleSearchObject
            List<SampleSearchObject> samples = new SampleSearchObject().ConvertFromJson(sampleList);
            var errors = new List<ErrorObject>();
            var parameters = new Dictionary<string, string>();

            //Build the string comma separated to send it to the service
            var sampleComma = string.Empty;
            foreach (var sample in samples)
            {
                sampleComma += sample.SampleId + ",";
            }
            //Remove the trailing comma
            sampleComma = sampleComma.Remove(sampleComma.Length - 1);

            SampleManager sampleManager = null;
            parameters.Add("SampleIds", sampleComma);
            Thread sampleManagerThread = new Thread(() =>
            {
                //Send the request to the service and check if the connection to sam failed
                try
                {
                    sampleManager = new SampleManager(sampleComma);
                }
                catch
                {
                    errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_SAM_CONNECTION, parameters));
                    LoggerFactory.GetLogger().InfoJson(Methods.Error_ToLogObject(Guid.NewGuid().ToString(), "OfferService", operationType.SAMCall.ToString(), "AdminUI".ToString(), parameters, errors));
                }
            });

            
            sampleManagerThread.Start();
            if (!sampleManagerThread.Join(10000))
            {
                errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_SAM_CONNECTION, parameters));
                foreach (var sample in samples)
                {
                    var thisResult = new AddResultModel();
                    thisResult.SampleId = sample.SampleId;
                    thisResult.Added = false;
                    thisResult.Message = msgUtil.GetMessage(MessageKey.ERR_ADDSAMPLES_TIMEOUT);
                    results.Add(thisResult);
                }
            }

            //If the connection to sam was successful
            if (errors.Count() == 0)
            {
                //Loop through all the samples to be added
                foreach (var sample in samples)
                {
                    //By default it wasn t added
                    var Added = false;
                    string Message = string.Empty;

                    try
                    {
                        //Setting the fields of the new offer
                        OfferObject newOffer = new OfferObject();
                        newOffer.SampleId = sample.SampleId;
                        newOffer.LOI = sample.LOI;
                        newOffer.StudyStartDate = sample.StartDate;
                        newOffer.StudyEndDate = sample.EndDate;
                        //Default title Survey - {SampleId}
                        newOffer.Title = "Survey - " + sample.SampleId;

                        //If the sample does exist
                        if (sampleManager.CheckIfSampleExist(sample.SampleId))
                        {
                            //Get the specific sample from the service response
                            var newSample = sampleManager.GetOpenSampleObject(sample.SampleId);
                            newOffer.StudyId = newSample.StudyId;
                            newOffer.QuotaRemaining = newSample.MainstreamQuotaRemaining;
                            newOffer.IR = newSample.IR;
                            newOffer.OfferLink = System.Configuration.ConfigurationManager.AppSettings["OfferLink"] + "?{2}&oid={0}&tid={1}";
                            new OfferRepository().Insert(newOffer);

                            newOffer.Id = new OfferRepository().SelectOfferBySampleId(sample.SampleId).Id;
                            new RespondentAttributeRepository().UpdateOrRemove(newOffer.Id, newSample.Attributes);
                            
                            Added = true;
                            Message = msgUtil.GetMessage(MessageKey.MSG_ADDSAMPLES_SUCCESSFULL);
                        }
                        else
                        {
                            Message = msgUtil.GetMessage(MessageKey.ERR_ADDSAMPLES_MISSINGDATA);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerFactory.GetLogger().Error(string.Format(msgUtil.GetMessage(MessageKey.LOG_ADDSAMPLES_EXCEPTION), sample.SampleId, sample.StartDate, sample.EndDate, sample.LOI), e);
                        Added = false;
                    }

                    //Add the result to the list
                    var thisResult = new AddResultModel();
                    thisResult.SampleId = sample.SampleId;
                    thisResult.Added = Added;
                    thisResult.Message = Message;
                    results.Add(thisResult);
                }
            }
            //Return JSON object to the javascript
            return JsonConvert.SerializeObject(results);
        }
    }
}
