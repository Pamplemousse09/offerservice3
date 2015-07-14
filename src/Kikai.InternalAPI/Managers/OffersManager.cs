using Kikai.BL.Concrete;
using Kikai.BL.IRepository;
using Kikai.Domain.Common;
using Kikai.InternalAPI.DTO;
using Kikai.InternalAPI.Managers.IManagers;
using Kikai.Logging.DTO;
using Kikai.Logging.Extensions;
using Kikai.Logging.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Kikai.InternalAPI.Managers
{
    public class OffersManager : IOffersManager
    {

        #region Private Variables
        private IOfferRepository IOfferRepository;
        private readonly string OperationType = operationType.InternalAPI.ToString();
        private bool xunit = false;
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <returns></returns>
        public OffersManager()
        {
        }

        /// <summary>
        /// Xunit constructor
        /// </summary>
        /// <param name="IOfferRepository"></param>
        public OffersManager(IOfferRepository IOfferRepository)
        {
            this.xunit = true;
            this.IOfferRepository = IOfferRepository;
        }

        #endregion

        #region Repository Calls
        private IOfferRepository OfferRepository()
        {
            if (xunit)
                return IOfferRepository;
            else
                return new OfferRepository();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Function that will return offers based on the study id supplied.
        /// If StudyId is null, it will fetch all the offers from the database and return them
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="StudyId"></param>
        /// <param name="ApiUser"></param>
        /// <returns></returns>
        public OffersStudyIdDataObject GetOffersBySudyId(HttpRequestMessage Request, string StudyId, string ApiUser)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            OffersStudyIdDataObject data = new OffersStudyIdDataObject();
            String requestId = Request.Properties["requestId"].ToString();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string OperationName = operationName.GetOffersByStudyId.ToString();
            int sid;
            try
            {
                parameters.Add("StudyId", StudyId);
                bool isNumeric = Int32.TryParse(StudyId, out sid);
                if (StudyId == null || isNumeric)
                {
                    data.Offers = OfferRepository().SelectByStudyId(StudyId).ToList();
                    if (StudyId == null && data.Offers.Count == 0)
                    {
                        data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_API_NO_OFFERS, parameters));
                    }
                    else if (data.Offers.Count == 0)
                    {
                        data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_API_INVALID_STUDY, parameters));
                    }
                }
                else if (StudyId != null && !isNumeric)
                {
                    data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_API_INVALID_STUDY, parameters));
                }
            }

            catch (Exception e)
            {
                data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                log.InfoJson(new Methods().Exception_ToLogObject(requestId, ApiUser, OperationType, OperationName, e));
            }
            finally
            {
                if (data.Errors.Count != 0)
                {
                    log.InfoJson(new Methods().Error_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data.Errors));
                    log.ProcessingDebug(requestId, OperationName + " request was unsuccessful.");
                }

                //The response has no errors, we insert a request successful message into the logs
                else
                {
                    log.InfoJson(new Methods().Response_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data));
                    log.ProcessingDebug(requestId, OperationName + " request was successful.");
                }
            }
            return data;
        }

        #endregion
    }
}