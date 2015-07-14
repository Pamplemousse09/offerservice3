using Kikai.BL.IRepository;
using Kikai.BL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kikai.BL.DTO.ApiObjects;

namespace Kikai.BL.Concrete
{
    public class LiveOfferRepository : ILiveOfferRepository
    {
        /// <summary>
        /// Gets the list of Offers that are active and have a valid Term
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LiveOfferApiObject> GetAvailableOffer()
        {
            return new DataUtils().GetList<LiveOfferApiObject>("EXEC Offer_GetAvailableOffers");
        }
    }
}

