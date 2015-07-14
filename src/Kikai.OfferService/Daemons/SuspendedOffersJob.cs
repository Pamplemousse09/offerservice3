using Kikai.BL.Concrete;
using Kikai.BL.IRepository;
using Kikai.Common.Managers;
using Kikai.BL.Utils;
using Kikai.Domain.Common;
using Kikai.Logging.DTO;
using Kikai.Logging.Utils;
using Kikai.Logging.Extensions;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kikai.OfferService.Daemons
{
    [DisallowConcurrentExecution]
    public class SuspendedOffersJob : IJob
    {
        Dictionary<string, string> parameters;

        public SuspendedOffersJob()
        {
            parameters = new Dictionary<string, string>();
        }

        public void Execute(IJobExecutionContext context)
        {
            LoggerFactory.SetThreadLogger("SuspendedOffersJob");
            LoggingUtility log = LoggerFactory.GetLogger();
            log.Info("SuspendedOffersJob started processing at " + DateTime.Now);
            IOfferRepository offerRep = new OfferRepository();

            //Getting all the suspended samples in order to check if they are still mainstream enabled
            var suspendedSamples = offerRep.GetSuspendedSampleIdsAndDates();

            //The maximum number of sent sample per request
            var MaxSamplePerRequest = 150;

            //This loop was made to send limited number of samples per request because the service returns HTTP response 414 for large numbers of samples
            for (var i = 0; i < suspendedSamples.Count(); i += MaxSamplePerRequest)
            {
                //Takes maximum number of samples starting index "i"
                var currentSamples = suspendedSamples.Skip(i).Take(MaxSamplePerRequest).ToList();
                var errors = new List<ErrorObject>();
                SampleManager sampleManager = null;

                //Check if the service threw an error
                try
                {
                    sampleManager = new SampleManager(new JobUtil().SampleListAsString(currentSamples));
                }
                catch
                {
                    //If the service threw an error it must be a connection error
                    errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_SAM_CONNECTION, parameters));
                    log.ErrorJson(new Methods().Error_ToLogObject(Guid.NewGuid().ToString(), "OfferService", operationType.SAMCall.ToString(), "SuspendedOffersJob".ToString(), parameters, errors));
                }

                //If the service didn't throw any error continue
                if (errors.Count() == 0 && !sampleManager.ResponseHasUnwantedErrors())
                {
                    //Loop through all the samples
                    foreach (var sample in currentSamples)
                    {
                        //Adding the sample to the parameters
                        parameters.Add("SampleId", sample.SampleId.ToString());

                        //Inactivate the offer if the conditions are met (End date reached or no end date on the offer but 30 days has passed since the start date)
                        var expiration = sample.StudyEndDate;
                        if (expiration == null && sample.StudyStartDate != null)
                        {
                            expiration = sample.StudyStartDate.AddDays(30);
                        }

                        //Check if the sample exist in the service response
                        bool isMainstreamEnabled = sampleManager.CheckIfSampleExist(sample.SampleId);

                        if (isMainstreamEnabled)
                        {
                            //If sample is still enabled, try to activate the offer and update the respondent attributes and quota remaining
                            try
                            {
                                var newStatus = sampleManager.Activate(sample.SampleId, sample.Id);
                            }
                            // If offer activation throws an exception (E.G. connection was lost while trying to get quota cells), keep offer status as suspended
                            catch
                            {
                                offerRep.UpdateOfferStatus(sample.SampleId, (int)OfferStatus.Suspended);
                            }
                        }
                        else
                        {
                            if (expiration == null || expiration <= DateTime.Now)
                            {
                                offerRep.UpdateOfferStatus(sample.SampleId, (int)OfferStatus.Inactive);
                                log.Info("Sample " + sample.SampleId + " is not mainstream enabled and it expired (Expiration date: " + expiration + "). Setting sample to Inactive.");
                            }
                            else
                            {
                                log.Info("Sample " + sample.SampleId + " is not mainstream enabled and not expired yet. Sample will remain suspended.");
                            }
                        }
                        //Clear the dictionaries so we don t have duplicate keys
                        errors.Clear();
                        parameters.Clear();
                    }
                }
            }
            log.Info("SuspendedOffersJob ended processing at " + DateTime.Now);
        }
    }
}
