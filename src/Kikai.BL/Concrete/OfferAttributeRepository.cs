using Kikai.BL.DTO.ApiObjects;
using Kikai.BL.IRepository;
using System;
using System.Collections.Generic;

namespace Kikai.BL.Concrete
{
    public class OfferAttributeRepository : IOfferAttributeRepository
    {
        public List<OfferAttributeApiObject> GetOfferAttributes(Guid offerId)
        {
            return new DataUtils().GetList<OfferAttributeApiObject>("EXEC RespondentAttribute_GetOfferAttributesApi @OfferId=@P1", offerId);
        }

        public List<AttributeUsageApiObject> GetRpcOfferAttributes(Guid offerId)
        {
            return new DataUtils().GetList<AttributeUsageApiObject>("EXEC RespondentAttribute_GetRpcOfferAttributes @OfferId=@P1", offerId);
        }
    }
}
