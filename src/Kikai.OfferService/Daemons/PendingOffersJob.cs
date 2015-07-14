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
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace Kikai.OfferService.Daemons
{
    [DisallowConcurrentExecution]
    public class PendingOffersJob : IJob
    {
        Dictionary<string, string> parameters;

        public PendingOffersJob()
        {
            parameters = new Dictionary<string, string>();
        }

        public void Execute(IJobExecutionContext context)
        {
            LoggerFactory.SetThreadLogger("PendingOffersJob");
            LoggingUtility log = LoggerFactory.GetLogger();
            log.Info("PendingOffersJob started processing at " + DateTime.Now);
            SampleManager sampleManager = null;
            var responseSuccessfull = false;
            var maxRetries = Convert.ToInt32(ConfigurationManager.AppSettings["PendingOffersJobMaxRetries"]);
            IOfferRepository offerRep = new OfferRepository();
            Stopwatch sw = new Stopwatch();

            var maxSamplePerRequest = 150;

            //Getting all the pending samples
            var pendingSamples = offerRep.GetPendingSampleIdsAndRetryCount();

            //If there are at least 1 pending sample
            if (pendingSamples.Count() > 0)
            {
                for (var i = 0; i < pendingSamples.Count(); i += maxSamplePerRequest)
                {
                    bool responseHasUnwantedError = true;
                    //takes the maximum number of samples allowed in the URL
                    var currentSamples = pendingSamples.Skip(i).Take(maxSamplePerRequest).ToList();
                    var errors = new List<ErrorObject>();

                    foreach (var sample in currentSamples)
                    {
                        offerRep.UpdateRetryCount(sample.SampleId, ++sample.RetryCount);
                        log.Info("Attempt #" + sample.RetryCount + " to update the status of Offer having sampleId: " + sample.SampleId);
                    }

                    try
                    {
                        sw.Start();
                        sampleManager = new SampleManager(new JobUtil().SampleListAsString(currentSamples));
                        sw.Stop();
                        if (sw.ElapsedMilliseconds > 30000)
                        {
                            log.Warn("SAM connection returned response after " + sw.ElapsedMilliseconds + " milliseconds.");
                        }
                        responseSuccessfull = true;
                        responseHasUnwantedError = sampleManager.ResponseHasUnwantedErrors();
                    }
                    catch
                    {
                        //If the service threw an error it must be a connection error
                        responseSuccessfull = false;
                        log.Error("Offer service could not connect to SAM for unknown reasons.");
                    }


                    //Loop through the pending samples

                    foreach (var sample in currentSamples)
                    {
                        var suspend = false;
                        parameters.Add("SampleId", sample.SampleId.ToString());
                        //If SAM response was successful update the sample accordingly
                        if (responseSuccessfull && !responseHasUnwantedError)
                        {
                            var isMainstreamEnabled = sampleManager.CheckIfSampleExist((int)sample.SampleId);
                            if (isMainstreamEnabled)
                            {
                                //If sample is still enabled update the respondent attributes and quota remaining
                                var newStatus = (int)OfferStatus.Pending;
                                // Try to activate the offer
                                try
                                {
                                    newStatus = sampleManager.Activate(sample.SampleId, sample.Id);
                                }
                                // If offer activation throws an exception (E.G. connection was lost while trying to get quota cells), keep offer status as pending
                                catch
                                {
                                    newStatus = (int)OfferStatus.Pending;
                                }
                                // If the new offer status isn't active and the retry count exceeded the max retries ==> Suspend the offer
                                finally
                                {
                                    if (newStatus != (int)OfferStatus.Active && sample.RetryCount >= maxRetries)
                                    {
                                        suspend = true;
                                        //Log that the sample id will be suspended because the current status is not active and the maximum retries is reached
                                        log.Info("Suspending sample " + sample.SampleId + " as the maximum retry count is reached.");
                                    }
                                }
                            }
                            else
                            {
                                suspend = true;
                                //Log that the sample id will be suspended because the sample was not mainstream enabled
                                log.Info("Suspending sample " + sample.SampleId + " as it is not mainstream enabled.");
                            }
                        }
                        // If SAM response failed update the retry count
                        // Or response has unwanted errors like timeout or unexpected sql error
                        // wen check if the sample has hit the max retries count
                        // and suspend it if it did
                        else
                        {
                            //If the max retries is reached suspend the offer
                            if (sample.RetryCount >= maxRetries)
                            {
                                suspend = true;
                                //Log that the sample id will be suspended because the max retries is reached
                                log.Info("Suspending sample " + sample.SampleId + " as the maximum retry count is reached.");
                            }
                        }

                        //If suspend flag is true update the row in the database
                        if (suspend)
                        {
                            offerRep.UpdateOfferStatus(sample.SampleId, (int)OfferStatus.Suspended);
                        }
                        errors.Clear();
                        parameters.Clear();
                    }
                }
            }
            log.Info("PendingOffersJob ended processing at " + DateTime.Now);
        }
    }
}
