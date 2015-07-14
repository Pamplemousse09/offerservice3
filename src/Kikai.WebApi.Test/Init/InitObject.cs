using Kikai.BL.DTO;
using Kikai.BL.DTO.ApiObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kikai.WebAPI.Test.Init
{
    public class InitObject
    {
        #region Methods
        public OfferObject OfferObject(int status, bool test)
        {
            OfferObject offer = new OfferObject();
            offer.Status = status;
            offer.TestOffer = test;
            offer.Id = new Guid();
            offer.SampleId = 1;
            return offer;
        }

        public OfferApiObject OfferApiObject(bool test)
        {
            OfferApiObject offer = new OfferApiObject();
            offer.TestOffer = test;
            offer.OfferID = new Guid();
            offer.Title = "Xunit Offer";
            return offer;
        }

        public OfferAttributeApiObject OfferAttributeApiObject(string id, string value)
        {
            OfferAttributeApiObject attribute = new OfferAttributeApiObject();
            attribute.Name = id;
            attribute.Value = value;
            return attribute;
        }

        public List<OfferAttributeApiObject> ListOfferAttributeApiObject()
        {
            List<OfferAttributeApiObject> attributesList = new List<OfferAttributeApiObject>();
            attributesList.Add(OfferAttributeApiObject("COREcontact_country", "165"));
            attributesList.Add(OfferAttributeApiObject("CORElanguage", "14"));
            return attributesList;
        }

        public List<OfferAttributeApiObject> ListOfferUnpublishedAttributeApiObject()
        {
            List<OfferAttributeApiObject> attributesList = new List<OfferAttributeApiObject>();
            return attributesList;
        }

        public List<AttributeObject> ListRequiredAttributes()
        {
            List<AttributeObject> requiredAttributesList = new List<AttributeObject>();
            requiredAttributesList.Add(new AttributeObject() { Id = "COREcontact_country" });
            requiredAttributesList.Add(new AttributeObject() { Id = "CORElanguage" });
            return requiredAttributesList;
        }

        public List<StudyOfferObject> ListStudyOfferObject(bool excludeOffer = false)
        {
            List<StudyOfferObject> listStudyOffer = new List<StudyOfferObject>();
            if (excludeOffer)
                listStudyOffer.Add(new StudyOfferObject() { StudyId = 1, OfferId = new Guid("00000000-0000-0000-0000-000000000001") });
            else
                listStudyOffer.Add(new StudyOfferObject() { StudyId = 1, OfferId = new Guid() });
            return listStudyOffer;
        }
        #endregion
    }
}
