using Kikai.BL.Concrete;
using Kikai.BL.DTO.ApiObjects;
using Kikai.Domain.Common;
using Kikai.Logging.Utils;
using Kikai.WebApi.Decorators;
using Kikai.Logging.DTO;
using Kikai.Logging.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kikai.WebApi.Controllers
{
    public class RpcController : ApiController
    {
        Dictionary<string, string> parameters;
        private readonly string OperationType = operationType.Hub.ToString();
        private readonly string OperationName = operationName.GetOfferAttributeUsage.ToString();

        public RpcController()
        {
            parameters = new Dictionary<string, string>();
        }

        // GET RpcService
        ///<summary>
        ///Returns the term status given offerID and termId
        ///</summary>
        ///<param name="OfferId">Unique ID for the Offer</param>
        ///<param name="TermsId">Unique ID for the term</param>
        ///<returns>
        ///Status: Term status|
        ///Error: In case of invalid Offer ID or Term ID or Term ID expiration date latest then 30 minutes|
        ///LivematchAttributeUsage: List of the attributes names for the passed OfferId |
        ///Offer data: OfferId, OfferType, StudyId, SampleId|
        ///</returns>
        public IHttpActionResult GetOfferAttributeUsage(string OfferId, string TermsId)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            RpcOfferResponse _responseEnvelope = new RpcOfferResponse(Request.Properties["requestId"].ToString(), true);
            var oid = new Guid();
            var tid = new Guid();
            parameters.Add("OfferId", OfferId);
            parameters.Add("TermId", TermsId);
            log.ProcessingDebug(_responseEnvelope.Meta.Id, "Received GET OfferAttributeUsage request.");
            try
                {
            //Test if one of the required parameters is not supplied in the URL
            if (OfferId == null || TermsId == null)
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_HUB_MISSING_DATA_ARGUMENTS, parameters));
            }

            //Test if given offer id is of type GUID
            else if (Guid.TryParse(OfferId, out oid) == false)
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_HUB_INVALID_OFFER_ARGUMENT, parameters));
            }

            //Test if given term id is of type GUID
            else if (Guid.TryParse(TermsId, out tid) == false)
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_HUB_INVALID_TERM_ARGUMENT, parameters));
            }
            else
            {

                    log.ProcessingDebug(_responseEnvelope.Meta.Id, "Getting requested offer and term from the database.");
                    var ms = new OfferRepository().SelectByID(oid);
                    //There is no offer with this id, add an error
                    if (ms == null)
                        _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_HUB_INVALID_OFFER_ARGUMENT, parameters));
                    //Offer is inactive or suspended
                    else if (ms.Status != (int)OfferStatus.Active)
                        _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_HUB_OFFER_SUSPENDED_INACTIVE, parameters));
                    //Check if the provided term exists for the given offer
                    else if (new TermRepository().CheckTermForOffer(tid, oid) == false)
                    {
                        _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_HUB_INVALID_TERM_ARGUMENT, parameters));
                    }

                    else
                    {
                        //Check term if still valid
                        log.ProcessingDebug(_responseEnvelope.Meta.Id, "Checking if requested term has expired.");
                        if (new TermRepository().CheckTermValidity(tid) == null)
                        {
                            _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_HUB_TERM_EXPIRED, parameters));
                        }
                        //Add offer data to response
                        else
                        {
                            log.ProcessingDebug(_responseEnvelope.Meta.Id, "Getting requested offer attributes from the database.");
                            //_responseEnvelope.Data.Add(new RpcApiObject(new OfferAttributeRepository().GetRpcOfferAttributes(oid).ToList(), new RpcOfferApiDetails(ms.Id, ms.SampleId, ms.StudyId)));
                            _responseEnvelope.Data.LivematchAttributeUsage = new OfferAttributeRepository().GetRpcOfferAttributes(oid).ToList();
                            _responseEnvelope.Data.Offer = new RpcOfferDetailsApiObject(ms.Id, ms.StudyId, ms.SampleId);
                        }
                    }
                }             
            }
            catch (Exception e)
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                log.InfoJson(new Methods().Exception_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, operationType.WS.ToString(), operationName.GetAllLiveOffers.ToString(), e));
            }
            finally
            {
                //If the response has errors, insert an error message into the logs
                if (_responseEnvelope.Data.Errors.Count != 0)
                {
                    log.InfoJson(new Methods().Error_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, OperationType, OperationName, parameters, _responseEnvelope.Data.Errors));
                    log.ProcessingDebug(_responseEnvelope.Meta.Id, "GET OfferAttributeUsage request was unsuccessful.");
                    _responseEnvelope.Status = false;
                }

                //The response has no errors, we insert a request successful message into the logs
                else
                {
                    _responseEnvelope.Data.Errors = null;
                    log.InfoJson(new Methods().Response_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, OperationType, OperationName, parameters, _responseEnvelope.Data));
                    log.ProcessingDebug(_responseEnvelope.Meta.Id, "GET OfferAttributeUsage request was successful.");
                }
            }

            return Ok(_responseEnvelope);
        }
    }
}
