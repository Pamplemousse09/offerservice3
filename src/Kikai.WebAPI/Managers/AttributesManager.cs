using Kikai.BL.Concrete;
using Kikai.BL.DTO.ApiObjects;
using Kikai.BL.IRepository;
using Kikai.Domain.Common;
using Kikai.Logging.DTO;
using Kikai.Logging.Extensions;
using Kikai.Logging.Utils;
using Kikai.WebApi.DTO;
using Kikai.WebApi.Managers.IManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Kikai.WebApi.Managers
{
    public class AttributesManager : IAttributesManager
    {
        private IAttributeRepository AttributeRespository;
        private readonly string OperationType = operationType.WS.ToString();
        Dictionary<string, string> parameters;

        public AttributesManager()
        {
            this.AttributeRespository = new AttributeRepository();
            parameters = new Dictionary<string, string>();
        }

        public AttributesManager(IAttributeRepository IAttributeRepository)
        {
            //log = LoggerFactory.GetLogger("Xunit");
            //detailedLog = LoggerFactory.GetLogger("Xunit");
            this.AttributeRespository = IAttributeRepository;
            parameters = new Dictionary<string, string>();
        }

        public CodebookDataObject GetPublishedAttributes(HttpRequestMessage Request, string ApiUser)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            String requestId = Request.Properties["requestId"].ToString();
            CodebookDataObject data = new CodebookDataObject();
            string OperationName = operationName.GetAttributes.ToString();
            log.ProcessingDebug(requestId, "Received GET Attributes request.");
            try
            {
                log.ProcessingDebug(requestId, "Getting published attributes from database.");
                var attributes = AttributeRespository.SelectPublishedAttributes().ToList();
                if (attributes != null && attributes.Count > 0)
                {
                    data.Attributes = attributes;
                }
                else
                {
                    data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_NO_PUBLISHED_ATTRIBUTES));
                }
            }
            catch (Exception e)
            {
                //Edit for R184
                data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                log.InfoJson(new Methods().Exception_ToLogObject(requestId, ApiUser, OperationType, OperationName, e));
            }
            finally
            {
                //If the response has errors, insert an error message into the logs
                if (data.Errors.Count != 0)
                {
                    log.InfoJson(new Methods().Error_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data.Errors));
                    log.ProcessingDebug(requestId, "GET Attributes request was unsuccessful.");
                }

                //The response has no errors, we insert a request successful message into the logs
                else
                {
                    log.InfoJson(new Methods().Response_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data));
                    log.ProcessingDebug(requestId, "GET Attributes request was successful.");
                }
            }
            return data;
        }

        public AttributeDataObject GetAttribute(HttpRequestMessage Request, string ApiUser, string AttributeId)
        {
            LoggingUtility log = LoggerFactory.GetLogger();
            String requestId = Request.Properties["requestId"].ToString();
            AttributeDataObject data = new AttributeDataObject();
            string OperationName = operationName.GetAttributeOptions.ToString();
            log.ProcessingDebug(requestId, "Received GET Attribute details request.");
            parameters.Add("AttributeId", AttributeId);
            try
            {
                log.ProcessingDebug(requestId, "Getting Attribute details from database.");
                var attribute = AttributeRespository.GetAttributeDetails(AttributeId);
                if (attribute == null)
                {
                    //Edit for R184
                    data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_INVALID_ATTRIBUTE_ARGUMENT, parameters));
                    //R185 Modification
                    //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_PROVIDER_INVALID_ATTRIBUTE_ARGUMENT, parameters));
                }
                else
                {
                    data.Attribute = attribute;
                }
            }
            catch (Exception e)
            {
                //Edit for R184
                data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                //R185 Modification
                //_responseEnvelope.Data.Errors.Add(new ErrorObject(ErrorKey.ERR_INTERNAL_FATAL));
                log.InfoJson(new Methods().Exception_ToLogObject(requestId, ApiUser, OperationType, OperationName, e));
            }
            finally
            {
                //If the response has errors, insert an error message into the logs
                //Edit for R184
                if (data.Errors != null)
                {
                    log.InfoJson(new Methods().Error_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data.Errors));
                    log.ProcessingDebug(requestId, "GET Attribute details request was unsuccessful.");
                }
                //R185 Modification
                //if (_responseEnvelope.Data.Errors.Count != 0)
                //{
                //    log.InfoJson(new Methods().Error_ToLogObject(_responseEnvelope.Meta.Id, RequestContext.Principal.Identity.Name, OperationType, OperationName, parameters, _responseEnvelope.Data.Errors));
                //    detailedLog.ProcessingDebug(_responseEnvelope.Meta.Id, "GET Offer attributes request was unsuccessful.");
                //    _responseEnvelope.Status = false;
                //}

                //The response has no errors, we insert a request successful message into the logs
                else
                {
                    log.InfoJson(new Methods().Response_ToLogObject(requestId, ApiUser, OperationType, OperationName, parameters, data));
                    log.ProcessingDebug(requestId, "GET Attribute details was successful.");
                }
            }

            return data;
        }
    }
}
