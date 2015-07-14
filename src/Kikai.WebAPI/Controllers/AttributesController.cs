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
using Kikai.WebApi.Managers.IManagers;
using Kikai.WebApi.Managers;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Kikai.WebApi.Controllers
{
    public class AttributesController : ApiController
    {
        #region Properties

        Dictionary<string, string> parameters;
        private readonly string OperationType = operationType.WS.ToString();
        private string OperationName;
        private IAttributesManager AttributesManager;

        #endregion

        #region Constructors

        //Default Constructor
        public AttributesController()
        {
            parameters = new Dictionary<string, string>();
            this.AttributesManager = new AttributesManager();
        }

        //Xunit Constructor
        public AttributesController(IAttributesManager IAttributesManager)
        {
            parameters = new Dictionary<string, string>();
            this.AttributesManager = IAttributesManager;
        }

        //Automation Constructor
        public AttributesController(IAttributesManager IAttributesManager, HttpRequestMessage Request, HttpRequestContext RequestContext)
        {
            parameters = new Dictionary<string, string>();
            this.RequestContext = RequestContext;
            this.Request = Request;
            this.AttributesManager = IAttributesManager;
        }

        #endregion

        #region Web Service Calls

        //api/public/Attributes
        ///<summary>
        ///Returns a collection of published attributes.
        ///</summary>
        ///<returns>
        ///Name: Name of the attribute|
        ///Question: Question associated with the attribute|
        ///</returns>
        [ResponseType(typeof(AttributeApiObject))]
        public IHttpActionResult GetAttributes()
        {
            MonitorEvent evt = new MonitorEvent("OfferService");
            int status = 1;
            CodebookResponse _responseEnvelope = new CodebookResponse(Request.Properties["requestId"].ToString(), true);
            String apiUser = RequestContext.Principal.Identity.Name;
            OperationName = operationName.GetAttributes.ToString();

            //Get the attributes and put them into the response envelope
            try
            {
                _responseEnvelope.Data = AttributesManager.GetPublishedAttributes(Request, apiUser);
            }
            catch (Exception e)
            {
                //Edit for R184
                _responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                LoggerFactory.GetLogger().InfoJson(new Methods().Exception_ToLogObject(_responseEnvelope.Meta.Id, apiUser, OperationType, OperationName, e));
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

                evt.Log(_responseEnvelope.Meta.Id, "GetAttributeList", status);
            }

            return Ok(_responseEnvelope);
        }

        // GET api/public/Attributes/id
        ///<summary>
        ///Returns information about a specific attribute.
        ///</summary>
        ///<param name="id">Attribute Id</param>
        ///<returns>
        ///Name: Name of the attribute|
        ///Question: Question associated with the attribute|
        ///Type: Type of the attribute|
        ///AttributeOptions: The attribute Options
        ///</returns>
        [ResponseType(typeof(AttributeDetailsApiObject))]
        public IHttpActionResult GetAttribute([FromUri] string id)
        {
            OperationName = operationName.GetAttributeOptions.ToString();
            MonitorEvent evt = new MonitorEvent("OfferService");
            String apiUser = RequestContext.Principal.Identity.Name;
            int status = 1;
            AttributeResponse _responseEnvelope = new AttributeResponse(Request.Properties["requestId"].ToString(), true);
            parameters.Add("AttributeId", id);
            try
            {
                _responseEnvelope.Data = AttributesManager.GetAttribute(Request, apiUser, id);
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

                evt.Log(_responseEnvelope.Meta.Id, "GetAttributeOptions", status);
            }
            return Ok(_responseEnvelope);
        }

        #endregion
    }
}
