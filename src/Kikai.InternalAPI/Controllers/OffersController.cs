using Kikai.Domain.Common;
using Kikai.InternalAPI.Decorators;
using Kikai.InternalAPI.Extensions;
using Kikai.InternalAPI.Managers;
using Kikai.InternalAPI.Managers.IManagers;
using Kikai.Logging.DTO;
using Kikai.Logging.Extensions;
using Kikai.Logging.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;

namespace Kikai.InternalAPI.Controllers
{
    public class OffersController : ApiController
    {
        #region Private Variables

        Dictionary<string, string> parameters;
        private readonly string OperationType = operationType.WS.ToString();
        private string OperationName;
        private IOffersManager OffersManager;

        #endregion

        #region Constructors

        //Default Constructor
        public OffersController()
        {
            parameters = new Dictionary<string, string>();
            this.OffersManager = new OffersManager();
        }

        #endregion

        #region Methods

        // GET api/offers
        public IHttpActionResult Get()
        {
            var action = new InternalAPIMethods().FilterControllerAction(Request);
            switch (action)
            {
                case "getoffersbystudyid":
                    return GetOffersByStudyId();
                default:
                    return MissingMethod();
            }
        }

        public IHttpActionResult MissingMethod()
        {
            ErrorResponse _responseEnvelope = new ErrorResponse(Request.Properties["requestId"].ToString(), false);
            _responseEnvelope.Data.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_API_MISSING_METHOD));
            return Ok(_responseEnvelope);
        }

        [HttpGet]
        [ActionName("GetOffersByStudyId")]
        public IHttpActionResult GetOffersByStudyId()
        {
            string studyId;
            var QueryValues = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key.ToLower(), x => x.Value);
            QueryValues.TryGetValue("studyid", out studyId);
            String apiUser = RequestContext.Principal.Identity.Name;
            OffersStudyIdResponse _responseEnvelope = new OffersStudyIdResponse(Request.Properties["requestId"].ToString(), true);
            OperationName = operationName.GetOffersByStudyId.ToString();
            studyId = !String.IsNullOrEmpty(studyId) ? studyId : null;
            try
            {
                _responseEnvelope.Data = OffersManager.GetOffersBySudyId(Request, studyId, apiUser);
            }
            catch (Exception e)
            {
                //In case of an internal application error, we will end up with the internal error id
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));

                //We add the exception with the stack-trace in the OfferServiceLog file
                LoggerFactory.GetLogger().ErrorJson(new Methods().Exception_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, OperationType, OperationName, e));
            }
            finally
            {
                if (_responseEnvelope.Data.Errors.Count > 0)
                    _responseEnvelope.Status = false;
                else
                    _responseEnvelope.Data.Errors = null;
                if (_responseEnvelope.Data.Offers != null && _responseEnvelope.Data.Offers.Count == 0)
                    _responseEnvelope.Data.Offers = null;
            }

            return Ok(_responseEnvelope);
        }

        #endregion

        /*---TO DO: Add CRUD Operations -- 
         
        // GET api/offers/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/offers
        public void Post([FromBody]string value)
        {
        }

        // PUT api/offers/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/offers/5
        public void Delete(int id)
        {
        }
         
        */
    }
}
