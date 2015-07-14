using Kikai.BL.Concrete;
using Kikai.BL.DTO;
using Kikai.Common.Managers;
using Kikai.Domain.Common;
using Kikai.Logging.Utils;
using Kikai.WebAdmin.Common;
using Kikai.WebAdmin.DTO;
using Kikai.WebAdmin.Extensions;
using Kikai.WebAdmin.Managers;
using Kikai.WebAdmin.UIModels;
using Kikai.WebAdmin.Utils;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kikai.WebAdmin.Controllers
{
    [CustomAuthorizeAttribute]
    public class OfferController : Controller
    {

        private MessagesUtil msgUtil;

        private static Thread activationThread = new Thread(new ParameterizedThreadStart((newOffer) =>
            {
                activationProcedure(newOffer);
            }
                 ));

        public OfferController()
        {
            msgUtil = new MessagesUtil();
        }
        //
        // GET: /Offer/        
        public ActionResult Index()
        {
            Session.Remove("ProviderName");
            Session.Remove("AttributeId");
            Session.Remove("Published");
            Session.Remove("Page"); ;
            Session.Remove("Provider_PageSize");
            Session.Remove("AttributePageSize");
            return View();
        }

        //
        // GET: /Offer/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }


        public ActionResult OfferResult(OfferSearchObject searchObject, string sort = "", string sortdir = "")
        {
            var offerList = new OfferModel();
            offerList.PageSize = searchObject.PageSize;
            offerList.Page = searchObject.Page;

            //Active column is ambiguous in the database so we should specify that we are sorting the active offers
            sort = (sort == "Status") ? "o.Status" : sort;

            try
            {
                offerList.Offers = new OfferRepository().GetFilteredOffers(searchObject.StudyId, searchObject.OfferTitle, searchObject.OfferStatus, searchObject.OfferType, searchObject.Page - 1, searchObject.PageSize, sort, sortdir).ToList();
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_GETOFFERS_EXCEPTION), searchObject.StudyId, searchObject.OfferTitle, searchObject.OfferStatus, searchObject.OfferType, searchObject.Page, searchObject.PageSize), e);
                return null;
            }

            //try
            //{
            //    var sortedOffers = offerList.Offers.AsQueryable();
            //    offerList.Offers = SortIQueryable<FilteredOfferObject>(sortedOffers, sort, sortdir).ToList();
            //}
            //catch (Exception e)
            //{
            //    log.Error(@"A problem occurred while sorting the Offers list.", e);
            //}

            SetSearchSessionVariables(searchObject);

            return PartialView("_OfferResult", offerList);

        }

        private void SetSearchSessionVariables(OfferSearchObject searchObject)
        {
            Session["StudyId"] = searchObject.StudyId;
            Session["OfferTitle"] = searchObject.OfferTitle;
            Session["OfferStatus"] = searchObject.OfferStatus;
            Session["OfferType"] = searchObject.OfferType;
            Session["Page"] = searchObject.Page;
            Session["OfferPageSize"] = searchObject.PageSize;
        }

        //Edit page
        //Get /Offer/Edit/{id}
        [CustomAuthorize(UserPermission.OFFER_CAN_EDIT_OFFERS, UserPermission.OFFER_ADMIN)]
        public ActionResult Edit(string id)
        {
            //Test if one of the required parameters is not supplied in the URL
            Guid oid = new Guid();
            OfferObject offer = new OfferObject();
            if (id == null || Guid.TryParse(id, out oid) == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                offer = new OfferRepository().SelectByID(oid);
                if (offer == null)
                    return RedirectToAction("Index");
                else
                    return View(offer);
            }
        }

        //Offer details page
        //Get /Offer/View/{id}
        public new ActionResult View(string id)
        {
            //Test if one of the required parameters is not supplied in the URL
            Guid oid = new Guid();
            OfferObject offer = new OfferObject();
            if (id == null || Guid.TryParse(id, out oid) == false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                offer = new OfferRepository().SelectByID(oid);
                if (offer == null)
                    return RedirectToAction("Index");
                else
                    return View(offer);
            }
        }


        //This partial view is used in the Edit offer section
        [CustomAuthorize(UserPermission.OFFER_CAN_EDIT_OFFERS, UserPermission.OFFER_ADMIN)]
        public ActionResult GetDropDownOfferAttributes(OfferAttributesSearchObject searchObject)
        {
            var OfferAttributes = new OfferAttributesModel();

            try
            {
                var Attributes = new RespondentAttributeRepository().SelectByOffer(searchObject.OfferId).ToList();
                OfferAttributes.Attributes = Attributes;
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(string.Format(msgUtil.GetMessage(MessageKey.LOG_GETDROPDOWNOFFERATTRIBUTES_EXCEPTION), searchObject.OfferId), e);
            }

            return PartialView("_OfferAttributesDropDownResult", OfferAttributes);
        }

        //This partial view is used in the Edit offer section
        [CustomAuthorize(UserPermission.OFFER_CAN_EDIT_OFFERS, UserPermission.OFFER_ADMIN)]
        public ActionResult GetOfferAttributes(OfferAttributesSearchObject searchObject)
        {
            var OfferAttributes = new OfferAttributesModel();

            try
            {
                var Attributes = new RespondentAttributeRepository().SelectByOffer(searchObject.OfferId).ToList();
                OfferAttributes.TotalRows = Attributes.Count;
                OfferAttributes.Page = searchObject.Page;
                OfferAttributes.PageSize = 10;
                OfferAttributes.Attributes = Attributes.Skip((OfferAttributes.Page - 1) * OfferAttributes.PageSize).Take(OfferAttributes.PageSize).ToList();
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_GETOFFERATTRIBUTESEDIT_EXCEPTION), searchObject.OfferId), e);
            }

            return PartialView("_OfferAttributesResult", OfferAttributes);
        }

        //This partial view is used in the View offer details sections        
        public ActionResult GetOfferAttributesView(OfferAttributesSearchObject searchObject)
        {
            var OfferAttributes = new OfferAttributesModel();
            try
            {
                var Attributes = new RespondentAttributeRepository().SelectByOffer(searchObject.OfferId).ToList();
                OfferAttributes.TotalRows = Attributes.Count;
                OfferAttributes.Page = searchObject.Page;
                OfferAttributes.PageSize = 10;
                OfferAttributes.Attributes = Attributes.Skip((OfferAttributes.Page - 1) * OfferAttributes.PageSize).Take(OfferAttributes.PageSize).ToList();
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_GETOFFERATTRIBUTESVIEW_EXCEPTION), searchObject.OfferId), e);
            }
            return PartialView("_OfferAttributesViewResult", OfferAttributes);
        }


        //
        // POST: /Offer/Edit/5
        //This function is used to update an offer
        //If enabled is true then this is a mainstream enabled offer
        //If enabled is false then this offer has been suspended on SAM and should be suspend in the offer service
        [HttpPost]
        [CustomAuthorize(UserPermission.OFFER_CAN_EDIT_OFFERS, UserPermission.OFFER_ADMIN)]
        public void EditSubmit(string result)
        {
            OfferObject offer = JsonConvert.DeserializeObject<OfferObject>(result);
            offer.Status = new OfferRepository().SelectByID(offer.Id).Status;
            try
            {
                //Update the offer values
                new OfferRepository().Update(offer);
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_UPDATEOFFER_EXCEPTION), offer.Id, offer.SampleId), e);
            }
        }

        public static IQueryable<T> SortIQueryable<T>(IQueryable<T> data, string fieldName, string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(fieldName)) return data;
            if (string.IsNullOrWhiteSpace(sortOrder)) return data;

            var param = Expression.Parameter(typeof(T), "i");
            Expression conversion = Expression.Convert(Expression.Property(param, fieldName), typeof(object));
            var mySortExpression = Expression.Lambda<Func<T, object>>(conversion, param);

            return (sortOrder == "DESC") ? data.OrderByDescending(mySortExpression) : data.OrderBy(mySortExpression);
        }

        public int? GetStatus(Guid OfferId)
        {
            var offer = new OfferRepository().SelectByID(OfferId);

            int? Status = -1;

            if (offer != null && offer.Terms.Count() > 0 && !string.IsNullOrEmpty(offer.Title))
            {
                Status = offer.Status;
            }

            return Status;
        }

        public int? GetQuotaRemaining(Guid OfferId)
        {
            return new OfferRepository().SelectByID(OfferId).QuotaRemaining;
        }

        [HttpPut]
        [CustomAuthorize(UserPermission.OFFER_CAN_EDIT_OFFERS, UserPermission.OFFER_ADMIN)]
        public int UpdateStatus(string result)
        {
            OfferObject offer = JsonConvert.DeserializeObject<OfferObject>(result);
            activationThread.Abort();
            int suspendedCode = 0;
            bool setToPending = false;
            activationThread = new Thread(new ParameterizedThreadStart((newOffer) =>
            {
                try
                {
                    offer.Status = activationProcedure(newOffer, out suspendedCode);
                }
                catch (Exception e)
                {
                    LoggerFactory.GetLogger().Error(String.Format(msgUtil.GetMessage(MessageKey.LOG_UPDATEOFFERACTIVATE_EXCEPTION), offer.Id, offer.SampleId), e);
                    setToPending = true;
                }
            }
            ));

            new OfferRepository().UpdateRetryCount((int)offer.SampleId, 0);
            try
            {
                if (offer.Status == (int)OfferStatus.Active)
                {
                    activationThread.Start(offer);
                    if (!activationThread.Join(5000))
                    {
                        setToPending = true;
                    }
                }
                else if (offer.Status == (int)OfferStatus.Inactive)
                {
                    offer.Status = (int)OfferStatus.Inactive;
                    new OfferRepository().UpdateRetryCount((int)offer.SampleId, 0);
                    new OfferRepository().UpdateOfferStatus((int)offer.SampleId, (int)offer.Status);
                }
            }
            catch (Exception e)
            {
                LoggerFactory.GetLogger().Error(string.Format(msgUtil.GetMessage(MessageKey.LOG_UPDATEOFFERSTATUS_EXCEPTION), offer.Id, offer.SampleId), e);
            }
            if (setToPending)
            {
                offer.Status = (int)OfferStatus.Pending;
                new OfferRepository().UpdateRetryCount((int)offer.SampleId, 0);
                new OfferRepository().UpdateOfferStatus((int)offer.SampleId, (int)offer.Status);
            }

            return (suspendedCode == 0) ? (int)offer.Status : suspendedCode;
        }

        private static int activationProcedure(object offerObject)
        {
            int suspendedCode;
            return activationProcedure (offerObject, out suspendedCode);
        }

        private static int activationProcedure(object offerObject, out int suspendedCode)
        {
            OfferObject offer = (OfferObject)offerObject;
            SampleManager sampleManager;
            int status = (int)offer.Status;
            
            sampleManager = new SampleManager(offer.SampleId.ToString());
            status = sampleManager.Activate((int)offer.SampleId, (Guid)offer.Id, out suspendedCode);

            return status;
        }
    }
}
