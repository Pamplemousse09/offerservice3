using Kikai.BL.DTO;
using Kikai.BL.IRepository;
using Kikai.Domain.Common;
using Kikai.Internal.Contracts.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kikai.BL.Concrete
{
    public class RespondentAttributeRepository:IGenericRepository<RespondentAttributeObject>, IRespondentAttributeRepository
    {
        public IEnumerable<RespondentAttributeObject> SelectAll()
        {
            return new DataUtils().GetList<RespondentAttributeObject>("EXEC RespondentAttribute_GetAll").ToList();
        }

        public RespondentAttributeObject SelectByID(object id)
        {
            return new DataUtils().GetList<RespondentAttributeObject>("EXEC RespondentAttribute_GetById @Id=@P1", id).ToList().FirstOrDefault();
        }

        public IEnumerable<RespondentAttributeObject> SelectByOffer(Guid OfferId)
        {
            try
            {
                return new DataUtils().GetList<RespondentAttributeObject>("EXEC RespondentAttribute_GetOfferAttributes @OfferId=@P1", OfferId).ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// This function takes the Offer Id and the Attribute Identifier as parameters and returns the correspondent RespondentAttribute
        /// </summary>
        /// <param name="OfferId"></param>
        /// <param name="Ident"></param>
        /// <returns></returns>
        public RespondentAttributeObject SelectRespondentAttribute(Guid OfferId, String Ident)
        {
            return new DataUtils().GetList<RespondentAttributeObject>("EXEC RespondentAttribute_GetRespondentAttribute @OfferId=@P1, @Ident=@P2", OfferId, Ident).ToList().FirstOrDefault();
        }

        public void Insert(RespondentAttributeObject obj)
        {
            try
            {
                new DataUtils().ExcecuteCommand(@"EXEC RespondentAttribute_Add @OfferId=@P1, 
                                    @Ident=@P2, 
                                    @Values=@P3",
                                        obj.OfferId,
                                        obj.Ident,
                                        obj.Values);
            }
            catch
            {
                throw;
            }
        }

        public void Update(RespondentAttributeObject obj)
        {
            new DataUtils().ExcecuteCommand(@"EXEC RespondentAttribute_Update @Id=@P1, 
                                    @OfferId=@P2, 
                                    @Ident=@P3, 
                                    @Values=@P4",
                                    obj.Id,
                                    obj.OfferId,
                                    obj.Ident,
                                    obj.Values);
        }

        /// <summary>
        /// This function is used in the thread in order to Update or Delete the attributes that are returned by the service
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateOrRemove(Guid OfferId, IEnumerable<AttributeValuesObject> returnedAttributes)
        {
            //Get the list of respondent attribute for the given Offer Id
            IEnumerable<RespondentAttributeObject> offerAttributes = SelectByOffer(OfferId);
            //Get the list of supported attributes(to be removed after integration of steam service)
            List<string> supportedAttributes = new Constants().SupportedAttributes;
            //loop through all the existing attributes in the respondentAttribute of an offer
            foreach (var attr in offerAttributes)
            {
                //We are doing this check because the service now only returns supported attributes and the update must
                //only occure on those attribute(this check is to be removed after integration of steam service)
                if (supportedAttributes.Contains(attr.Ident))
                {
                    //Get the attribute from the service
                    var attributeFromService = returnedAttributes.Where(a => a.Ident == attr.Ident);

                    //Check if the offer attribute is returned by the service then update
                    if (attributeFromService.Count() == 1)
                    {
                        new AttributeRepository().ProccessAttribute(attr.Ident);
                        attr.Values = returnedAttributes.Where(a => a.Ident == attr.Ident).FirstOrDefault().Values;
                        Update(attr);
                        //Remove the attribute from the list for adding the unprocessed attributes
                        returnedAttributes = returnedAttributes.Except(attributeFromService);
                    }
                    //If it is not returned delete the respondent attribute
                    else
                    {
                        Delete(attr.Id);
                    }
                }
            }

            foreach (var attr in returnedAttributes)
            {
                if (supportedAttributes.Contains(attr.Ident))
                {
                    new AttributeRepository().ProccessAttribute(attr.Ident);

                    //This block initializes the respondent attribute to be added
                    if (this.SelectRespondentAttribute(OfferId, attr.Ident) == null)
                    {
                        RespondentAttributeObject newRespondentAttribute = new RespondentAttributeObject();
                        newRespondentAttribute.OfferId = OfferId;
                        newRespondentAttribute.Ident = attr.Ident;
                        newRespondentAttribute.Values = attr.Values;

                        Insert(newRespondentAttribute);
                    }
                }
            }
        }

        public void Delete(object id)
        {
            try
            {
                new DataUtils().ExcecuteCommand(@"EXEC RespondentAttribute_Delete @Id=@P1", id);
            }
            catch
            {
                throw;
            }
        }

    }
}
