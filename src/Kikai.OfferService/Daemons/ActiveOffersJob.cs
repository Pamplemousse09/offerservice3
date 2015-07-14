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
using log4net;

namespace Kikai.OfferService.Daemons
{
    [DisallowConcurrentExecution]
    public class ActiveOffersJob : IJob
    {
        Dictionary<string, string> parameters;

        public ActiveOffersJob()
        {
            parameters = new Dictionary<string, string>();
        }

        public void Execute(IJobExecutionContext context)
        {
            LoggerFactory.SetThreadLogger("ActiveOffersJob");
            LoggingUtility log = LoggerFactory.GetLogger();
            log.Info("ActiveOffersJob started processing at " + DateTime.Now);
            IOfferRepository offerRep = new OfferRepository();

            //Getting all the active samples in order to check if they are still mainstream enabled
            var activeSamples = offerRep.GetActiveSampleIds();

            //The maximum number of sent sample per request
            var MaxSamplePerRequest = 150;

            //This loop was made to send limited number of samples per request because the service returns HTTP response 414 for large numbers of samples
            for (var i = 0; i < activeSamples.Count(); i += MaxSamplePerRequest)
            {
                //Takes maximum number of samples starting index "i"
                var currentSamples = activeSamples.Skip(i).Take(MaxSamplePerRequest).ToList();
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
                    log.Error("Offer service could not connect to SAM for unknown reasons.");
                }

                //If the service didn't throw any error and SAM didn't reply with errors continue
                if (errors.Count() == 0 && !sampleManager.ResponseHasUnwantedErrors())
                {
                    //Loop through all the active samples
                    foreach (var sample in currentSamples)
                    {
                        //Adding the sample to the parameters
                        parameters.Add("SampleId", sample.SampleId.ToString());
                        //Check if the sample exist in the service response
                        bool isMainstreamEnabled = sampleManager.CheckIfSampleExist(sample.SampleId);
                        //If the sample is not mainstream enabled we update the active status to 2(Suspended)
                        if (!isMainstreamEnabled)
                        {
                            offerRep.UpdateOfferStatus(sample.SampleId, (int)OfferStatus.Suspended);
                            //Log that the sample id will be suspended
                            log.Info("Suspending sample " + sample.SampleId + " as it is not mainstream enabled.");
                        }
                        //If the sample is mainstream enabled we should Update and Insert the correspondent attributes in the Respondent attributes table
                        else
                        {
                            //If sample is still enabled update the respondent attributes and quota remaining, try to activate the offer
                            try
                            {
                                var newStatus = sampleManager.Activate(sample.SampleId, sample.Id);
                                if (newStatus == (int)OfferStatus.Pending)
                                {
                                    new OfferRepository().UpdateRetryCount(sample.SampleId, 0);
                                }
                            }
                            // If offer activation throws an exception (E.G. connection was lost while trying to get quota cells), keep offer status as active
                            catch
                            {
                                offerRep.UpdateOfferStatus(sample.SampleId, (int)OfferStatus.Active);
                            }
                        }

                        //Clear the dictionaries so we don't have duplicate keys
                        errors.Clear();
                        parameters.Clear();
                    }
                }
            }
            log.Info("ActiveOffersJob ended processing at " + DateTime.Now);
        }
    }
}
