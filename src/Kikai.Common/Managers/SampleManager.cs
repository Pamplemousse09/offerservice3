using Kikai.BL.Concrete;
using Kikai.Domain.Common;
using Kikai.Logging.Utils;
using Kikai.Internal.Contracts.Objects;
using Kikai.Internal.IManagers;
using Kikai.Internal.Managers;
using Kikai.Common.IManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kikai.Common.Managers
{
    public class SampleManager : ISampleManager
    {
        private const int UnexpectedSqlErrorCode = -1;
        private const int TimeoutErrorCode = -2;
        private const int SampleNotMainstreamEnabledErrorCode = -3;

        private string sampleList;
        private OpenSampleAttributeResponseObject openSampleAttribute = new OpenSampleAttributeResponseObject();

        public SampleManager(string sampleList)
        {
            try
            {
                this.sampleList = sampleList;
                openSampleAttribute = new Sam().ProcessGetOpenSampleAttributes(sampleList);
            }
            catch
            {
                throw;
            }
        }

        public bool CheckIfSampleExist(int sampleId)
        {
            return (this.openSampleAttribute.OpenSampleAttributeList.Where(o => o.SampleId == sampleId).Count() > 0);
        }

        /// <summary>
        /// Function that checks if SAM response has unwanted errors.
        /// Unwanted errors are:
        /// -1 : Unexpected sql error occurred
        /// -2 : Request Timeout, Try Again
        /// </summary>
        /// <returns></returns>
        public bool ResponseHasUnwantedErrors()
        {
            bool hasUnwantedErrors = false;
            if (this.openSampleAttribute != null && this.openSampleAttribute.ServiceErrorList != null)
            {
                var ErrorList = this.openSampleAttribute.ServiceErrorList.ToList();

                if ((ErrorList.Exists(i => Convert.ToInt32(i.code) == TimeoutErrorCode)) || (ErrorList.Exists(i => Convert.ToInt32(i.code) == UnexpectedSqlErrorCode)))
                {
                    hasUnwantedErrors = true;
                    LoggerFactory.GetLogger().Info("Sam response for SampleId: " + this.sampleList + " returned with error: " + ErrorList.FirstOrDefault().message + ".");
                }
            }
            return hasUnwantedErrors;
        }

        public OpenSampleObject GetOpenSampleObject(int sampleId)
        {
            if (this.openSampleAttribute.OpenSampleAttributeList.Count() > 0)
            {
                return this.openSampleAttribute.OpenSampleAttributeList.Where(o => o.SampleId == sampleId).First();
            }
            else
            {
                return null;
            }
        }

        public int Activate(int sampleId, Guid offerId) {
            int suspendedCode;
            return Activate(sampleId, offerId, out suspendedCode);
        }

        public int Activate(int sampleId, Guid offerId, out int suspendedCode)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            OfferRepository offerRep = new OfferRepository();
            var newStatus = (int)OfferStatus.Active;
            suspendedCode = 0;

            try
            {
                OpenSampleObject sample = GetOpenSampleObject(sampleId);
                //If the sample was mainstream enabled
                if (sample != null)
                {
                    // update offer attributes in local database based on attributes from SAM
                    new RespondentAttributeRepository().UpdateOrRemove(offerId, sample.Attributes);

                    // update sample quota remaining in local database from SAM
                    offerRep.UpdateQuotaRemaining(sampleId, sample.MainstreamQuotaRemaining);

                    // collect quota cell attributes, and quota cell quota remaining and store in database
                    var QuotaCellRequestSuccess = new QuotaCellManager().InitializeQuotaCells(offerId);

                    // Set the offer as pending if it failed in getting the quota cells
                    if (!QuotaCellRequestSuccess)
                    {
                        //newStatus = (int)OfferStatus.Suspended;
                        suspendedCode = 22;
                        newStatus = (int)OfferStatus.Suspended;
                        log.Info("Suspending the sample " + sample.SampleId + " as the QuotaCell could not be initialized.");
                    }
                }
                //If the sample was not mainstream enabled
                else
                {
                    var ErrorList = this.openSampleAttribute.ServiceErrorList.ToList();
                    //If SAM returned error message in it's response
                    if (ErrorList.Count > 0)
                    {
                        // If error code is for a TimeOut or an unexpected sql error (SAM side)
                        // => Make the offer as pending
                        if (ErrorList.Exists(i => Convert.ToInt32(i.code) == TimeoutErrorCode) || ErrorList.Exists(i => Convert.ToInt32(i.code) == UnexpectedSqlErrorCode))
                        {
                            newStatus = (int)OfferStatus.Pending;
                            log.Info("Making the sample " + sampleId + " as pending as SAM returned an error.");
                        }
                        // If error code is for Sample not mainstream enabled
                        // => Make the offer as suspended
                        else if (ErrorList.Exists(i => Convert.ToInt32(i.code) == SampleNotMainstreamEnabledErrorCode))
                        {
                            //newStatus = (int)OfferStatus.Suspended;
                            suspendedCode = 21;
                            newStatus = (int)OfferStatus.Suspended;
                            log.Info("Suspending the sample " + sampleId + " as it was not found as mainstream enabled.");
                        }

                    }
                    // SAM response doesn't contain an error message, SampleId not found in response
                    // => Suspend the offer
                    else
                    {
                        //newStatus = (int)OfferStatus.Suspended;
                        suspendedCode = 21;
                        newStatus = (int)OfferStatus.Suspended;
                        log.Info("Suspending the sample " + sampleId + " as it was not found as mainstream enabled.");
                    }
                }
            }
            catch (Exception e)
            {
                if (e is ThreadAbortException)
                    newStatus = (int)OfferStatus.Inactive;
                else
                    newStatus = (int)OfferStatus.Pending;

                log.Error(@"A problem occurred while trying to activate the offer: " + offerId.ToString() + " Sample Id: " + sampleId + ".", e);
            }
            finally
            {
                offerRep.UpdateOfferStatus(sampleId, newStatus);
            }
            return newStatus;
        }
    }
}
