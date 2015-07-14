using Kikai.BL.DTO.ApiObjects;
using System;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
    public interface IOfferAttributeRepository
    {
        List<OfferAttributeApiObject> GetOfferAttributes(Guid offerId);
    }
}
