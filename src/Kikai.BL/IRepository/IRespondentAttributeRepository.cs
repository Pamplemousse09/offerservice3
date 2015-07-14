using Kikai.BL.DTO;
using Kikai.Internal.Contracts.Objects;
using System;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
    public interface IRespondentAttributeRepository
    {
        RespondentAttributeObject SelectRespondentAttribute(Guid OfferId, String Ident);

        IEnumerable<RespondentAttributeObject> SelectByOffer(Guid OfferId);

        void UpdateOrRemove(Guid OfferId, IEnumerable<AttributeValuesObject> returnedAttributes);
    }
}
