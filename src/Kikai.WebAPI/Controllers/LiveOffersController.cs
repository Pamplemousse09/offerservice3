using Kikai.BL.DTO.ApiObjects;
using Kikai.Domain.Common;
using Kikai.Logging.Utils;
using Kikai.WebApi.Decorators;
using Kikai.Logging.DTO;
using Kikai.Logging.Extensions;
using Monitoring.ApplicationMonitor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Kikai.WebApi.Managers;
using Kikai.WebApi.Managers.IManagers;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Kikai.WebApi.Controllers
{
    public class LiveOffersController : ApiController
    {
        #region Properties

        Dictionary<string, string> parameters;
        private readonly string OperationType = operationType.WS.ToString();
        private string OperationName;
        private IOffersManager OffersManager;

        #endregion

        #region Constructors

        //Default Constructor
        public LiveOffersController()
        {
            parameters = new Dictionary<string, string>();
            this.OffersManager = new OffersManager();
        }

        //Xunit Constructor
        public LiveOffersController(IOffersManager IOffersManager)
        {
            parameters = new Dictionary<string, string>();
            this.OffersManager = IOffersManager;
        }

        //Automation Constructor
        public LiveOffersController(IOffersManager IOffersManager, HttpRequestMessage Request, HttpRequestContext RequestContext)
        {
            parameters = new Dictionary<string, string>();
            this.RequestContext = RequestContext;
            this.RequestContext.RouteData = new HttpRouteData(RequestContext.RouteData.Route, new HttpRouteValueDictionary { { "controller", "liveoffers" } });
            this.Request = Request;
            this.OffersManager = IOffersManager;
        }

        #endregion

        #region Web Service Calls

        /// GET api/public/LiveOffers
        /// <summary>
        /// Gets the list of all available LIVE offers
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(OfferApiObject))]
        public IHttpActionResult Get()
        {
            MonitorEvent evt = new MonitorEvent("OfferService");
            int status = 1;
            String apiUser = RequestContext.Principal.Identity.Name;
            OffersResponse _responseEnvelope = new OffersResponse(Request.Properties["requestId"].ToString(), true);
            OperationName = operationName.GetAllLiveOffers.ToString();

            try
            {
                _responseEnvelope.Data = OffersManager.GetOffers(Request, apiUser);
            }
            catch (Exception e)
            {
                //In case of an internal application error, we will end up with the internal error id
                //In case of an internal web service call, we will end up with the back-end unavailable error id
                //Edit for R184
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(errorKey));
                //We add the exception with the stack-trace in the OfferServiceLog file
                LoggerFactory.GetLogger().ErrorJson(new Methods().Exception_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, OperationType, OperationName, e));
                status = 0;
            }
            finally
            {
                if (_responseEnvelope.Data.Errors.Count > 0)
                {
                    if (_responseEnvelope.Data.Errors.Exists(i => i.Id == ErrorKey.ERR_INTERNAL_FATAL))
                        status = 0;
                    _responseEnvelope.Status = false;
                }
                else
                    _responseEnvelope.Data.Errors = null;

                evt.Log(_responseEnvelope.Meta.Id, "GetOffers", status);
            }

            return Ok(_responseEnvelope);
        }

        /// GET api/public/LiveOffers?pid={id}&request_attributes={values}
        ///<summary>
        ///Returns a collection of active live offers suitable for displaying on an offer wall filtered by Pid and request attributes.
        ///Currently the language and country parameters are required. They are not present in the API Uri because the set of required attributes is configurable and can change.
        ///</summary>
        ///<param name="pid">External PID of the respondent</param>
        ///<param name="country">Country code of respondent</param>
        ///<param name="language">Language code of respondent</param>
        ///<returns>
        ///Topic: Type of offer|
        ///Title: Short description of offer|
        ///Description: More detailed description of offer|
        ///LOI: Length of interview in minutes|
        ///IR: Incidence Rate (1-100)|
        ///CPI: Amount paid per complete in dollars|
        ///OfferID: Unique ID for offer|
        ///TermID: Unique ID for term|
        ///OfferLink: Beginning link for offer. Extra parameters may be appended to URI|
        ///QuotaRemaining: Quota Remaining for the offer|
        ///</returns>
        public IHttpActionResult Get([FromUri]string pid)
        {
            MonitorEvent evt = new MonitorEvent("OfferService");
            int status = 1;
            OperationName = operationName.GetOffersPID.ToString();
            String apiUser = RequestContext.Principal.Identity.Name;
            OffersResponse _responseEnvelope = new OffersResponse(Request.Properties["requestId"].ToString(), true);

            try
            {
                _responseEnvelope.Data = OffersManager.GetOffersByPid(Request, apiUser, pid);
            }
            catch (Exception e)
            {
                //In case of an internal application error, we will end up with the internal error id
                //In case of an internal web service call, we will end up with the back-end unavailable error id
                //Edit for R184
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(errorKey));
                //We add the exception with the stack-trace in the OfferServiceLog file
                LoggerFactory.GetLogger().ErrorJson(new Methods().Exception_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, OperationType, OperationName, e));
                status = 0;
            }
            finally
            {
                if (_responseEnvelope.Data.Errors.Count > 0)
                {
                    if (_responseEnvelope.Data.Errors.Exists(i => i.Id == ErrorKey.ERR_INTERNAL_FATAL))
                        status = 0;
                    _responseEnvelope.Status = false;
                }
                else
                    _responseEnvelope.Data.Errors = null;

                evt.Log(_responseEnvelope.Meta.Id, "GetOffersPID", status);
            }

            return Ok(_responseEnvelope);
        }

        /// <summary>
        /// Gets the list of respondent attributes of a live offer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("Attributes")]
        [ResponseType(typeof(OfferAttributeApiObject))]
        public IHttpActionResult GetAttributes(string id)
        {
            MonitorEvent evt = new MonitorEvent("OfferService");
            int status = 1;
            String apiUser = RequestContext.Principal.Identity.Name;
            OfferAttributesResponse _responseEnvelope = new OfferAttributesResponse(Request.Properties["requestId"].ToString(), true);

            try
            {
                _responseEnvelope.Data = OffersManager.GetOfferAttributes(Request, apiUser, id);
            }
            catch (Exception e)
            {
                //Edit for R184
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                LoggerFactory.GetLogger().InfoJson(new Methods().Exception_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, OperationType, OperationName, e));
                status = 0;
            }
            finally
            {
                if (_responseEnvelope.Data.Errors.Count > 0)
                {
                    if (_responseEnvelope.Data.Errors.Exists(i => i.Id == ErrorKey.ERR_INTERNAL_FATAL))
                        status = 0;
                    _responseEnvelope.Status = false;
                }
                else
                    _responseEnvelope.Data.Errors = null;

                evt.Log(_responseEnvelope.Meta.Id, "GetOffersAttributes", status);
            }

            return Ok(_responseEnvelope);
        }

        /// GET /api/public/LiveOffers/{offerId}/QuotaCells
        ///<summary>
        ///Returns a collection of live offer quota expressions
        ///</summary>
        ///<param name="id">Offer ID</param>
        ///<returns>
        ///QuotaExpressions: Quota expressions
        ///</returns>
        [ActionName("QuotaCells")]
        [ResponseType(typeof(QuotaExpressionsApiObject))]
        public IHttpActionResult GetQuotaExpressions(string id)
        {
            OfferQuotaCellsResponse _responseEnvelope = new OfferQuotaCellsResponse(Request.Properties["requestId"].ToString(), true);
            int status = 1;
            String apiUser = RequestContext.Principal.Identity.Name;
            MonitorEvent evt = new MonitorEvent("OfferService");

            try
            {
                _responseEnvelope.Data = OffersManager.GetOfferQuotaExpression(Request, apiUser, id);
            }
            catch (Exception e)
            {
                //Edit for R184
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                LoggerFactory.GetLogger().InfoJson(new Methods().Exception_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, OperationType, OperationName, e));
                status = 0;
            }
            finally
            {
                if (_responseEnvelope.Data.Errors != null && _responseEnvelope.Data.Errors.Count > 0)
                {
                    if (_responseEnvelope.Data.Errors.Exists(i => i.Id == ErrorKey.ERR_INTERNAL_FATAL))
                        status = 0;
                    _responseEnvelope.Status = false;
                }
                else
                    _responseEnvelope.Data.Errors = null;
                evt.Log(_responseEnvelope.Meta.Id, "GetOfferQuotaCells", status);
            }

            return Ok(_responseEnvelope);
        }

        #endregion
    }
}
