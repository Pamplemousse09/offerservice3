using Kikai.BL.DTO;
using System.Collections.Generic;

namespace Kikai.BL.IRepository
{
    public interface IAttributeOptionRepository 
    {
        IEnumerable<AttributeOptionObject> SelectListByID(object id);
    }
}
