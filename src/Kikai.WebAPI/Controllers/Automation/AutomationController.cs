using System.Threading.Tasks;
using System.Web.Http;
using Kikai.BL.DTO;
using Kikai.WebAPI.Managers;
using Kikai.WebAPI.Decorators;
using Kikai.Logging.DTO;

namespace Kikai.WebApi.Controllers.Automation
{
    public class AutomationController : ApiController
    {
        [HttpPost]
        [ActionName("InsertOffer")]
        [Route("api/auto/manage/InsertOffer")]
        public IHttpActionResult InsertOffer(WebServiceResponse query)
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            string status = "Error adding offer.";
            if (AutomationManager.InsertOffer(query))
            {
                _responseEnvelope.Data.Info = "Offer successfully added";
                _responseEnvelope.Data.Errors = null;
            }
            else
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject("-1", "ErrorAutomation", "-1", status + "Invalid Offer Id"));
                _responseEnvelope.Status = false;
            }
            return Ok(_responseEnvelope);
        }

        [HttpPost]
        [ActionName("InsertAttribute")]
        [Route("api/auto/manage/InsertAttribute")]
        public IHttpActionResult InsertAttribute(WebServiceResponse query)
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            if (AutomationManager.InsertAttribute(query))
            {
                _responseEnvelope.Data.Info = "Attribute successfully added.";
                _responseEnvelope.Data.Errors = null;
            }
            else
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject("-1", "ErrorAutomation", "-1", "Error adding attribute."));
                _responseEnvelope.Status = false;
            }
            return Ok(_responseEnvelope);
        }

        [HttpPost]
        [ActionName("InsertRespondentAttribute")]
        [Route("api/auto/manage/InsertRespondentAttribute")]
        public IHttpActionResult InsertRespondentAttribute(WebServiceResponse query)
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            string status = "Error adding respondent attribute.";
            if (AutomationManager.InsertRespondentAttribute(query))
            {
                _responseEnvelope.Data.Info = "Respondent attribute successfully added.";
                _responseEnvelope.Data.Errors = null;
            }
            else
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject("-1", "ErrorAutomation", "-1", status + "The Offer Id for this respondent attribute does not exist."));
                _responseEnvelope.Status = false;
            }
            return Ok(_responseEnvelope);
        }

        [HttpPost]
        [ActionName("InsertTerm")]
        [Route("api/auto/manage/InsertTerm")]
        public IHttpActionResult InsertTerm(WebServiceResponse query)
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            string status = "Error adding Term.";
            if (AutomationManager.InsertTerm(query))
            {
                _responseEnvelope.Data.Info = "Term successfully added.";
                _responseEnvelope.Data.Errors = null;
            }
            else
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject("-1", "ErrorAutomation", "-1", status + " The Offer Id for this Term does not exist."));
                _responseEnvelope.Status = false;
            }
            return Ok(_responseEnvelope);
        }

        [ActionName("SetLiveMatchInternalPIDResponse")]
        [Route("api/auto/manage/SetLiveMatchInternalPIDResponse")]
        public IHttpActionResult SetLiveMatchInternalPIDResponse(WebServiceResponse query)
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            string status = "Error reading Internal PID from XML";
            if (AutomationManager.LiveMatchInternalPIDResponse(query))
            {
                _responseEnvelope.Data.Info = "Internal PID successfully parsed and set. Internal PID=" + AutomationManager.internalpid;
                _responseEnvelope.Data.Errors = null;
            }
            else
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject("-1", "ErrorAutomation", "-1", status));
                _responseEnvelope.Status = false;
            }
            return Ok(_responseEnvelope);
        }

        [ActionName("SetLiveMatchFetchNonPanelistStudiesResponse")]
        [Route("api/auto/manage/SetLiveMatchFetchNonPanelistStudiesResponse")]
        public IHttpActionResult SetLiveMatchFetchNonPanelistStudiesResponse(WebServiceResponse query)
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            if (AutomationManager.LiveMatchFetchNonPanelistStudiesResponse(query))
            {
                _responseEnvelope.Data.Info = AutomationManager.studiesList.ToString();
                _responseEnvelope.Data.Errors = null;
            }
            else
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject("-1", "ErrorAutomation", "-1", "Error reading studies list from XML"));
                _responseEnvelope.Status = false;
            }
            return Ok(_responseEnvelope);
        }

        [ActionName("SetThirdpartyRespondentProfileResponse")]
        [Route("api/auto/manage/SetThirdpartyRespondentProfileResponse")]
        public IHttpActionResult SetThirdpartyRespondentProfileResponse(WebServiceResponse query)
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            if (AutomationManager.ThirdpartyRespondentProfileResponse(query))
            {
                _responseEnvelope.Data.Info = AutomationManager.respondentCatalogueAttributes.ToString();
                _responseEnvelope.Data.Errors = null;
            }
            else
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject("-1", "ErrorAutomation", "-1", "Error reading respondent catalog attributes from XML"));
                _responseEnvelope.Status = false;
            }
            return Ok(_responseEnvelope);
        }

        [ActionName("SetQuotaExpressions")]
        [Route("api/auto/manage/SetQuotaExpressions")]
        public IHttpActionResult SetQuotaExpressions(WebServiceResponse query)
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            string status = "Error saving Quota Expressions.";
            if (AutomationManager.QuotaExpressions(query))
            {
                _responseEnvelope.Data.Info = "Quota Expressions saved for sample id.";
                _responseEnvelope.Data.Errors = null;
            }
            else
            {
                _responseEnvelope.Data.Errors.Add(new ErrorObject("-1", "ErrorAutomation", "-1", status + " Either SampleId specified does not exists or invalid OfferId"));
                _responseEnvelope.Status = false;
            }
            return Ok(_responseEnvelope);
        }

        [Route("api/auto/manage/reset")]
        public IHttpActionResult Get()
        {
            AutomationResponse _responseEnvelope = new AutomationResponse(true);
            AutomationManager.reset();
            _responseEnvelope.Data.Info = "Data has been reset.";
            _responseEnvelope.Data.Errors = null;
            return Ok(_responseEnvelope);
        }
    }



    public class AttributeWithSettings
    {
        public string attribute { get; set; }
    }
}
