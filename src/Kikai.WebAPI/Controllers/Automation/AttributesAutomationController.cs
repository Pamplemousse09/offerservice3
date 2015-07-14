using Kikai.BL.DTO.ApiObjects;
using Kikai.Domain.Common;
using Kikai.WebApi.Decorators;
using Kikai.Logging.DTO;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Kikai.WebApi.Managers;
using Kikai.WebApi.Managers.IManagers;
using Kikai.BL.DTO;
using Moq;
using System.Web.Http.Routing;
using Kikai.WebAPI.Managers;
using System.Web.Http.Results;

namespace Kikai.WebApi.Controllers.Automation
{
    public class AttributesAutomationController : ApiController
    {
        static IAttributesManager AttributesManager;

        [ResponseType(typeof(AttributeApiObject))]
        public IHttpActionResult GetAttributes()
        {
            CodebookResponse _responseEnvelope = new CodebookResponse(Request.Properties["requestId"].ToString(), true);
            String apiUser = RequestContext.Principal.Identity.Name;
            AutomationManager.AttributeObjectToAttributeApiObject(AutomationManager.attributes);
            AttributesManager = new AttributesManager(AutomationManager.IAttributeRepository.Object);
            var result = new AttributesController(AttributesManager, this.Request, this.RequestContext).GetAttributes() as OkNegotiatedContentResult<CodebookResponse>;
            return Ok(result.Content);
        }


        [ResponseType(typeof(AttributeDetailsApiObject))]
        public IHttpActionResult GetAttribute([FromUri] string id)
        {
            String apiUser = RequestContext.Principal.Identity.Name;
            AttributeResponse _responseEnvelope = new AttributeResponse(Request.Properties["requestId"].ToString(), true);
            AutomationManager.AttributeObjectToAttributeDetailsApiObject(AutomationManager.attributes);
            AttributesManager = new AttributesManager(AutomationManager.IAttributeRepository.Object);
            var result = new AttributesController(AttributesManager, this.Request, this.RequestContext).GetAttribute(id) as OkNegotiatedContentResult<AttributeResponse>;
            return Ok(result.Content);
        }
    }
}
